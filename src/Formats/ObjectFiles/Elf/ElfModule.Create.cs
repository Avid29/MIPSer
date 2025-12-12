// Avishai Dernis 2025

using LibObjectFile.Elf;
using MIPS.Assembler.Models.Config;
using MIPS.Assembler.Models.Modules;
using MIPS.Models.Modules.Tables.Enums;
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
        }
    }

    private static void CreateSymTable(Module module, ElfFile elfFile)
    {
        var elfSymbolTable = new ElfSymbolTable();
        foreach(var symbol in module.Symbols.Values)
        {
            var elfSymbol = new ElfSymbol()
            {
                Name = symbol.Name,
                Value = (ulong)(symbol.Address?.Value ?? 0),
                Bind = symbol.Binding switch
                {
                    SymbolBinding.Global => ElfSymbolBind.Global,
                    SymbolBinding.Weak => ElfSymbolBind.Weak,
                    SymbolBinding.Local or _ => ElfSymbolBind.Local,
                },
            };

            elfSymbolTable.Entries.Add(elfSymbol);
        }

        elfFile.Add(elfSymbolTable);
    }
}
