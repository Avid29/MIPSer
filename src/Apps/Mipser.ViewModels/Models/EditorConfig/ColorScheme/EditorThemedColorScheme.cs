// Avishai Dernis 2025

using System.Text.Json.Serialization;

namespace Mipser.Models.EditorConfig.ColorScheme;

/// <summary>
/// A color scheme for the code editor with theme context containing all the color info.
/// </summary>
public record EditorThemedColorScheme
{
    /// <summary>
    /// Gets the foreground color of the color scheme.
    /// </summary>
    [JsonPropertyName("foreground")]
    public required string Foreground { get; init; }

    /// <summary>
    /// Gets the background color of the color scheme.
    /// </summary>
    [JsonPropertyName("background")]
    public required string Background { get; init; }

    /// <summary>
    /// Gets the syntax highlighting scheme of the color scheme.
    /// </summary>
    [JsonPropertyName("syntax_highlighting")]
    public required SyntaxHighlightingScheme SyntaxHighlighting { get; init; }
}
