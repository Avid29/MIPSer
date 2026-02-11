// Avishai Dernis 2025

using System.IO;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;

namespace Zarem.Config;

/// <summary>
/// A model for project configurations.
/// </summary>
[XmlRoot("Project")]
public partial class ProjectConfig
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ProjectConfig"/> class.
    /// </summary>
    public ProjectConfig()
    {
    }

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
