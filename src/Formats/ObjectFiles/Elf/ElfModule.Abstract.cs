// Avishai Dernis 2025

using LibObjectFile.Elf;
using LibObjectFile.PE;
using MIPS.Assembler.Models.Config;
using MIPS.Assembler.Models.Modules;
using MIPS.Models.Addressing;
using MIPS.Models.Modules.Tables;
using MIPS.Models.Modules.Tables.Enums;
using ObjectFiles.Elf.Extensions;
using static System.Collections.Specialized.BitVector32;

namespace ObjectFiles.Elf;

public partial class ElfModule
{
    /// <inheritdoc/>
    public Module? Abstract(AssemblerConfig config)
    {
        var module = new Module();

        foreach (var section in _elfFile.Sections)
        {
            _ = section switch
            {
                // Stream sections
                ElfStreamSection streamSection => AbstractStreamSection(module, streamSection),
                ElfSymbolTable symbolTable => AbstractSymbolTable(module, symbolTable),
                ElfRelocationTable relocationTable => AbstractRelocationTable(module, relocationTable),
                _ => false,
            };
        }

        return module;
    }

    private bool AbstractStreamSection(Module module, ElfStreamSection streamSection)
    {
        var sectionName = streamSection.Name.Value;

        module.AddSection(sectionName, SectionFlags.Default);
        module.Append(sectionName, streamSection.Stream, true);
        return true;
    }

    private bool AbstractSymbolTable(Module module, ElfSymbolTable symbolTable)
    {
        foreach(var elfSymbol in symbolTable.Entries)
        {
            var name = elfSymbol.Name.Value;
            if (name is null)
                continue;

            var sectionName = elfSymbol.SectionLink.Section?.Name.Value;
            Address? value = sectionName is null ? null : new Address((long)elfSymbol.Value, sectionName);
            var binding = elfSymbol.Bind.FromElf();

            module.TryDefineSymbol(name, SymbolType.Label, value, binding);
        }

        return true;
    }

    private bool AbstractRelocationTable(Module module, ElfRelocationTable relocationTable)
    {
        foreach(var relEntry in relocationTable.Entries)
        {
            var symbolIndex = relEntry.SymbolIndex;
        }

        return true;
    }
}
