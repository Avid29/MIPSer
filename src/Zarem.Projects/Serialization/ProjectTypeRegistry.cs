// Avishai Dernis 2026

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Zarem.Attributes;
using Zarem.Config;
using Zarem.Models.Modules.Interface;

namespace Zarem.Serialization;

/// <summary>
/// Central registry that maps string identifiers from XML
/// to concrete .NET types.
/// 
/// This allows polymorphic deserialization without compile-time
/// knowledge of architecture assemblies.
/// </summary>
public static class ProjectTypeRegistry
{
    private static readonly Dictionary<string, ProjectTypeInfo> _projectTypes = [];
    private static readonly Dictionary<string, FormatTypeInfo> _formatTypes = [];
    private static readonly Dictionary<Type, ProjectTypeInfo> _projectTypesReverse = [];
    private static readonly Dictionary<Type, FormatTypeInfo> _formatTypesReverse = [];

    static ProjectTypeRegistry()
    {
        //Populate();
    }

    /// <summary>
    /// Registers a project type.
    /// </summary>
    /// <param name="typeName">The name of the type.</param>
    /// <param name="projectType">The <see cref="Type"/> for the project class.</param>
    /// <param name="configType">The <see cref="Type"/> for the project config class.</param>
    public static void RegisterProject(string typeName, Type projectType, Type configType)
    {
        var typeInfo = new ProjectTypeInfo(typeName, projectType, configType);
        _projectTypes[typeName] = typeInfo;
        _projectTypesReverse[projectType] = typeInfo;
        _projectTypesReverse[configType] = typeInfo;
    }

    /// <summary>
    /// Registers a project type.
    /// </summary>
    /// <typeparam name="T">The type of project to register.</typeparam>
    public static void RegisterProject<T>()
        where T : IProject
        => TryRegisterProject(typeof(T));

    /// <summary>
    /// Registers an object format type.
    /// </summary>
    /// <param name="typeName">The name of the type.</param>
    /// <param name="formatType">The <see cref="Type"/> for the format config class.</param>
    /// <param name="configType">The <see cref="Type"/> for the format config class.</param>
    public static void RegisterFormat(string typeName, Type formatType, Type configType)
    {
        var typeInfo = new FormatTypeInfo(typeName, formatType, configType);
        _formatTypes[typeName] = typeInfo;
        _formatTypesReverse[formatType] = typeInfo;
        _formatTypesReverse[configType] = typeInfo;
    }

    /// <summary>
    /// Registers a format type.
    /// </summary>
    /// <typeparam name="T">The type of format to register.</typeparam>
    public static void RegisterFormat<T>()
        where T : IModule
        => TryRegisterFormat(typeof(T));

    /// <summary>
    /// Resolves a project type by its XML "Type" attribute.
    /// </summary>
    internal static ProjectTypeInfo? GetProjectType(string typeName)
    {
        if (_projectTypes.TryGetValue(typeName, out var type))
            return type;

        return null;
    }

    /// <summary>
    /// Resolves a project type by its XML "Type" attribute.
    /// </summary>
    internal static ProjectTypeInfo? GetProjectType(Type projType)
    {
        if (_projectTypesReverse.TryGetValue(projType, out var type))
            return type;

        return null;
    }

    /// <summary>
    /// Resolves a format type by its XML "Type" attribute.
    /// </summary>
    internal static FormatTypeInfo? GetFormatType(string typeName)
    {
        if (_formatTypes.TryGetValue(typeName, out var type))
            return type;

        return null;
    }

    /// <summary>
    /// Resolves a format type by its XML "Type" attribute.
    /// </summary>
    internal static FormatTypeInfo? GetFormatType(Type formatType)
    {
        if (_formatTypesReverse.TryGetValue(formatType, out var type))
            return type;

        return null;
    }

    /// <summary>
    /// Scan loaded assemblies to find all derived types for the <see cref="ProjectConfig"/> and <see cref="FormatConfig"/>.
    /// </summary>
    internal static void Populate()
    {
        // Get all loaded assemblies in the current AppDomain
        var assemblies = AppDomain.CurrentDomain.GetAssemblies();

        foreach (var assembly in assemblies)
        {
            Type[] types;
            try
            {
                types = assembly.GetTypes();
            }
            catch (ReflectionTypeLoadException ex)
            {
                // Some assemblies may fail to load types (e.g., native dependencies)
                types = ex.Types.Where(t => t != null).ToArray()!;
            }

            foreach (var type in types)
            {
                if (type == null || type.IsAbstract)
                    continue;

                // Register ProjectConfigs
                if (typeof(Project<,,,>).IsAssignableFrom(type))
                    TryRegisterProject(type);

                // Register FormatConfigs
                if (typeof(FormatConfig).IsAssignableFrom(type))
                    TryRegisterFormat(type);
            }
        }
    }

    private static void TryRegisterProject(Type projectType)
    {
        var attr = projectType.GetCustomAttribute<ProjectTypeAttribute>();
        if (attr is null)
            return;

        var typeName = attr.TypeName;
        var configType = attr.ConfigType;

        // Register the type
        RegisterProject(typeName, projectType, configType);
    }

    private static void TryRegisterFormat(Type formatType)
    {
        var attr = formatType.GetCustomAttribute<FormatTypeAttribute>();
        if (attr is null)
            return;

        var typeName = attr.TypeName;
        var configType = attr.ConfigType;

        // Register the type
        RegisterFormat(typeName, formatType, configType);
    }
}
