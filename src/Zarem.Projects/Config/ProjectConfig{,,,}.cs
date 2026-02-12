// Avishai Dernis 2025

using Zarem.Assembler.Config;
using Zarem.Emulator.Config;

namespace Zarem.Config;

/// <summary>
/// A model for project configurations.
/// </summary>
public abstract partial class ProjectConfig<TAsmConfig, TEmuConfig> : ProjectConfig
    where TAsmConfig : AssemblerConfig
    where TEmuConfig : EmulatorConfig
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ProjectConfig"/> class.
    /// </summary>
    public ProjectConfig()
    {

    }

    /// <summary>
    /// Gets the assembler configuration info.
    /// </summary>
    public TAsmConfig? AssemblerConfig { get; init; }

    /// <summary>
    /// Gets the emulator configuration info.
    /// </summary>
    public TEmuConfig? EmulatorConfig { get; init; }
}
