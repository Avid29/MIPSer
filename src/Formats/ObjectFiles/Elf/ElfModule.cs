// Avishai Dernis 2025

using LibObjectFile.Elf;
using MIPS.Assembler.Models.Config;
using MIPS.Assembler.Models.Modules;
using MIPS.Assembler.Models.Modules.Interfaces;
using MIPS.Interpreter.Models.Modules;
using System.IO;
using System.Linq;

namespace ObjectFiles.Elf;

/// <summary>
/// An object module in ELF format.
/// </summary>
public partial class ElfModule : IBuildModule<ElfModule>, IExecutableModule
{
    private readonly ElfFile _elfFile;

    private ElfModule(string? name,  ElfFile elfFile)
    {
        Name = name;
        _elfFile = elfFile;
    }
    
    /// <inheritdoc/>
    public string? Name { get; }

    /// <inheritdoc/>
    public uint EntryAddress => (uint)_elfFile.EntryPointAddress;

    /// <inheritdoc/>
    public static ElfModule? Open(string? name, Stream stream)
    {
        var elfFile = ElfFile.Read(stream);
        return new ElfModule(name, elfFile);
    }

    /// <inheritdoc/>
    public Module? Abstract(AssemblerConfig config)
    {
        throw new System.NotImplementedException();
    }

    /// <inheritdoc/>
    public void Load(Stream destination)
    {
        // TODO: Load the module into the stream.
        foreach (var section in _elfFile.Sections.OfType<ElfStreamSection>())
        {
            if (!section.Flags.HasFlag(ElfSectionFlags.Alloc))
                continue;

            destination.Position = (long)section.VirtualAddress;
            section.Stream.Position = 0;
            section.Stream.CopyTo(destination);
        }
    }

    /// <inheritdoc/>
    public void Save(Stream stream)
    {
        _elfFile.Write(stream);
    }
}
