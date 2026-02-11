// Avishai Dernis 2025

using System;

namespace Zarem.Services.Versioning.Models;

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

    /// <summary>
    /// Gets the sha hash of the commit.
    /// </summary>
    public string Sha { get; init; }

    /// <summary>
    /// Gets the date the commit was made.
    /// </summary>
    public DateTime CommitDate { get; init; }
}
