// Adam Dernis 2023

using MIPS.Assembler.Tokenization;
using MIPS.Assembler.Tokenization.Enums;

namespace System;

/// <summary>
/// A class containing extensions for <see cref="Span{Token}"/>.
/// </summary>
public static class TokenSpanExtensions
{
    /// <summary>
    /// Grabs the label declaration from the start of a token line.
    /// </summary>
    /// <param name="line">The line to trim.</param>
    /// <param name="type">The type of the </param>
    /// <param name="trimmed">The label on the line, if any.</param>
    public static Span<Token> TrimType(this Span<Token> line, TokenType type, out Token? trimmed)
    {
        trimmed = null;

        if (line.Length is 0)
            return line;

        // Find components starts
        if (line[0].Type == type)
        {
            trimmed = line[0];
            line = line[1..];
        }
        
        return line;
    }

    /// <summary>
    /// Splits a token line at a token type.
    /// </summary>
    /// <param name="line">The line to split.</param>
    /// <param name="type">The type to split on.</param>
    /// <param name="before">The line before the split.</param>
    /// <param name="split">The token that causes the split.</param>
    /// <returns>The token after the split.</returns>
    public static Span<Token> SplitAtNext(this Span<Token> line, TokenType type, out Span<Token> before, out Token? split)
    {
        for (var i = 0; i < line.Length; i++)
        {
            if (line[i].Type == type)
            {
                split = line[i];
                before = line[..i];
                return line[(i + 1)..];
            }
        }

        split = null;
        before = line;
        return [];
    }
}
