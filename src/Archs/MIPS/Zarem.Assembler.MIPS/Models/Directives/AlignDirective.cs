// Adam Dernis 2024

using Zarem.Assembler.MIPS.Models.Directives.Abstract;

namespace Zarem.Assembler.MIPS.Models.Directives;

/// <summary>
/// A <see cref="Directive"/> for memory alignments.
/// </summary>
public class AlignDirective(int boundary) : Directive
{
    /// <summary>
    /// Gets the alignment boundary.
    /// </summary>
    public int Boundary { get; } = boundary;
}
