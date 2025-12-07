// Avishai Dernis 2025

using CommunityToolkit.Diagnostics;
using Mipser.Models.Files;
using Mipser.Models.ProjectConfig;

namespace Mipser;

/// <summary>
/// A loaded mipser project.
/// </summary>
public partial class Project
{
    private Project(ProjectConfig config)
    {
        Guard.IsNotNull(config.RootFolderPath);

        Config = config;
        SourceFiles = new SourceCollection(this, config.RootFolderPath);
    }

    /// <summary>
    /// Gets the project's configuration.
    /// </summary>
    public ProjectConfig Config { get; }

    /// <summary>
    /// Gets the collection of source files in the project.
    /// </summary>
    public SourceCollection SourceFiles { get; }

    /// <summary>
    /// Loads a project.
    /// </summary>
    public static Project? Load(ProjectConfig config)
    {
        if (config.RootFolderPath is null)
            return null;

        return new Project(config);
    }
}
