// Avishai Dernis 2026

using System;

namespace Zarem.Attributes;

/// <summary>
/// An attribute to mark a format type.
/// </summary>
[AttributeUsage(AttributeTargets.Class)]
public class FormatTypeAttribute : Attribute
{
    /// <summary>
    /// Initializes a new instance of the <see cref="FormatTypeAttribute"/> class.
    /// </summary>
    public FormatTypeAttribute(string typeName, Type configType)
    {
        TypeName = typeName;
        ConfigType = configType;
    }

    /// <summary>
    /// Gets the format's type's name.
    /// </summary>
    public string TypeName { get; }

    /// <summary>
    /// Gets the format's config <see cref="Type"/>.
    /// </summary>
    public Type ConfigType { get; }
}
