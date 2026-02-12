// Avishai Dernis 2026

using CommunityToolkit.Diagnostics;
using System;
using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;
using Zarem.Config;
using Zarem.Serialization.Registry;

namespace Zarem.Serialization;

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
        Guard.IsNotNull(config.FormatConfig);

        var typeInfo = ProjectTypeRegistry.GetProjectType(config.GetType());
        Guard.IsNotNull(typeInfo);

        var formatTypeInfo = ProjectTypeRegistry.GetFormatType(config.FormatConfig.GetType());
        Guard.IsNotNull(formatTypeInfo);

        // Construct closed project type
        var openProjectType = typeInfo.ProjectType;
        var moduleType = formatTypeInfo.FormatType;
        var modConfigType = formatTypeInfo.ConfigType;
        var closedProjectType = openProjectType.MakeGenericType(moduleType, modConfigType);

        // TODO: Get Module config to the module

        // Construct project
        var project = (IProject?)Activator.CreateInstance(closedProjectType, config);
        Guard.IsNotNull(project);
        
        return project;
    }

    /// <summary>
    /// Loads a <see cref="IProject"/> from XML.
    /// </summary>
    /// <param name="path">The path to the config file.</param>
    /// <returns>The loaded <see cref="IProject"/>.</returns>
    public static IProject Load(string path)
    {
        var doc = XDocument.Load(path);
        var root = doc.Root!;

        var typeName = root.Attribute("Type")?.Value;
        Guard.IsNotNull(typeName);

        var projectInfo = ProjectTypeRegistry.GetProjectType(typeName);
        Guard.IsNotNull(projectInfo);

        var config = (ProjectConfig?)Activator.CreateInstance(projectInfo.ConfigType);
        Guard.IsNotNull(config);

        ProjectSerializer.ReadObjectProperties(root, config);

        config.ConfigPath = path;
        return Create(config);
    }

    /// <summary>
    /// Stores a <see cref="ProjectConfig"/> as an XML file.
    /// </summary>
    /// <param name="config">The project config to save as a file.</param>
    /// <param name="path">The file path to save the file to.</param>
    public static void Store(ProjectConfig config, string path)
    {
        var configType = config.GetType();

        var typeInfo = ProjectTypeRegistry.GetProjectType(configType);
        Guard.IsNotNull(typeInfo);

        // Create the root node
        var root = new XElement("Project", new XAttribute("Type", typeInfo.TypeName));

        // Serialize ProjectConfig properties automatically
        ProjectSerializer.WriteObjectProperties(root, config);

        var doc = new XDocument(root);
        doc.Save(path);
    }
}
