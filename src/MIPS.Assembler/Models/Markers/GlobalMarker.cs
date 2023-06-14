// Adam Dernis 2023

using MIPS.Assembler.Models.Markers.Abstract;

namespace MIPS.Assembler.Models.Markers;

/// <summary>
/// A <see cref="Marker"/> for global references.
/// </summary>
public class GlobalMarker : Marker
{
    /// <summary>
    /// Initializes a new instance of the <see cref="GlobalMarker"/> class.
    /// </summary>
    public GlobalMarker(string symbol)
    {
        Symbol = symbol;
    }

    /// <summary>
    /// Gets the symbol referenced.
    /// </summary>
    public string Symbol { get; }
}
