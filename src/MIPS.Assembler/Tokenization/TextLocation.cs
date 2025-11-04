// Avishai Dernis 2025

namespace MIPS.Assembler.Tokenization;

/// <summary>
/// A location in a text file.
/// </summary>
public struct TextLocation
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
