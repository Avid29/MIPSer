// Avishai Dernis 2025

using LibObjectFile.Elf;
using MIPS.Assembler.Models.Config;
using MIPS.Assembler.Models.Modules;
using System.IO;

namespace ObjectFiles.Elf;

/// <summary>
/// An object module in ELF format.
/// </summary>
public partial class ElfModule
{
    /// <inheritdoc/>
    public static ElfModule? Create(Module constructor, AssemblerConfig config, Stream? stream = null)
    {
        stream ??= new MemoryStream();

        var file = new ElfFile(ElfArch.MIPS);

        foreach (var section in constructor.Sections.Values)
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
            file.Add(elfSec);

            section.Stream.Position = 0;
            elfSec.Stream.CopyFrom(section.Stream, (int)section.Stream.Length);
        }

        file.Write(stream);
        return Open(constructor.Name, stream);
    }

}
