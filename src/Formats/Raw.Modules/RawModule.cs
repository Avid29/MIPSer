// Adam Dernis 2025

using MIPS.Assembler.Models.Config;
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
    public RawModule(string? name, Stream source)
    {
        Name = name;
        _source = source;
    }
    
    /// <inheritdoc/>
    public Stream Contents => _source;
    
    /// <inheritdoc/>
    public uint EntryAddress => 0;
    
    /// <inheritdoc/>
    public string? Name { get; }

    /// <inheritdoc/>
    public static RawModule? Create(Module constructor, AssemblerConfig config, Stream? stream = null)
    {
        stream ??= new MemoryStream();
        
        // Append segments to stream
        constructor.ResetStreamPositions();
        foreach(var section in constructor.Sections.Values)
            section.Stream.CopyTo(stream);

        return Load(constructor.Name, stream);
    }
    
    /// <inheritdoc/>
    public static RawModule? Load(string? name, Stream stream)
    {
        return new RawModule(name, stream);
    }
    
    /// <inheritdoc/>
    public Module? Abstract(AssemblerConfig config)
    {
        throw new NotImplementedException();
    }
}
