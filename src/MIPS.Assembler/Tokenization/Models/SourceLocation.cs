// Avishai Dernis 2025

namespace MIPS.Assembler.Tokenization.Models;

/// <summary>
/// The location of a <see cref="Token"/> in a source file.
/// </summary>
public struct SourceLocation
{
    /// <summary>
    /// Gets the index of the location.
    /// </summary>
    public required int Index { get; set; }

    /// <summary>
    /// Gets the row of the location file.
    /// </summary>
    public required int Line { get; set; }

    /// <summary>
    /// Gets the column of the location.
    /// </summary>
    public required int Column { get; set; }
}
