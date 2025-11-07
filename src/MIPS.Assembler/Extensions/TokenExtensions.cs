// Avishai Dernis 2025

using System.Linq;
using System.Text.RegularExpressions;

namespace MIPS.Assembler.Tokenization.Models;

/// <summary>
/// A static class containing extensions on the <see cref="Token"/> type.
/// </summary>
public static class TokenExtensions
{
    /// <summary>
    /// Gets whether or not a token is numerical an identifier.
    /// </summary>
    public static bool IsIdentifier(this Token? token)
        => token?.Source.All(x => char.IsLetterOrDigit(x) || x is '_') ?? false;

    /// <summary>
    /// Gets whether or not a token is numerical.
    /// </summary>
    public static bool IsNumeric(this Token? token)
        => Regex.IsMatch(token?.Source ?? string.Empty, @"^[+-]?(?:0x[0-9a-fA-F]+|0b[01]+|0o[0-7]+|\d+)$");
}
