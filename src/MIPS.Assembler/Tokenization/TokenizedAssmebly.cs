// Adam Dernis 2024

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;

namespace MIPS.Assembler.Tokenization;

/// <summary>
/// A class for wrapping tokenized assembly files.
/// </summary>
public class TokenizedAssmebly
{
    private readonly List<List<Token>> _tokenLines;

    /// <summary>
    /// Initializes a new instance of the <see cref="TokenizedAssmebly"/> class.
    /// </summary>
    public TokenizedAssmebly(List<List<Token>> tokens) => _tokenLines = tokens;

    /// <summary>
    /// Gets a line's token list as a span.
    /// </summary>
    /// <param name="line">The number of the line to retrieve.</param>
    /// <returns>The line's token list as a span.</returns>
    public AssemblyLine this[int line]
    {
        get => new(CollectionsMarshal.AsSpan(_tokenLines[line - 1]));
    }

    /// <summary>
    /// Gets the total number of lines.
    /// </summary>
    public int LineCount => _tokenLines.Count;

    /// <summary>
    /// Gets the total number of tokens.
    /// </summary>
    public int TokenCount => _tokenLines.Sum(x => x.Count);
}
