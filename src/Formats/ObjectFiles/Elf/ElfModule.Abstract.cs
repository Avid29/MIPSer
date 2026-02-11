// Avishai Dernis 2025

using LibObjectFile.Elf;
using LibObjectFile.PE;
using MIPS.Assembler.Config;
using MIPS.Assembler.Models.Modules;
using MIPS.Models.Addressing;
using MIPS.Models.Modules.Tables;
using MIPS.Models.Modules.Tables.Enums;
using ObjectFiles.Elf.Extensions;
using System.Linq;
using static System.Collections.Specialized.BitVector32;

namespace ObjectFiles.Elf;

public partial class ElfModule
{
    private ref struct ElfAbstractContext
    {
        private ElfSymbolTable? _symTab;

        public ElfAbstractContext(ElfFile elfFile)
        {
            Module = new();
        }

        public Module Module { get; }

        public bool AbstractStreamSection(ElfStreamSection streamSection)
        {
            var sectionName = streamSection.Name.Value;

            Module.AddSection(sectionName, SectionFlags.Default);
            Module.Append(sectionName, streamSection.Stream, true);
            return true;
        }

        public bool AbstractSymbolTable(ElfSymbolTable symbolTable)
        {
            _symTab = symbolTable;

            foreach (var elfSymbol in symbolTable.Entries)
            {
                var name = elfSymbol.Name.Value;
                if (name is null)
                    continue;

                var sectionName = elfSymbol.SectionLink.Section?.Name.Value;
                Address? value = sectionName is null ? null : new Address((long)elfSymbol.Value, sectionName);
                var binding = elfSymbol.Bind.FromElf();

                Module.TryDefineSymbol(name, SymbolType.Label, value, binding);
            }

            return true;
        }

        public bool AbstractRelocationTable(ElfRelocationTable relocationTable)
        {
            if (_symTab is null)
                return false;

            var sectionName = relocationTable.Info.Section?.Name.Value;
            if (sectionName is null)
                return false;

            foreach (var relEntry in relocationTable.Entries)
            {
                var symbol = _symTab.Entries[(int)relEntry.SymbolIndex];
                var address = new Address((long)relEntry.Offset, sectionName);
                var refEntry = new ReferenceEntry(symbol.Name.Value ?? string.Empty, address, (MipsReferenceType)relEntry.Type.Value, relEntry.Addend);
                Module.TryTrackReference(refEntry);
            }

            return true;
        }
    }

    /// <inheritdoc/>
    public Module? Abstract(AssemblerConfig config)
    {
        var context = new ElfAbstractContext(_elfFile);

        foreach (var section in _elfFile.Sections)
        {
            _ = section switch
            {
                // Stream sections
                ElfStreamSection streamSection => context.AbstractStreamSection(streamSection),
                ElfSymbolTable symbolTable => context.AbstractSymbolTable(symbolTable),
                ElfRelocationTable relocationTable => context.AbstractRelocationTable(relocationTable),
                _ => false,
            };
        }

        return context.Module;
    }
}
