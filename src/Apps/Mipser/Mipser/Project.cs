// Avishai Dernis 2025

using Mipser.Models;
using Mipser.Models.ProjectConfig;
using System.Collections.Generic;
using System.IO;

namespace Mipser;

/// <summary>
/// A loaded mipser project.
/// </summary>
public partial class Project
{
    private Project(ProjectConfig config)
    {
        Config = config;
    }

    /// <summary>
    /// Gets the project's configuration.
    /// </summary>
    public ProjectConfig Config { get; }

    /// <summary>
    /// Gets the collection of source files in the project.
    /// </summary>
    public SourceCollection? SourceFiles { get; private set; }

    /// <summary>
    /// Loads a project.
    /// </summary>
    public static Project? Load(ProjectConfig config)
    {
        // Initialize project object
        var project = new Project(config);

        if (config.RootFolderPath is null)
            return null;

        project.SourceFiles = new SourceCollection(config.RootFolderPath);

        return project;
    }
}
