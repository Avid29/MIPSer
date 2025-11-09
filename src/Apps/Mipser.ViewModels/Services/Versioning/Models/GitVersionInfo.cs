// Avishai Dernis 2025

namespace Mipser.Services.Versioning.Models;

/// <summary>
/// A struct containing git version info.
/// </summary>
public struct GitVersionInfo
{
    /// <summary>
    /// Gets the commit the build was made from.
    /// </summary>
    public string Commit { get; init; }

    /// <summary>
    /// Gets the name of the git branch the build was made from.
    /// </summary>
    public string Branch { get; init; }
}
