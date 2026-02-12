// Avishai Dernis 2026

using System;

namespace Zarem.Attributes;

/// <summary>
/// An attribute to mark a project type.
/// </summary>
[AttributeUsage(AttributeTargets.Class)]
public class ProjectTypeAttribute : Attribute
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ProjectTypeAttribute"/> class.
    /// </summary>
    public ProjectTypeAttribute(string typeName, Type configType)
    {
        TypeName = typeName;
        ConfigType = configType;
    }

    /// <summary>
    /// Gets the project type's name.
    /// </summary>
    public string TypeName { get; }

    /// <summary>
    /// Gets the project's config <see cref="Type"/>.
    /// </summary>
    public Type ConfigType { get; }
}
