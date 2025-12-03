// Avishai Dernis 2025

using MIPS.Models.Instructions.Enums;
using System.Xml.Serialization;

namespace Mipser.Models.ProjectConfig;

/// <summary>
/// A model for assembler configurations.
/// </summary>
public class AssemblerConfig
{
    /// <summary>
    /// Gets or sets the version of mips to use.
    /// </summary>
    [XmlElement("mips_version")]
    public required MipsVersion MipsVersion { get; set; }
}
