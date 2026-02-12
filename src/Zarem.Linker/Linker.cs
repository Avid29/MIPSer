// Adam Dernis 2024

using CommunityToolkit.Diagnostics;
using System.Collections.Generic;
using Zarem.Assembler.Logging;
using Zarem.Assembler.Models.Modules;
using Zarem.Config;
using Zarem.Extensions.System.IO;
using Zarem.Models.Modules;
using Zarem.Models.Modules.Tables;
using Zarem.Models.Modules.Tables.Enums;

namespace Zarem.Linker;

/// <summary>
/// A MIPS linker.
/// </summary>
public abstract class Linker<TLinker>
    where TLinker : Linker<TLinker>, new()
{
    private readonly Logger _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="Linker{TLinker}"/> class.
    /// </summary>
    public Linker()
    {
        _logger = new Logger();
        Module = new Module();
    }

    /// <summary>
    /// Gets the resulting linked module being built.
    /// </summary>
    protected Module Module { get; }

    /// <summary>
    /// Links an array of object modules into a single object module.
    /// </summary>
    public static Module Link<TModule, TConfig>(TConfig config, params TModule[] modules)
        where TModule : IBuildModule<TModule, TConfig>
        where TConfig : FormatConfig
        => Link(null, config, modules);

    /// <summary>
    /// Links an array of object modules into a single object module.
    /// </summary>
    public static Module Link<TModule, TConfig>(string? entryPoint, TConfig config, params TModule[] modules)
    where TModule : IBuildModule<TModule, TConfig>
    where TConfig : FormatConfig
    {
        Module[] abstracteds = new Module[modules.Length];
        for (int i = 0; i < modules.Length; i++)
        {
            var module = modules[i].Abstract(config);
            if (module is null)
            {
                // TODO: Linker errors
                continue;
            }
        }

        return Link(entryPoint, abstracteds);
    }

    /// <summary>
    /// Links an array of object modules into a single object module.
    /// </summary>
    public static Module Link(params Module[] modules)
        => Link(null, modules);

    /// <summary>
    /// Links an array of object modules into a single object module.
    /// </summary>
    public static Module Link(string? entryPoint, params Module[] modules)
    {
        var linker = new TLinker();
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
            linker.Module.SetEntryPoint(entryPoint);
        }

        return linker.Module;
    }

    private void LinkModule(Module module)
    {
        // Track the initial positions of each stream
        var offsets = new Dictionary<string, long>();

        // Copy stream contents
        foreach (var srcSection in module.Sections.Values)
        {
            var destSection = Module.GetOrAddSection(srcSection.Name);

            offsets.Add(destSection.Name, destSection.Stream.Position);
            destSection.Append(srcSection.Stream);
        }

        // Merge symbol tables
        foreach (var symEntry in module.Symbols.Values)
        {
            // TODO: What flags are tracked?

            Module.TryDefineOrUpdateSymbol(symEntry.Name, symEntry.Type, symEntry.Address);
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
            Module.TryTrackReference(entry);

            // Relocate apply relocations
            if (entry.Symbol is null)
            {
                Relocate(entry, offset);
            }
        }
    }

    private void AlignSections()
    {
        uint position = 0x00;
        foreach (var section in Module.Sections.Values)
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
        foreach (var entry in Module.References)
        {
            // Skip relocations. Everything has been relocated
            if (entry.Symbol is null || !Module.TryGetSymbol(entry.Symbol, out var symbol))
                continue;

            Guard.IsNotNull(entry.Location.Section);
            Guard.IsNotNull(symbol.Address?.Section);

            var section = Module.Sections[entry.Location.Section];
            var symbolSection = Module.Sections[symbol.Address.Value.Section];
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

    /// <summary>
    /// Applies a relocation.
    /// </summary>
    /// <param name="entry">The reference entry detailing the relocation.</param>
    /// <param name="offset">The offset amount of the relocation.</param>
    protected abstract void Relocate(ReferenceEntry entry, long offset);
}
