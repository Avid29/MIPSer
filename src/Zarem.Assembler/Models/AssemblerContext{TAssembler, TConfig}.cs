// Adam Dernis 2024

using Zarem.Assembler.Config;
using Zarem.Models.Addressing;
using Zarem.Models.Modules;

namespace Zarem.Assembler.Models;

/// <summary>
/// This class provides an interface for assembler context to the parsers.
/// </summary>
/// <remarks>
/// The assembler is structured so the parsers create as densely represented objects as possible,
/// however these objects are not appended to the module until being passed back to the assembler.
/// This class provides readonly access to a handful of contextual information the parsers depend on.
/// </remarks>
public abstract class AssemblerContext<TAssembler, TConfig> : AssemblerContext
    where TAssembler : Assembler<TConfig>
    where TConfig : AssemblerConfig
{
    /// <summary>
    /// Initializes a new instance of the <see cref="AssemblerContext"/> class.
    /// </summary>
    protected AssemblerContext(TAssembler assembler, Module module) : base(module)
    {
        Assembler = assembler;
    }

    /// <remarks>
    /// This is for testing purposes. Don't adjust nullability arround ensuring it works.
    /// </remarks>
    protected AssemblerContext(Module module) : base(module)
    {
        Assembler = null!;
    }

    /// <summary>
    /// Gets the assembler.
    /// </summary>
    protected TAssembler Assembler { get; }

    /// <inheritdoc cref="Assembler{TConfig}.Config"/>
    public TConfig Config => Assembler.Config;

    /// <inheritdoc cref="Assembler.CurrentAddress"/>
    public override Address CurrentAddress => Assembler?.CurrentAddress ?? default;
}
