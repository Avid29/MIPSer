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
}
