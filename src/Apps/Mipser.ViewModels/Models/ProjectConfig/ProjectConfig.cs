// Avishai Dernis 2025

using MIPS.Assembler.Models.Config;
using System;
using System.Xml.Serialization;

namespace Mipser.Models.ProjectConfig;

/// <summary>
/// A model for project configurations.
/// </summary>
[XmlRoot("Project")]
public class ProjectConfig
{
    /// <summary>
    /// Gets or sets the project name.
    /// </summary>
    [XmlElement]
    public required string Name { get; set; }

    /// <summary>
    /// Gets or sets the path where the project is found.
    /// </summary>
    [XmlIgnore]
    public Uri? Path { get; set; }

    /// <summary>
    /// Gets or sets the assembler configuration for the project.
    /// </summary>
    [XmlElement]
    public required AssemblerConfig AssemblerConfig { get; set; }
}
