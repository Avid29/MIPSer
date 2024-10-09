// Adam Dernis 2024

using MIPS.Assembler.Models.Directives.Abstract;
using MIPS.Models.Addressing.Enums;

namespace MIPS.Assembler.Models.Directives;

/// <summary>
/// A <see cref="Directive"/> for section changes.
/// </summary>
public class SectionDirective(Section activeSection) : Directive
{
    /// <summary>
    /// Gets the new active section.
    /// </summary>
    public Section ActiveSection { get; } = activeSection;
}
