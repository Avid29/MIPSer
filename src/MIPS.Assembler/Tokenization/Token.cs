// Adam Dernis 2023

using MIPS.Assembler.Tokenization.Enums;

namespace MIPS.Assembler.Tokenization;

/// <summary>
/// A token for parsing the assembly.
/// </summary>
/// <param name="value">The token as a string.</param>
/// <param name="filename">The name of the file the token appears in.</param>
/// <param name="lineNum">The line the token appears on.</param>
/// <param name="column">The column of the token on the line.</param>
/// <param name="type">The token type.</param>
public class Token(string value, string? filename, int lineNum, int column, TokenType type)
{
    /// <summary>
    /// Gets the value of the token as a string.
    /// </summary>
    public string Value { get; } = value;

    /// <summary>
    /// Gets the token's filename.
    /// </summary>
    public string? Filename { get; } = filename;

    /// <summary>
    /// Gets the token's linenum.
    /// </summary>
    public int LineNum { get; } = lineNum;

    /// <summary>
    /// Gets the token's start column.
    /// </summary>
    public int Column {get; } = column;

    /// <summary>
    /// Gets the token's type.
    /// </summary>
    public TokenType Type { get; } = type;

    /// <inheritdoc/>
    public override string ToString() => Value;
}
