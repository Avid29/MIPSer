// Avishai Dernis 2026

using CommunityToolkit.Diagnostics;
using System;
using System.Xml.Linq;
using Zarem.Config;
using Zarem.Serialization.Registry;

namespace Zarem.Serialization;

/// <summary>
/// A class for serializing/deserialization <see cref="ProjectConfig"/> instances.
/// </summary>
public static class ProjectSerializer
{
    internal static void ReadObjectProperties(XElement element, object obj)
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

    internal static void WriteObjectProperties(XElement parent, object obj)
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
