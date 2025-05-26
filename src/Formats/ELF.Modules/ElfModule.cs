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
public class ElfModule : IModule<ElfModule>
{
    /// <inheritdoc/>
    public unsafe static ElfModule? Create(ModuleConstructor constructor, AssemblerConfig config, Stream? stream = null)
    {
        stream ??= new MemoryStream();

        // Verify 
        if (config is not ElfConfig elfConfig)
        {
            ThrowHelper.ThrowArgumentException(nameof(config), $"{config} must be an {nameof(ElfConfig)}.");
            return null;
        }

        // Ensure 32-bit class
        // TODO: Support creating 64-bit elf.
        if (elfConfig.ElfIdentity.Class is not Class.Bit32)
        {
            ThrowHelper.ThrowArgumentException(nameof(elfConfig.ElfIdentity.Class), $"Configuration class must be 32bit for MIPS.");
        }

        // Allocate space for header
        var header = new Header<uint>()
        {
            Identity = elfConfig.ElfIdentity,
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
            section.CopyTo(stream);

        throw new NotImplementedException();
    }
    
    /// <inheritdoc/>
    public static ElfModule? Load(Stream stream)
    {
        throw new NotImplementedException();
    }
    
    /// <inheritdoc/>
    public ModuleConstructor? Abstract(AssemblerConfig config)
    {
        throw new NotImplementedException();
    }
}
