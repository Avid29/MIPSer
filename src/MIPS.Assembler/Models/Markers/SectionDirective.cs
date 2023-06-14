// Adam Dernis 2023

using MIPS.Assembler.Models.Directives.Abstract;
using MIPS.Models.Addressing.Enums;

namespace MIPS.Assembler.Models.Directives;

/// <summary>
/// A <see cref="Directive"/> for section changes.
/// </summary>
public class SectionDirective : Directive
{
    /// <summary>
    /// Initializes a new instance of the <see cref="SectionDirective"/> class.
    /// </summary>
    public SectionDirective(Section activeSection)
    {
        ActiveSection = activeSection;
    }

    /// <summary>
    /// Gets the new active section.
    /// </summary>
    public Section ActiveSection { get; }
}
