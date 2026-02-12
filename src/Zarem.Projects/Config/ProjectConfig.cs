// Avishai Dernis 2025

using System.IO;
using System.Xml.Serialization;

namespace Zarem.Config;

/// <summary>
/// A model for project configurations.
/// </summary>
[XmlRoot("Project")]
public abstract partial class ProjectConfig
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ProjectConfig"/> class.
    /// </summary>
    public ProjectConfig()
    {
    }
    
    /// <summary>
    /// Gets or sets the project name.
    /// </summary>
    public string? Name { get; init; }

    /// <summary>
    /// Gets or sets the project type name.
    /// </summary>
    [XmlAttribute("Type")]
    public string TypeName { get; set; } = "";

    /// <summary>
    /// Gets or sets the path for the config file.
    /// </summary>
    [XmlIgnore]
    public string? ConfigPath { get; set; }

    /// <summary>
    /// Gets the path root folder path.
    /// </summary>
    [XmlIgnore]
    public string? RootFolderPath => Path.GetDirectoryName(ConfigPath);

    /// <summary>
    /// Gets or sets the format configuration for the project.
    /// </summary>
    public FormatConfig? FormatConfig { get; set; }
}
