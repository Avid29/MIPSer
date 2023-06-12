// Adam Dernis 2023

using MIPS.Assembler.Models.Markers.Abstract;

namespace MIPS.Assembler.Models.Markers;

/// <summary>
/// A <see cref="Marker"/> for memory alignments.
/// </summary>
public class AlignMarker : Marker
{
    /// <summary>
    /// Initializes a new instance of the <see cref="AlignMarker"/> class.
    /// </summary>
    public AlignMarker(int boundary)
    {
        Boundary = boundary;
    }

    /// <summary>
    /// Gets the alignment boundary.
    /// </summary>
    public int Boundary { get; }
}
