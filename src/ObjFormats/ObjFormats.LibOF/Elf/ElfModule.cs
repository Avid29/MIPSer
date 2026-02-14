// Avishai Dernis 2025

using LibObjectFile.Elf;
using ObjFormats.LibOF.Elf.Config;
using System.IO;
using System.Threading.Tasks;
using Zarem.Assembler.Models.Modules;
using Zarem.Attributes;
using Zarem.Emulator.Models.Modules;

namespace ObjectFiles.Elf;

/// <summary>
/// An object module in ELF format.
/// </summary>
[FormatType("ELF", typeof(ElfConfig))]
public partial class ElfModule : IBuildModule<ElfModule, ElfConfig>, IExecutableModule
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
    public async Task SaveAsync(Stream stream)
    {
        _elfFile.Write(stream);
    }
}
