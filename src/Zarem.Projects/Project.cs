// Avishai Dernis 2025

using CommunityToolkit.Diagnostics;
using Zarem.Components.Interfaces;
using Zarem.Config;
using Zarem.Models.Files;
using Zarem.Serialization;

namespace Zarem;

/// <summary>
/// A loaded mipser project.
/// </summary>
public partial class Project : IProject
{
    /// <summary>
    /// Initialzes a new instance of the <see cref="ProjectConfig{TAsmConfig, TEmuConfig}"/> class.
    /// </summary>
    internal Project(IProjectConfig config, IAssembleComponent assemble, IFormatComponent format)
    {
        Guard.IsNotNull(config.RootFolderPath);

        Config = config;
        Assemble = assemble;
        Format = format;
        SourceFiles = new SourceCollection(this, config.RootFolderPath);
    }

    /// <summary>
    /// Gets the project's configuration.
    /// </summary>
    public IProjectConfig Config { get; internal set; }

    /// <summary>
    /// Gets the collection of source files in the project.
    /// </summary>
    public SourceCollection SourceFiles { get; }

    /// <summary>
    /// Gets the project's assemble component.
    /// </summary>
    public IAssembleComponent Assemble { get; }

    /// <summary>
    /// Gets the project's format component.
    /// </summary>
    public IFormatComponent Format { get; }

    /// <inheritdoc/>
    IProjectConfig IProject.Config => Config;

    /// <inheritdoc/>
    public void Save()
    {
        Guard.IsNotNull(Config.ConfigPath);

        ProjectSerializer.Serialize(Config, Config.ConfigPath);
    }
}
