// Avishai Dernis 2026

using CommunityToolkit.Diagnostics;
using System;
using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;
using Zarem.Assembler.Config;
using Zarem.Components;
using Zarem.Components.Interfaces;
using Zarem.Config;
using Zarem.Emulator.Config;
using Zarem.Serialization.Registry;

namespace Zarem.Serialization;

/// <summary>
/// A class for creating <see cref="IProject"/> types.
/// </summary>
public static class ProjectFactory
{
    /// <summary>
    /// Constructs an <see cref="IProject"/> from an <see cref="IProjectConfig"/>.
    /// </summary>
    /// <param name="config"></param>
    /// <returns></returns>
    public static IProject Create(IProjectConfig config)
    {
        Guard.IsNotNull(config.AssemblerConfig);
        Guard.IsNotNull(config.EmulatorConfig);
        Guard.IsNotNull(config.FormatConfig);

        // Retrieve type info
        var typeInfo = ProjectTypeRegistry.GetProjectType(config.GetType());
        var formatTypeInfo = ProjectTypeRegistry.GetFormatType(config.FormatConfig.GetType());
        Guard.IsNotNull(typeInfo);
        Guard.IsNotNull(formatTypeInfo);

        // Create components
        var assemble = CreateComponent<IAssembleComponent, AssemblerConfig>(typeof(AssembleComponent<,>), typeInfo.ProjectType.AssemblerType, config.AssemblerConfig);
        var emulate = CreateComponent<IEmulateComponent, EmulatorConfig>(typeof(EmulateComponent<,>), typeInfo.ProjectType.EmulatorType, config.EmulatorConfig);
        var format = CreateComponent<IFormatComponent, FormatConfig>(typeof(FormatComponent<,>), formatTypeInfo.FormatType, config.FormatConfig);

        var project = new Project(config, assemble, emulate, format);
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

    private static T CreateComponent<T, TConfig>(Type openType, Type primaryType, TConfig config)
        where T : IProjectComponent
        where TConfig : notnull
    {
        // Form a closed-type format component
        var closedType = openType.MakeGenericType(primaryType, config.GetType());

        // Instantializes
        var compoent = (T?)Activator.CreateInstance(closedType, config);
        Guard.IsNotNull(compoent);

        return compoent;
    }
}
