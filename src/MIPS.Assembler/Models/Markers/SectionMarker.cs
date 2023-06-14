// Adam Dernis 2023

using MIPS.Assembler.Models.Markers.Abstract;
using MIPS.Models.Addressing.Enums;

namespace MIPS.Assembler.Models.Markers;

/// <summary>
/// A <see cref="Marker"/> for section changes.
/// </summary>
public class SectionMarker : Marker
{
    /// <summary>
    /// Initializes a new instance of the <see cref="SectionMarker"/> class.
    /// </summary>
    public SectionMarker(Section activeSection)
    {
        ActiveSection = activeSection;
    }

    /// <summary>
    /// Gets the new active section.
    /// </summary>
    public Section ActiveSection { get; }
}
