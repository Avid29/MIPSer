// Adam Dernis 2023

using MIPS.Assembler.Models.Markers.Abstract;
using MIPS.Models.Addressing.Enums;

namespace MIPS.Assembler.Models.Markers;

/// <summary>
/// A <see cref="Marker"/> for segment changes.
/// </summary>
public class SegmentMarker : Marker
{
    /// <summary>
    /// Initializes a new instance of the <see cref="SegmentMarker"/> class.
    /// </summary>
    public SegmentMarker(Segment activeSegment)
    {
        ActiveSegment = activeSegment;
    }

    /// <summary>
    /// Gets the new active segment.
    /// </summary>
    public Segment ActiveSegment { get; }
}
