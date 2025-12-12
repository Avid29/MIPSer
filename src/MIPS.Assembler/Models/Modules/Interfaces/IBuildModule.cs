// Adam Dernis 2024

using MIPS.Assembler.Models.Config;
using System.IO;

namespace MIPS.Assembler.Models.Modules.Interfaces;

/// <summary>
/// An interface for a module implementation.
/// </summary>
public interface IBuildModule
{
    /// <summary>
    /// Gets the name of the <see cref="IBuildModule"/>.
    /// </summary>
    public string? Name { get; }

    /// <summary>
    /// Abstracts the module into a <see cref="Module"/> for modification or linking.
    /// </summary>
    /// <param name="config">The configuration settings.</param>
    /// <returns>The module as a <see cref="Module"/>.</returns>
    public Module? Abstract(AssemblerConfig config);

    /// <summary>
    /// Save the module to a stream (likely as a file).
    /// </summary>
    public void Save(Stream stream);
}
