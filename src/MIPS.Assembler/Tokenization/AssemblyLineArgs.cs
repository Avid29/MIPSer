// Adam Dernis 2024

namespace MIPS.Assembler.Tokenization;

using CommunityToolkit.HighPerformance;
using MIPS.Assembler.Tokenization.Enums;
using System;
using System.Collections.Generic;

/// <summary>
/// A struct for wrapping the assembly args component 
/// </summary>
public struct AssemblyLineArgs
{
    private ArraySegment<Token>[]? _args;

    /// <summary>
    /// Initializes a new instance of the <see cref="AssemblyLineArgs"/> struct.
    /// </summary>
    /// <param name="argsSegment"></param>
    public AssemblyLineArgs(ArraySegment<Token> argsSegment)
    {
        SplitArgs(argsSegment);
    }

    /// <summary>
    /// Gets an arg from the args array;
    /// </summary>
    public readonly ReadOnlySpan<Token> this[int index] => _args is not null ? _args[index] : [];

    /// <summary>
    /// Gets a range of args from the args array.
    /// </summary>
    public readonly ReadOnlySpan<ArraySegment<Token>> this[Range range] => _args is not null ? _args[range] : [];

    /// <summary>
    /// Gets the number of args.
    /// </summary>
    public readonly int Count => _args?.Length ?? 0;

    private void SplitArgs(ArraySegment<Token> tokens)
    {
        var args = new List<ArraySegment<Token>>();
        
        while (tokens.Count > 0)
        {
            var nextComma = ((ReadOnlySpan<Token>)tokens.AsSpan()).FindNext(TokenType.Comma);
            if (nextComma is -1)
            {
                // This is a base case for the final argument
                // We add the remaining tokens and break
                args.Add(tokens);
                break;
            }

            // Extract the argument up to the next comma,
            // then slice tokens to the start of the next argument
            args.Add(tokens[..nextComma]);
            tokens = tokens[(nextComma+1)..];
        }

        // Convert the list to an array segment
        _args = [..args];
    }
}
