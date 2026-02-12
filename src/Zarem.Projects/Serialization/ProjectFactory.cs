// Avishai Dernis 2026

using CommunityToolkit.Diagnostics;
using System;
using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;
using Zarem.Config;

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
    /// <param name="stream">The stream of the XML to load.</param>
    /// <param name="path">The path to the config file.</param>
    /// <returns>The loaded <see cref="IProject"/>.</returns>
    public static IProject Load(Stream stream, string path)
    {
        var doc = XDocument.Load(stream);
        var root = doc.Root!;

        var typeName = root.Attribute("Type")?.Value;
        Guard.IsNotNull(typeName);

        var projectInfo = ProjectTypeRegistry.GetProjectType(typeName);
        Guard.IsNotNull(projectInfo);

        var config = (ProjectConfig?)Activator.CreateInstance(projectInfo.ConfigType);
        Guard.IsNotNull(config);

        ReadObjectProperties(root, config);

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

        var typeInfo = ProjectTypeRegistry.GetProjectType(configType);
        Guard.IsNotNull(typeInfo);

        // Create the root node
        var root = new XElement("Project", new XAttribute("Type", typeInfo.TypeName));

        // Serialize ProjectConfig properties automatically
        WriteObjectProperties(root, config);

        var doc = new XDocument(root);
        doc.Save(stream);
    }

    private static void ReadObjectProperties(XElement element, object obj)
    {
        foreach (var child in element.Elements())
        {
            var prop = obj.GetType().GetProperty(child.Name.LocalName);
            if (prop == null)
                continue;

            if (typeof(FormatConfig).IsAssignableFrom(prop.PropertyType))
            {
                var formatTypeName = child.Attribute("Type")?.Value;
                Guard.IsNotNull(formatTypeName);

                var typeInfo = ProjectTypeRegistry.GetFormatType(formatTypeName);
                Guard.IsNotNull(typeInfo);

                var formatInstance = (FormatConfig?)Activator.CreateInstance(typeInfo.ConfigType);
                Guard.IsNotNull(formatInstance);

                ReadObjectProperties(child, formatInstance);

                prop.SetValue(obj, formatInstance);
            }
            else if (IsSimple(prop.PropertyType))
            {
                var value = Convert.ChangeType(
                    child.Value,
                    prop.PropertyType);

                prop.SetValue(obj, value);
            }
            else
            {
                var nestedInstance = Activator.CreateInstance(prop.PropertyType)!;

                ReadObjectProperties(child, nestedInstance);
                prop.SetValue(obj, nestedInstance);
            }
        }
    }

    private static void WriteObjectProperties(XElement parent, object obj)
    {
        foreach (var prop in obj.GetType().GetProperties())
        {
            var value = prop.GetValue(obj);
            if (value == null)
                continue;

            if (value is FormatConfig format)
            {
                var typeInfo = ProjectTypeRegistry.GetFormatType(format.GetType());
                Guard.IsNotNull(typeInfo);

                var formatElement = new XElement("FormatConfig", new XAttribute("Type", typeInfo.TypeName));

                WriteObjectProperties(formatElement, format);
                parent.Add(formatElement);
            }
            else if (IsSimple(prop.PropertyType))
            {
                parent.Add(new XElement(prop.Name, value));
            }
            else
            {
                var element = new XElement(prop.Name);
                WriteObjectProperties(element, value);
                parent.Add(element);
            }
        }
    }

    private static bool IsSimple(Type type)
    {
        return type.IsPrimitive
            || type == typeof(string)
            || type == typeof(decimal)
            || type == typeof(DateTime);
    }
}
