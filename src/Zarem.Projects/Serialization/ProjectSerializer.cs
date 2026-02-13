// Avishai Dernis 2026

using CommunityToolkit.Diagnostics;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using System.Xml.Linq;
using System.Xml.Serialization;
using Zarem.Config;
using Zarem.Serialization.Registry;

namespace Zarem.Serialization;

/// <summary>
/// A class for serializing/deserialization <see cref="ProjectConfig"/> instances.
/// </summary>
public static partial class ProjectSerializer
{
    delegate void SerializeDelegate(XElement parent, PropertyInfo prop, object value);

    /// <summary>
    /// Serializes a <see cref="ProjectConfig"/>.
    /// </summary>
    /// <param name="config">The <see cref="ProjectConfig"/> to serialize.</param>
    /// <param name="path">The path to store the result in.</param>
    public static void Serialize(ProjectConfig config, string path)
    {
        var configType = config.GetType();

        var typeInfo = ProjectTypeRegistry.GetProjectType(configType);
        Guard.IsNotNull(typeInfo);

        // Create the root node
        var root = new XElement("Project", new XAttribute("Type", typeInfo.TypeName));

        // Serialize ProjectConfig properties automatically
        WriteObjectProperties(root, config);

        var doc = new XDocument(root);
        doc.Save(path);
    }

    private static void WriteObjectProperties(XElement parent, object obj)
    {
        foreach (var prop in obj.GetType().GetProperties())
        {
            if (!prop.CanRead)
                continue;

            if (Attribute.IsDefined(prop, typeof(XmlIgnoreAttribute)))
                continue;

            var value = prop.GetValue(obj);
            if (value == null)
                continue;

            // Select serialization function
            SerializeDelegate @delegate = prop switch
            {
                _ when value is FormatConfig => SerializeFormatConfig,
                _ when prop.PropertyType.IsEnum => SerializeEnum,
                _ when IsSimple(prop.PropertyType) => SerializeSimple,
                _ => SerializeObject,
            };

            // Run serialization
            @delegate(parent, prop, value);
        }
    }

    private static void SerializeFormatConfig(XElement parent, PropertyInfo prop, object value)
    {
        var typeInfo = ProjectTypeRegistry.GetFormatType(value.GetType());
        Guard.IsNotNull(typeInfo);

        var formatElement = new XElement("FormatConfig", new XAttribute("Type", typeInfo.TypeName));

        WriteObjectProperties(formatElement, value);
        parent.Add(formatElement);
    }

    private static void SerializeSimple(XElement parent, PropertyInfo prop, object value)
    {
        parent.Add(new XElement(prop.Name, value));
    }

    private static void SerializeEnum(XElement parent, PropertyInfo prop, object value)
    {
        var member = prop.PropertyType.GetMember(value.ToString()!)[0];

        var attr = member
            .GetCustomAttributes(typeof(XmlEnumAttribute), false)
            .Cast<XmlEnumAttribute>()
            .FirstOrDefault();

        var name = attr?.Name ?? value.ToString()!;
        parent.Add(new XElement(prop.Name, name));
    }

    private static void SerializeObject(XElement parent, PropertyInfo prop, object value)
    {
        var element = new XElement(prop.Name);
        WriteObjectProperties(element, value);
        parent.Add(element);
    }

    private static bool IsSimple(Type type)
    {
        return type.IsPrimitive
            || type == typeof(string)
            || type == typeof(decimal)
            || type == typeof(DateTime);
    }
}
