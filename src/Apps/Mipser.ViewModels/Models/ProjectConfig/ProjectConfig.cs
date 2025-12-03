// Avishai Dernis 2025

using System.Xml.Serialization;

namespace Mipser.Models.ProjectConfig;

/// <summary>
/// A model for project configurations.
/// </summary>
public class ProjectConfig
{
    /// <summary>
    /// Gets or sets the assembler configuration for the project.
    /// </summary>
    [XmlElement("assembler_config")]
    public required AssemblerConfig AssemblerConfig { get; set; }
}
