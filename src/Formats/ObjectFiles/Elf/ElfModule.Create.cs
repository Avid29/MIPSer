// Avishai Dernis 2025

using CommunityToolkit.Diagnostics;
using LibObjectFile.Elf;
using MIPS.Assembler.Models.Config;
using MIPS.Assembler.Models.Modules;
using ObjectFiles.Elf.Extensions;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace ObjectFiles.Elf;

/// <summary>
/// An object module in ELF format.
/// </summary>
public partial class ElfModule
{
    private ref struct ElfBuildContext
    {
        private ElfSymbolTable? _symtab;

        public ElfBuildContext(Module module)
        {
            Module = module;
            ElfFile = new ElfFile(ElfArch.MIPS)
            {
                new ElfSectionHeaderStringTable(),
                new ElfSectionHeaderTable()
            };
        }

        public Module Module { get; }

        public ElfFile ElfFile { get; }

        public void CreateSections()
        {
            ulong pos = 0;

            foreach (var section in Module.Sections.Values)
            {
                var type = section.Name switch
                {
                    ".text" => ElfSectionSpecialType.Text,
                    ".data" => ElfSectionSpecialType.Data,
                    ".rodata" => ElfSectionSpecialType.ReadOnlyData,
                    ".sdata" => ElfSectionSpecialType.Data,
                    ".bss" => ElfSectionSpecialType.Bss,
                    ".sbss" => ElfSectionSpecialType.Bss,
                    _ => ElfSectionSpecialType.None,
                };

                var elfSec = new ElfStreamSection(type);
                ElfFile.Add(elfSec);

                section.Stream.Position = 0;
                elfSec.Stream.CopyFrom(section.Stream, (int)section.Stream.Length);

                elfSec.VirtualAddress = pos;
                pos += (ulong)section.Stream.Length;
                pos += 4096 - (pos % 4096);
            }
        }

        public void CreateSymTable()
        {
            _symtab = new ElfSymbolTable();
            foreach (var symbol in Module.Symbols.Values)
            {
                var sectionName = symbol.Address?.Section;
                ElfSectionLink link = default;
                if (sectionName is not null)
                {
                    var section = ElfFile.Sections.FirstOrDefault(x => x.Name == sectionName);
                    link = new ElfSectionLink(section);
                }

                var elfSymbol = new ElfSymbol()
                {
                    Name = symbol.Name,
                    Value = (ulong)(symbol.Address?.Value ?? 0),
                    Bind = symbol.Binding.ToElf(),
                    SectionLink = link,
                };

                _symtab.Entries.Add(elfSymbol);
            }

            ElfFile.Add(_symtab);
        }

        public void CreateRelTables()
        {
            Guard.IsNotNull(_symtab);

            Dictionary<(string, bool), ElfRelocationTable> relTables = new();

            foreach (var @ref in Module.References)
            {
                Guard.IsNotNull(@ref.Location.Section);

                var isRela = @ref.Append is not 0;

                if (!relTables.TryGetValue((@ref.Location.Section, isRela), out var table))
                {
                    table = new ElfRelocationTable(isRela);
                    relTables[(@ref.Location.Section, isRela)] = table;
                }

                var offset = @ref.Location.Value;
                var symbolIndex = _symtab.Entries.FindIndex(x => x.Name.Value == @ref.Symbol);
                Guard.IsNotEqualTo(symbolIndex, -1);

                var type = new ElfRelocationType(ElfArchEx.MIPS, (uint)@ref.Type);
                var relItem = new ElfRelocation((ulong)@ref.Location.Value, type, (uint)symbolIndex, @ref.Append);

                table.Entries.Add(relItem);
            }

            foreach (var ((section, isRela), table) in relTables)
            {
                table.Name = isRela ? $".rela{section}" : $".rel{section}";
                table.Info = new ElfSectionLink(ElfFile.Sections.First(x => x.Name == section));
                ElfFile.Add(table);
            }
        }
    }

    /// <inheritdoc/>
    public static ElfModule? Create(Module module, AssemblerConfig config)
    {
        var context = new ElfBuildContext(module);

        context.CreateSections();
        context.CreateSymTable();
        context.CreateRelTables();

        return new ElfModule(context.Module.Name, context.ElfFile);
    }
}
