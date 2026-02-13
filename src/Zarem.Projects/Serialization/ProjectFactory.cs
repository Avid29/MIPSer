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
        var config = ProjectSerializer.Deserialize(path);

        return Create(config);
    }
}
