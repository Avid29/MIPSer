// Adam Dernis 2023

using MIPS.Assembler.Models.Directives.Abstract;

namespace MIPS.Assembler.Models.Directives;

/// <summary>
/// A <see cref="Directive"/> for memory alignments.
/// </summary>
public class AlignDirective : Directive
{
    /// <summary>
    /// Initializes a new instance of the <see cref="AlignDirective"/> class.
    /// </summary>
    public AlignDirective(int boundary)
    {
        Boundary = boundary;
    }

    /// <summary>
    /// Gets the alignment boundary.
    /// </summary>
    public int Boundary { get; }
}
