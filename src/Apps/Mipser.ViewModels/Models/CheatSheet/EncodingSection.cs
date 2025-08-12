// Avishai Dernis 2025

using MIPS.Models.Instructions.Enums;
using System.Text.Json.Serialization;

namespace Mipser.Models.CheatSheet;

/// <summary>
/// A class representing a section of encodings in the cheatsheet.
/// </summary>
public record EncodingSection
{
    /// <summary>
    /// Gets or sets the name of the encoding section.
    /// </summary>
    [JsonPropertyName("name")]
    public string? Name { get; set; }

    /// <summary>
    /// Gets or sets the number of bits used for the section.
    /// </summary>
    [JsonPropertyName("size")]
    public int BitSize { get; set; }

    /// <summary>
    /// Gets or sets the starting bit offset for the section.
    /// </summary>
    [JsonPropertyName("offset")]
    public int BitOffset { get; set; }

    /// <summary>
    /// Gets or sets the constant value for the section, if applicable.
    /// </summary>
    [JsonPropertyName("value")]
    public int? ConstantValue { get; set; }

    /// <summary>
    /// Gets or sets the type of the section, indicating how it should be interpreted.
    /// </summary>
    public Argument? Argument { get; set; }

    /// <summary>
    /// Gets the start of the bit range for the section.
    /// </summary>
    public int BitRangeStart => BitOffset;

    /// <summary>
    /// Gets the end of the bit range for the section.
    /// </summary>
    public int BitRangeEnd => BitOffset + BitSize - 1;
}
