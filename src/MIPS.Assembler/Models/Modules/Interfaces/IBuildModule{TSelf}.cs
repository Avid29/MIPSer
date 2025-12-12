// Adam Dernis 2024

using MIPS.Assembler.Models.Config;
using System.IO;

namespace MIPS.Assembler.Models.Modules.Interfaces;

/// <summary>
/// An interface for a module implementation with knowledge of the underlying type.
/// </summary>
public interface IBuildModule<TSelf> : IBuildModule
    where TSelf : IBuildModule<TSelf>
{
    /// <summary>
    /// Creates a module from a <see cref="Module"/>.
    /// </summary>
    /// <param name="module">The <see cref="Module"/> to build from.</param>
    /// <param name="config">The configuration settings.</param>
    /// <returns>The constructed module.</returns>
    public static abstract TSelf? Create(Module module, AssemblerConfig config);
    
    /// <summary>
    /// Opens a module from a stream.
    /// </summary>
    /// <returns>The module contained in the stream.</returns>
    public static abstract TSelf? Open(string name, Stream stream);
}
