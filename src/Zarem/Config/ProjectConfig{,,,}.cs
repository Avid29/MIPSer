// Avishai Dernis 2025

using System.IO;
using System.Xml.Serialization;
using Zarem.Assembler.Config;
using Zarem.Emulator.Config;

namespace Zarem.Config;

/// <summary>
/// A model for project configurations.
/// </summary>
[XmlRoot("Project")]
public abstract partial class ProjectConfig<TAsmConfig, TEmuConfig> : IProjectConfig
    where TAsmConfig : AssemblerConfig
    where TEmuConfig : EmulatorConfig
{
    /// <summary>
    /// Initializes a new instance of the <see cref="IProjectConfig"/> class.
    /// </summary>
    public ProjectConfig()
    {

    }

    /// <inheritdoc/>
    public string? Name { get; init; }

    /// <inheritdoc/>
    [XmlIgnore]
    public string? ConfigPath { get; set; }

    /// <inheritdoc/>
    [XmlIgnore]
    public string? RootFolderPath => Path.GetDirectoryName(ConfigPath);

    /// <inheritdoc/>
    public TAsmConfig? AssemblerConfig { get; init; }

    /// <inheritdoc/>
    public TEmuConfig? EmulatorConfig { get; init; }

    /// <inheritdoc/>
    public FormatConfig? FormatConfig { get; init; }

    AssemblerConfig? IProjectConfig.AssemblerConfig => AssemblerConfig;

    EmulatorConfig? IProjectConfig.EmulatorConfig => EmulatorConfig;
}
