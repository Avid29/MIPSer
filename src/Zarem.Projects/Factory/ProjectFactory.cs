// Avishai Dernis 2026

using CommunityToolkit.Diagnostics;
using System;
using System.IO;
using System.Reflection;
using System.Xml;
using System.Xml.Serialization;
using Zarem.Config;

namespace Zarem.Factory;

/// <summary>
/// A class for creating <see cref="IProject"/> types.
/// </summary>
public static class ProjectFactory
{
    /// <summary>
    /// Constructs an <see cref="IProject"/> from an <see cref="ProjectConfig"/>.
    /// </summary>
    /// <param name="config"></param>
    /// <returns></returns>
    public static IProject Create(ProjectConfig config)
    {
        var typeInfo = ProjectTypeRegistry.GetProjectType(config.GetType());
        Guard.IsNotNull(typeInfo);

        // Construct project
        var project = (IProject?)Activator.CreateInstance(typeInfo.ProjectType, config);
        Guard.IsNotNull(project);
        
        return project;
    }

    /// <summary>
    /// Loads a <see cref="IProject"/> from XML.
    /// </summary>
    /// <param name="stream">The stream of the XML to load.</param>
    /// <param name="path">The path to the config file.</param>
    /// <returns>The loaded <see cref="IProject"/>.</returns>
    public static IProject Load(Stream stream, string path)
    {
        var doc = new XmlDocument();
        doc.Load(stream);

        var root = doc.DocumentElement;
        Guard.IsNotNull(root);

        var typeName = root.GetAttribute("Type");

        var typeInfo = ProjectTypeRegistry.GetProjectType(typeName);
        Guard.IsNotNull(typeInfo);

        // Deserialize config
        var serializer = new XmlSerializer(typeInfo.ConfigType, new XmlRootAttribute("Project"));
        using var reader = new XmlNodeReader(root);
        var config = (ProjectConfig?)serializer.Deserialize(reader);
        
        // Create the project
        Guard.IsNotNull(config);

        config.ConfigPath = path;
        return Create(config);
    }

    /// <summary>
    /// Stores a <see cref="ProjectConfig"/> as an XML file.
    /// </summary>
    /// <param name="config">The project config to save as a file.</param>
    /// <param name="stream">The stream to save the file to.</param>
    public static void Store(ProjectConfig config, Stream stream)
    {
        var configType = config.GetType();

        // Setup serializer of the proper type
        var typeInfo = ProjectTypeRegistry.GetProjectType(configType);
        Guard.IsNotNull(typeInfo);
        config.TypeName = typeInfo.TypeName;

        // Remove namespaces from the serializer
        var namespaces = new XmlSerializerNamespaces();
        namespaces.Add("", "");

        // TODO: This can probably be optimized

        // Serialize config
        using var tempStream = new MemoryStream();
        var xmlSerializer = new XmlSerializer(configType, new XmlRootAttribute("Project"));
        xmlSerializer.Serialize(tempStream, config, namespaces);

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
            foreach (XmlNode field in nullFields)
            {
                field.ParentNode?.RemoveChild(field);
            }
        }

        // Save the doc
        doc.Save(stream);
    }
}
