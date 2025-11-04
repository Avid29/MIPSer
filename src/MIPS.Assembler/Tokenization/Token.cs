// Adam Dernis 2024

using MIPS.Assembler.Tokenization.Enums;

namespace MIPS.Assembler.Tokenization;

/// <summary>
/// A token for parsing the assembly.
/// </summary>
/// <param name="source">The token as a string.</param>
/// <param name="filename">The name of the file the token appears in.</param>
/// <param name="location">The location of the start of the token.</param>
/// <param name="type">The token type.</param>
public class Token(string source, string? filename, TextLocation location, TokenType type)
{
    /// <summary>
    /// Gets the value of the token as a string.
    /// </summary>
    public string Source { get; } = source;

    /// <summary>
    /// Gets the token's filename.
    /// </summary>
    public string? Filename { get; } = filename;

    /// <summary>
    /// Gets the token's location.
    /// </summary>
    public TextLocation Location { get; } = location;

    /// <summary>
    /// Gets the token's type.
    /// </summary>
    public TokenType Type { get; } = type;

    /// <inheritdoc/>
    public override string ToString() => Source;
}
