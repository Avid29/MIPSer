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
    public ProjectTypeAttribute(string typeName, Type assemblerType, Type emulatorType)
    {
        TypeName = typeName;
        AssemblerType = assemblerType;
        EmulatorType = emulatorType;
    }

    /// <summary>
    /// Gets the project type's name.
    /// </summary>
    public string TypeName { get; }

    /// <summary>
    /// Gets the project's assembler <see cref="Type"/>.
    /// </summary>
    public Type AssemblerType { get; }

    /// <summary>
    /// Gets the project's emulator <see cref="Type"/>.
    /// </summary>
    public Type EmulatorType { get; }
}
