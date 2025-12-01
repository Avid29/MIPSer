// Avishai Dernis 2025

using System.Text.Json.Serialization;

namespace Mipser.Models.EditorConfig.ColorScheme;

/// <summary>
/// A color scheme for the code editor with a dark and light theme variant.
/// </summary>
public record EditorColorScheme
{
    /// <summary>
    /// Gets the name of the color scheme.
    /// </summary>
    [JsonPropertyName("name")]
    public required string Name { get; init; }

    /// <summary>
    /// Gets the dark theme variant of the color scheme.
    /// </summary>
    [JsonPropertyName("dark")]
    public EditorThemedColorScheme? Dark { get;  }

    /// <summary>
    /// Gets the light theme variant of the color scheme.
    /// </summary>
    [JsonPropertyName("light")]
    public EditorThemedColorScheme? Light { get;  }
}
