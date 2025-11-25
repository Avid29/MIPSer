// Avishai Dernis 2025

using LibObjectFile.Elf;
using MIPS.Assembler.Models;
using MIPS.Assembler.Models.Modules;
using MIPS.Assembler.Models.Modules.Interfaces;
using MIPS.Interpreter.Models.Modules;
using MIPS.Models.Addressing.Enums;
using System.IO;

namespace ObjectFiles;

/// <summary>
/// An object module in ELF format.
/// </summary>
public class ElfModule : IBuildModule<ElfModule, AssemblerConfig>, IExecutableModule
{
    private readonly ElfFile _elfFile;

    private ElfModule(ElfFile elfFile)
    {
        _elfFile = elfFile;
    }
    
    /// <inheritdoc/>
    public string Name => throw new System.NotImplementedException();

    /// <inheritdoc/>
    public Stream Contents => throw new System.NotImplementedException();

    /// <inheritdoc/>
    public uint EntryAddress => (uint)_elfFile.EntryPointAddress;

    /// <inheritdoc/>
    public static ElfModule? Create(Module constructor, AssemblerConfig config, Stream? stream = null)
    {
        stream ??= new MemoryStream();

        var file = new ElfFile(ElfArch.MIPS);

        foreach (var section in constructor.Sections)
        {
            var type = section.Section switch
            {
                Section.Text => ElfSectionSpecialType.Text,
                Section.Data => ElfSectionSpecialType.Data,
                Section.ReadOnlyData => ElfSectionSpecialType.ReadOnlyData,
                Section.SmallInitializedData => ElfSectionSpecialType.ReadOnlyData,
                Section.SmallUninitializedData => ElfSectionSpecialType.Bss,
                Section.UninitializedData => ElfSectionSpecialType.Bss,
                _ => ElfSectionSpecialType.None,
            };

            var elfSec = new ElfStreamSection(type);
            file.Add(elfSec);

            section.Stream.Position = 0;
            elfSec.Stream.CopyFrom(section.Stream, (int)section.Stream.Length);
        }

        file.Write(stream);
        return Load(constructor.Name, stream);
    }

    /// <inheritdoc/>
    public static ElfModule? Load(string? name, Stream stream)
    {
        var elfFile = ElfFile.Read(stream);
        return new ElfModule(elfFile);
    }

    /// <inheritdoc/>
    public Module? Abstract(AssemblerConfig config)
    {
        throw new System.NotImplementedException();
    }
}
