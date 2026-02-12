// Adam Dernis 2024

using Zarem.Assembler.Config;

namespace Zarem.Assembler;

/// <summary>
/// An assembler.
/// </summary>
public abstract class Assembler<TConfig> : Assembler
    where TConfig : AssemblerConfig
{
    /// <summary>
    /// Initializes a new instance of the <see cref="Assembler{TConfig}"/> class.
    /// </summary>
    public Assembler(TConfig config)
    {
        Config = config;
    }

    /// <summary>
    /// Gets the assembler's configuration.
    /// </summary>
    public TConfig Config { get; }
}
