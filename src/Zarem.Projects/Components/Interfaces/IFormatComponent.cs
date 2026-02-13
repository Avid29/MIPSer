// Avishai Dernis 2026

using System.Threading.Tasks;
using Zarem.Config;
using Zarem.Models.Modules;

namespace Zarem.Components.Interfaces;

/// <summary>
/// An interface for a component of a <see cref="Project"/> that exports formatted binaries.
/// </summary>
public interface IFormatComponent : IProjectComponent
{
    /// <summary>
    /// Gets the object format config.
    /// </summary>
    public FormatConfig Config { get; }

    /// <summary>
    /// Attempts to export a module in the target format.
    /// </summary>
    /// <param name="module">The module to export.</param>
    /// <param name="path">The file to save the export in.</param>
    /// <returns><see langword="true"/> if successfully exported, <see langword="false"/> otherwise.</returns>
    Task<bool> TryExportAsync(Module module, string path);
}
