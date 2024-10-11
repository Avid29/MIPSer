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
    /// <param name="index"></param>
    /// <returns></returns>
    public readonly ReadOnlySpan<Token> this[int index] => _args is not null ? _args[index].AsSpan() : [];

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

            var arg = tokens[..nextComma];
            tokens = tokens[(nextComma+1)..];
            args.Add(arg);
        }

        _args = [..args];
    }
}
