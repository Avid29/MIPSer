// Avishai Dernis 2025

namespace Zarem.Localization;

/// <summary>
/// An interface
/// </summary>
public interface IStringLocalizer
{
    /// <summary>
    /// Gets the localized string for the given key.
    /// </summary>
    public string? this[string key, params object?[] args] { get; }
}
