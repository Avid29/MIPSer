// Avishai Dernis 2025

using System.IO;
using System.Xml.Serialization;
using Zarem.Assembler.Config;

namespace Zarem.Config;

/// <summary>
/// A base class for a project configuration type
/// </summary>
public partial class ProjectConfig
{
    /// <summary>
    /// Gets or sets the project name.
    /// </summary>
    public string? Name { get; set; }

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
    /// Gets or sets the assembler configuration for the project.
    /// </summary>
    public MIPSAssemblerConfig? AssemblerConfig { get; set; }

    /// <summary>
    /// Gets or sets the format configuration for the project.
    /// </summary>
    public FormatConfig? FormatConfig { get; set; }
}
