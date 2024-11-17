// Adam Dernis 2024

using System.IO;

namespace MIPS.Assembler.Models.Modules.Interfaces;

/// <summary>
/// An interface for a module implementation.
/// </summary>
public interface IModule<TSelf>
    where TSelf : IModule<TSelf>
{
    /// <summary>
    /// Creates a module from a <see cref="ModuleConstructor"/>.
    /// </summary>
    /// <param name="stream">The stream to write the module to.</param>
    /// <param name="constructor">The <see cref="ModuleConstructor"/> to build from.</param>
    /// <param name="config">The configuration settings.</param>
    /// <returns>The constructed module.</returns>
    public static abstract TSelf? Create(Stream stream, ModuleConstructor constructor, AssemblerConfig config);
    
    /// <summary>
    /// Loads a module from a stream.
    /// </summary>
    /// <returns>The module contained in the stream.</returns>
    public static abstract TSelf? Load(Stream stream);

    /// <summary>
    /// Abstracts the module into a <see cref="ModuleConstructor"/> for modification or linking.
    /// </summary>
    /// <param name="config">The configuration settings.</param>
    /// <returns>The module as a <see cref="ModuleConstructor"/>.</returns>
    public ModuleConstructor? Abstract(AssemblerConfig config);
}
