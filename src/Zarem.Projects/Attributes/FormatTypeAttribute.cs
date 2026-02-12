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
    /// <param name="typeName"></param>
    public FormatTypeAttribute(string typeName) => TypeName = typeName;

    /// <summary>
    /// Gets the project type's name.
    /// </summary>
    public string TypeName { get; }
}
