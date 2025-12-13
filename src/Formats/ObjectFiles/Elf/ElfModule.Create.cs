// Avishai Dernis 2025

using LibObjectFile.Ar;
using LibObjectFile.Elf;
using MIPS.Assembler.Models.Config;
using MIPS.Assembler.Models.Modules;
using ObjectFiles.Elf.Extensions;
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
            var elfSymbolTable = new ElfSymbolTable();
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

                elfSymbolTable.Entries.Add(elfSymbol);
            }

            ElfFile.Add(elfSymbolTable);
        }
    }

    /// <inheritdoc/>
    public static ElfModule? Create(Module module, AssemblerConfig config)
    {

        var context = new ElfBuildContext(module);


        context.CreateSections();
        context.CreateSymTable();

        return new ElfModule(context.Module.Name, context.ElfFile);
    }
}
