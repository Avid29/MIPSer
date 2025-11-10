// Avishai Dernis 2025

namespace MIPS.Assembler.Tokenization.Models;

/// <summary>
/// The location of a <see cref="Token"/> in a source file.
/// </summary>
public struct SourceLocation
{
    /// <summary>
    /// Initializes a new instance of the <see cref="SourceLocation"/> struct.
    /// </summary>
    public SourceLocation()
    {
        Index = 0;
        Line = 1;
        Column = 1;
    }

    /// <summary>
    /// Gets the index of the location.
    /// </summary>
    public int Index { get; set; }

    /// <summary>
    /// Gets the row of the location file.
    /// </summary>
    public int Line { get; set; }

    /// <summary>
    /// Gets the column of the location.
    /// </summary>
    public int Column { get; set; }

    /// <summary>
    /// Gets the next line
    /// </summary>
    /// <returns></returns>
    public SourceLocation NextLine(int incSize = 1)
        => new()
        {
            Index = this.Index + incSize,
            Line = this.Line + 1,
            Column = 1,
        };

    /// <summary>
    /// Adds a number of characters to the location.
    /// </summary>
    public static SourceLocation operator +(SourceLocation pos, int inc)
        => new()
        {
            Index = pos.Index + inc,
            Line = pos.Line,
            Column = pos.Column + inc,
        };
}
