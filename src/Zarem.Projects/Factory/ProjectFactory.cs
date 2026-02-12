// Avishai Dernis 2026

using CommunityToolkit.Diagnostics;
using System;
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
        var project = (IProject)Activator.CreateInstance(
            typeInfo.ProjectType,
            config)!;

        return project;
    }

    /// <summary>
    /// Loads a <see cref="IProject"/> from XML.
    /// </summary>
    /// <param name="path">The path of the file to load.</param>
    /// <returns>The loaded <see cref="IProject"/>.</returns>
    public static IProject Load(string path)
    {
        var doc = new XmlDocument();
        doc.Load(path);

        var root = doc.DocumentElement!;
        var typeName = root.GetAttribute("Type");

        var typeInfo = ProjectTypeRegistry.GetProjectType(typeName);
        Guard.IsNotNull(typeInfo);

        // Deserialize config
        var serializer = new XmlSerializer(typeInfo.ConfigType);

        ProjectConfig config;
        using var reader = new XmlNodeReader(root);
        config = (ProjectConfig)serializer.Deserialize(reader)!;
        return Create(config);
    }

    /// <summary>
    /// Stores a <see cref="ProjectConfig"/> as an XML file.
    /// </summary>
    /// <param name="config">The project config to save as a file.</param>
    /// <param name="path">The path of the file to load.</param>
    /// <returns>The loaded <see cref="IProject"/>.</returns>
    public static void Store(ProjectConfig config, string path)
    {
        var configType = config.GetType();

        // Resolve type name from registry
        var typeInfo = ProjectTypeRegistry.GetProjectType(configType);
        Guard.IsNotNull(typeInfo);

        var serializer = new XmlSerializer(configType);

        var settings = new XmlWriterSettings
        {
            Indent = true
        };

        using var writer = XmlWriter.Create(path, settings);

        writer.WriteStartDocument();

        // Start root element
        writer.WriteStartElement("Project");

        // Write Type attribute
        writer.WriteAttributeString("Type", typeInfo.TypeName);

        // Serialize config content inside the Project element
        var namespaces = new XmlSerializerNamespaces();
        namespaces.Add("", ""); // remove xsi/xsd noise

        serializer.Serialize(writer, config, namespaces);

        writer.WriteEndElement(); // Project
        writer.WriteEndDocument();
    }
}
