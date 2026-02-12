// Avishai Dernis 2025

using CommunityToolkit.Diagnostics;
using Zarem.Assembler;
using Zarem.Assembler.Config;
using Zarem.Config;
using Zarem.Emulator;
using Zarem.Emulator.Config;
using Zarem.Models.Files;

namespace Zarem;

/// <summary>
/// A loaded mipser project.
/// </summary>
public abstract partial class Project<TAssembler, TEmulator, TAsmConfig, TEmuConfig> : IProject
    where TAssembler : IAssembler<TAsmConfig>
    where TEmulator : Emulator<TEmuConfig>
    where TAsmConfig : AssemblerConfig
    where TEmuConfig : EmulatorConfig
{
    /// <summary>
    /// Initialzes a new instance of the <see cref="ProjectConfig{TAsmConfig, TEmuConfig}"/> class.
    /// </summary>
    /// <param name="config"></param>
    protected Project(ProjectConfig<TAsmConfig, TEmuConfig> config)
    {
        Guard.IsNotNull(config.RootFolderPath);

        Config = config;
        SourceFiles = new SourceCollection(this, config.RootFolderPath);
    }

    /// <summary>
    /// Gets the project's configuration.
    /// </summary>
    public ProjectConfig<TAsmConfig, TEmuConfig> Config { get; internal set; }

    /// <summary>
    /// Gets the collection of source files in the project.
    /// </summary>
    public SourceCollection SourceFiles { get; }

    /// <inheritdoc/>
    ProjectConfig IProject.Config => Config;
}
