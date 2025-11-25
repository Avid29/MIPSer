// Adam Dernis 2025

using CommunityToolkit.Diagnostics;
using ELF.Modules.Config;
using ELF.Modules.Models.Headers;
using ELF.Modules.Models.Headers.Enums;
using MIPS.Assembler.Models;
using MIPS.Assembler.Models.Modules;
using MIPS.Assembler.Models.Modules.Interfaces;

namespace ELF.Modules;

/// <summary>
/// A fully assembled object module in ELF format
/// </summary>
public class ElfModule : IBuildModule<ElfModule, ElfConfig>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ElfModule"/> class.
    /// </summary>
    public ElfModule(string name)
    {
        Name = name;
    }

    /// <inheritdoc/>
    public string? Name { get; }

    /// <inheritdoc/>
    public unsafe static ElfModule? Create(Module constructor, ElfConfig config, Stream? stream = null)
    {
        stream ??= new MemoryStream();

        // Ensure 32-bit class
        // TODO: Support creating 64-bit elf.
        if (config.ElfIdentity.Class is not Class.Bit32)
        {
            ThrowHelper.ThrowArgumentException(nameof(config.ElfIdentity.Class), $"Configuration class must be 32bit for MIPS.");
        }

        // Allocate space for header
        var header = new Header<uint>()
        {
            Identity = config.ElfIdentity,
            HeaderSize = (ushort)sizeof(Header<uint>),
        };
        header.TryWrite(stream);

        // Allocate space for program header
        var progHeader = new ProgramHeader32()
        {
            
        };
        progHeader.TryWrite(stream);

        // Append segments to stream
        constructor.ResetStreamPositions();
        foreach (var section in constructor.Sections)
            section.Stream.CopyTo(stream);

        throw new NotImplementedException();
    }

    /// <inheritdoc/>
    public static ElfModule? Load(string name, Stream stream)
    {
        throw new NotImplementedException();
    }
    
    /// <inheritdoc/>
    public Module? Abstract(AssemblerConfig config)
    {
        throw new NotImplementedException();
    }
}
