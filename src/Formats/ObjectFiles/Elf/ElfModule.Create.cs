// Avishai Dernis 2025

using LibObjectFile.Elf;
using MIPS.Assembler.Models.Config;
using MIPS.Assembler.Models.Modules;
using ObjectFiles.Elf.Extensions;
using System.IO;

namespace ObjectFiles.Elf;

/// <summary>
/// An object module in ELF format.
/// </summary>
public partial class ElfModule
{
    /// <inheritdoc/>
    public static ElfModule? Create(Module module, AssemblerConfig config)
    {
        var elfFile = new ElfFile(ElfArch.MIPS)
        {
            new ElfSectionHeaderStringTable(),
            new ElfSectionHeaderTable()
        };

        CreateSections(module, elfFile);
        CreateSymTable(module, elfFile);

        return new ElfModule(module.Name, elfFile);
    }

    private static void CreateSections(Module module, ElfFile elfFile)
    {
        ulong pos = 0;

        foreach (var section in module.Sections.Values)
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
            elfFile.Add(elfSec);

            section.Stream.Position = 0;
            elfSec.Stream.CopyFrom(section.Stream, (int)section.Stream.Length);

            elfSec.VirtualAddress = pos;
            pos += (ulong)section.Stream.Length;
            pos += 4096 - (pos % 4096);
        }
    }

    private static void CreateSymTable(Module module, ElfFile elfFile)
    {
        var elfSymbolTable = new ElfSymbolTable();
        foreach(var symbol in module.Symbols.Values)
        {
            // TODO: Section link

            var elfSymbol = new ElfSymbol()
            {
                Name = symbol.Name,
                Value = (ulong)(symbol.Address?.Value ?? 0),
                Bind = symbol.Binding.ToElf()
            };

            elfSymbolTable.Entries.Add(elfSymbol);
        }

        elfFile.Add(elfSymbolTable);
    }
}
