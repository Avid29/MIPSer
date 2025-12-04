// Avishai Dernis 2025

using MIPS.Assembler.Models.Config;
using Mipser.Services.Files.Models;
using System;
using System.IO;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;

namespace Mipser.Models.ProjectConfig;

/// <summary>
/// A model for project configurations.
/// </summary>
[XmlRoot("Project")]
public class ProjectConfig
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
    [XmlElement(IsNullable = false)]
    public AssemblerConfig? AssemblerConfig { get; set; }

    /// <summary>
    /// Serializes the <see cref="ProjectConfig"/>
    /// </summary>
    public async Task SerializeAsync(Stream stream)
    {
        // Remove namespaces from serializer
        var namespaces = new XmlSerializerNamespaces();
        namespaces.Add(string.Empty, string.Empty);

        // TODO: This can probably be optimized

        // Serialize to temp stream
        using var tempStream = new MemoryStream();
        var serializer = new XmlSerializer(typeof(ProjectConfig));
        serializer.Serialize(tempStream, this, namespaces);

        // Load the doc to modify
        var doc = new XmlDocument();
        tempStream.Position = 0;
        doc.Load(tempStream);

        // Remove null fields from their parent
        var mgr = new XmlNamespaceManager(doc.NameTable);
        mgr.AddNamespace("xsi", "http://www.w3.org/2001/XMLSchema-instance");
        var nullFields = doc.SelectNodes("//*[@xsi:nil='true']", mgr);
        if (nullFields is not null)
        {
            foreach(XmlNode field in nullFields)
            {
                field.ParentNode?.RemoveChild(field);
            }
        }

        // Save the doc
        doc.Save(stream);
    }

    /// <summary>
    /// Deserializes a <see cref="ProjectConfig"/>
    /// </summary>
    public static async Task<ProjectConfig?> DeserializeAsync(string path, Stream stream)
    {
        var serializer = new XmlSerializer(typeof(ProjectConfig));
        var result = (ProjectConfig?)serializer.Deserialize(stream);
        if (result is null)
            return null;

        result.ConfigPath = path;
        return result;
    }
}
