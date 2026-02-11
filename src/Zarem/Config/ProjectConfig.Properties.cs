// Avishai Dernis 2025

using System.IO;
using System.Xml.Serialization;
using Zarem.Assembler.MIPS.Config;

namespace Zarem.Config;

public partial class ProjectConfig
{
    /// <summary>
    /// Gets or sets the project name.
    /// </summary>
    [XmlElement("ProjectName", IsNullable = false)]
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
    [XmlIgnore]
    public AssemblerConfig? AssemblerConfig
    {
        get => AssemblerConfigWrapper?.Config;
        set => AssemblerConfigWrapper?.Config = value;
    }

    /// <summary>
    /// Gets or sets the wrapper of the assembler configuration for the project.
    /// </summary>
    [XmlElement(nameof(AssemblerConfig), IsNullable = false)]
    public AssemblerConfigWrapper? AssemblerConfigWrapper { get; set; }
}
