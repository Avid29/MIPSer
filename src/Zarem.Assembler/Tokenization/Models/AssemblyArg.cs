// Avishai Dernis 2025

using System;

namespace Zarem.Assembler.Tokenization.Models;

/// <summary>
/// An argument in assembly.
/// </summary>
public struct AssemblyArg
{
    private readonly ArraySegment<Token> _tokens;

    /// <summary>
    /// Initializes a new instance of the <see cref="AssemblyArg"/> struct.
    /// </summary>
    public AssemblyArg(ArraySegment<Token> tokens, Token? preComma = null, Token? postComma = null)
    {
        _tokens = tokens;
        PrecedingComma = preComma;
        ProceedingComma = postComma;
    }

    /// <summary>
    /// Gets the tokens that make up the argument.
    /// </summary>
    public ReadOnlySpan<Token> Tokens => _tokens.AsSpan();

    /// <summary>
    /// Gets the <see cref="Token"/> for the comma preceding the argument.
    /// </summary>
    public Token? PrecedingComma { get; }

    /// <summary>
    /// Gets the <see cref="Token"/> for the comma proceeding the argument.
    /// </summary>
    public Token? ProceedingComma { get; }
}
