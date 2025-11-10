// Adam Dernis 2024

using System.IO;

namespace MIPS.Assembler.Models.Modules.Interfaces;

/// <summary>
/// An interface for a module implementation with knowledge of the underlying type.
/// </summary>
public interface IBuildModule<TSelf> : IBuildModule
    where TSelf : IBuildModule<TSelf>
{
    /// <summary>
    /// Creates a module from a <see cref="ModuleConstructor"/>.
    /// </summary>
    /// <param name="constructor">The <see cref="ModuleConstructor"/> to build from.</param>
    /// <param name="config">The configuration settings.</param>
    /// <param name="stream">The stream to write the module to. A new stream will be created if null.</param>
    /// <returns>The constructed module.</returns>
    public static abstract TSelf? Create(ModuleConstructor constructor, AssemblerConfig config, Stream? stream = null);
    
    /// <summary>
    /// Loads a module from a stream.
    /// </summary>
    /// <returns>The module contained in the stream.</returns>
    public static abstract TSelf? Load(string name, Stream stream);
}
