// Adam Dernis 2024

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
    public static ReadOnlySpan<Token> TrimType(this ReadOnlySpan<Token> line, TokenType type, out Token? trimmed)
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
    /// Gets the index of the first instance of a token type in a span of tokens.
    /// </summary>
    /// <param name="line">The line to scan.</param>
    /// <param name="type">The type of the token to find.</param>
    /// <returns>The index of the first token of the <paramref name="type"/>, or -1 if none is found.</returns>
    public static int FindNext(this ReadOnlySpan<Token> line, TokenType type)
    {
        for (int i = 0; i < line.Length; i++)
        {
            if (line[i].Type == type)
                return i;
        }

        return -1;
    }
}
