// Avishai Dernis 2025

namespace Zarem.Services.Versioning.Models;

/// <summary>
/// A struct containing the app's version info.
/// </summary>
public readonly struct AppVersion(ushort major, ushort minor, ushort revision)
{
    /// <summary>
    /// Gets the major version.
    /// </summary>
    public ushort MajorVersion { get; init; } = major;
    
    /// <summary>
    /// Gets the minor version.
    /// </summary>
    public ushort MinorVersion { get; init; } = minor;
    
    /// <summary>
    /// Gets the revision number.
    /// </summary>
    public ushort Revision { get; init; } = revision;
}
