// Adam Dernis 2024

using Zarem.Assembler.MIPS.Config;
using Zarem.Assembler.MIPS.Logging;
using Zarem.Assembler.MIPS.Models.Modules;
using Zarem.Assembler.MIPS.Models.Modules.Interfaces;
using CommunityToolkit.Diagnostics;
using Zarem.MIPS.Models.Modules.Tables;
using Zarem.MIPS.Models.Modules.Tables.Enums;
using System.Collections.Generic;
using System.IO;

namespace Zarem.Linker.MIPS;

/// <summary>
/// A MIPS linker.
/// </summary>
public class Linker<TModule, TConfig>
    where TModule : IBuildModule<TModule>
    where TConfig : AssemblerBuildConfig<TConfig, TModule>
{
    private readonly Logger _logger;
    private readonly Module _module;

    private Linker(AssemblerConfig config)
    {
        _logger = new Logger();
        _module = new Module();

        Config = config;
    }

    /// <summary>
    /// Gets the assembler's configuration.
    /// </summary>
    public AssemblerConfig Config { get; }

    /// <summary>
    /// Links an array of object modules into a single object module.
    /// </summary>
    public static Module Link(TConfig config, params TModule[] modules)
        => Link(config, null, modules);

    /// <summary>
    /// Links an array of object modules into a single object module.
    /// </summary>
    public static Module Link(TConfig config, string? entryPoint, params TModule[] modules)
    {
        Module[] abstracteds = new Module[modules.Length];
        for(int i = 0; i < modules.Length; i++)
        {
            var module = modules[i].Abstract(config);
            if (module is null)
            {
                // TODO: Linker errors
                continue;
            }
        }

        return Link(config, entryPoint, abstracteds);
    }

    /// <summary>
    /// Links an array of object modules into a single object module.
    /// </summary>
    public static Module Link(TConfig config, params Module[] modules)
        => Link(config, null, modules);

    /// <summary>
    /// Links an array of object modules into a single object module.
    /// </summary>
    public static Module Link(TConfig config, string? entryPoint, params Module[] modules)
    {
        var linker = new Linker<TModule, TConfig>(config);
        foreach (var module in modules)
        {
            linker.LinkModule(module);
        }

        linker.AlignSections();
        linker.ResolveReferences();

        if (entryPoint is not null)
        {
            // TODO: Verify the module is execution ready

            // Set the entry point
            linker._module.SetEntryPoint(entryPoint);
        }

        return linker._module;
    }

    private void LinkModule(Module module)
    {
        // Track the initial positions of each stream
        var offsets = new Dictionary<string, long>();

        // Copy stream contents
        foreach (var srcSection in module.Sections.Values)
        {
            var destSection = _module.GetOrAddSection(srcSection.Name);

            offsets.Add(destSection.Name, destSection.Stream.Position);
            destSection.Append(srcSection.Stream);
        }
        
        // Merge symbol tables
        foreach (var symEntry in module.Symbols.Values)
        {
            // TODO: What flags are tracked?

            _module.TryDefineOrUpdateSymbol(symEntry.Name, symEntry.Type, symEntry.Address);
        }

        // Append references and apply relocations
        foreach (var @ref in module.References)
        {
            ReferenceEntry entry = @ref;
            Guard.IsNotNull(entry.Location.Section);

            // Update address
            long offset = offsets[entry.Location.Section];
            entry.Location += offset;

            // Add to tracked references
            // (All references will be applied after all modules are merged)
            _module.TryTrackReference(entry);

            // Relocate apply relocations
            if (entry.Symbol is null)
            {
                _module.Relocate(entry, offset);
            }
        }
    }

    private void AlignSections()
    {
        uint position = 0x00;
        foreach (var section in _module.Sections.Values)
        {
            section.VirtualAddress = position;
            position += (uint)section.Stream.Length;

            // Ensure word alignment
            uint offset = position % 4;
            if (offset != 0)
            {
                position += 4 - offset;
            }
        }
    }

    private void ResolveReferences()
    {
        foreach (var entry in _module.References)
        {
            // Skip relocations. Everything has been relocated
            if (entry.Symbol is null || !_module.TryGetSymbol(entry.Symbol, out var symbol))
                continue;

            Guard.IsNotNull(entry.Location.Section);
            Guard.IsNotNull(symbol.Address?.Section);

            var section = _module.Sections[entry.Location.Section];
            var symbolSection = _module.Sections[symbol.Address.Value.Section];
            var baseLocation = entry.Location.Value;
            var symbolLocation = symbolSection.VirtualAddress + symbol.Address.Value.Value;

            switch (entry.Type)
            {
                case MipsReferenceType.Low16:
                    section.Stream.Position = baseLocation + 2;
                    section.Stream.TryWrite((ushort)symbolLocation);
                    break;
            }
        }
    }
}
