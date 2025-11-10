// Adam Dernis 2025

using MIPS.Assembler.Models;
using MIPS.Assembler.Models.Modules;
using MIPS.Assembler.Models.Modules.Interfaces;
using MIPS.Interpreter.Models.Modules;

namespace Raw.Modules;

/// <summary>
/// A raw wrapper format for a module that contains the raw binary data of the assembled program.
/// </summary>
public class RawModule : IBuildModule<RawModule>, IExecutableModule
{
    private readonly Stream _source;
    
    /// <summary>
    /// Initializes a new instance of the <see cref="RawModule"/> class.
    /// </summary>
    public RawModule(Stream source)
    {
        _source = source;
    }
    
    /// <inheritdoc/>
    public Stream Contents => _source;
    
    /// <inheritdoc/>
    public uint EntryAddress => 0;

    /// <inheritdoc/>
    public static RawModule? Create(ModuleConstructor constructor, AssemblerConfig config, Stream? stream = null)
    {
        stream ??= new MemoryStream();
        
        // Append segments to stream
        constructor.ResetStreamPositions();
        foreach(var section in constructor.Sections)
            section.Stream.CopyTo(stream);

        return Load(stream);
    }
    
    /// <inheritdoc/>
    public static RawModule? Load(Stream stream)
    {
        return new RawModule(stream);
    }
    
    /// <inheritdoc/>
    public ModuleConstructor? Abstract(AssemblerConfig config)
    {
        throw new NotImplementedException();
    }
}
