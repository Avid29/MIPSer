// Adam Dernis 2025

using MIPS.Assembler.Models;
using MIPS.Assembler.Models.Modules;
using MIPS.Assembler.Models.Modules.Interfaces;

namespace Raw.Modules;

/// <summary>
/// A raw wrapper format for 
/// </summary>
public class RawModule : IModule<RawModule>
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
    public static RawModule? Create(ModuleConstructor constructor, AssemblerConfig config, Stream? stream = null)
    {
        stream ??= new MemoryStream();
        
        // Append segments to stream
        constructor.ResetStreamPositions();
        foreach(var section in constructor.Sections)
            section.CopyTo(stream);

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
