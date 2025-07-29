// Adam Dernis 2024

using MIPS.Assembler.Logging;
using MIPS.Assembler.Logging.Enum;
using MIPS.Assembler.Models;
using MIPS.Assembler.Models.Modules;
using MIPS.Assembler.Models.Modules.Interfaces;
using MIPS.Models.Addressing.Enums;
using MIPS.Models.Modules.Tables;

namespace MIPS.Linker;

/// <summary>
/// A MIPS linker.
/// </summary>
public class Linker
{
    private readonly Logger _logger;
    private readonly ModuleConstructor _module;

    private Linker(AssemblerConfig config)
    {
        _logger = new Logger();
        _module = new ModuleConstructor();

        Config = config;
    }

    /// <summary>
    /// Gets the assembler's configuation.
    /// </summary>
    public AssemblerConfig Config { get; }

    /// <summary>
    /// Links an array of object modules into a single object module.
    /// </summary>
    /// <param name="modules"></param>
    /// <param name="config"></param>
    /// <returns></returns>
    public static Linker Link(IBuildModule[] modules, AssemblerConfig config)
    {
        var linker = new Linker(config);
        foreach (var m in modules)
        {
            var module = m.Abstract(config);
            if (module is null)
            {
                linker._logger.Log(Severity.Error, LogId.FailedToLoadModule, "A module could not be loaded, and therefore failed to link.");
                continue;
            }

            linker.LinkModule(module);
        }

        return linker;
    }

    private void LinkModule(ModuleConstructor module)
    {
        // Track the initial positions of each stream
        var offsets = new long[ModuleConstructor.SECTION_COUNT];

        // Copy stream contents
        for (int i = 0; i < ModuleConstructor.SECTION_COUNT; i++)
        {
            offsets[i] = module.GetStreamPosition((Section)i);
            _module.Append((Section)i, module.Sections[i]);
        }
        
        // Merge symbol tables
        foreach (var symEntry in module.Symbols.Values)
        {
            // TODO: What flags are tracked?

            module.TryDefineOrUpdateSymbol(symEntry.Name, symEntry.Type, symEntry.Address);
        }

        // Append references and apply relocations
        foreach (var @ref in module.References)
        {
            ReferenceEntry entry = @ref;

            // Update address
            long offset = offsets[(int)@ref.Address.Section];
            entry.Address += offset;

            // Add to tracked references
            _module.TryTrackReference(entry);

            if (entry.IsRelocation)
            {
                _module.Relocate(entry, offset);
            }
        }

        // Resolve all references
        foreach (var @ref in _module.References)
        {
            // Skip if the symbol is not defined
            if (!(@ref.Symbol is not null && _module.TryGetSymbol(@ref.Symbol, out var symbol) && symbol.IsDefined))
                continue;

        }
    }
}
