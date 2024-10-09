// Adam Dernis 2024

using MIPS.Assembler.Models.Directives.Abstract;

namespace MIPS.Assembler.Models.Directives;

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
