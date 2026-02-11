// Avishai Dernis 2025

namespace Zarem.Models;

/// <summary>
/// A class for containing the compoenents of a dependencies details.
/// </summary>
public class ThirdPartyNotice
{
    /// <summary>
    /// Gets the name of the dependency.
    /// </summary>
    public required string DependencyName { get; init; }

    /// <summary>
    /// Gets the url link to the dependency.
    /// </summary>
    public required string Url { get; init; }
}
