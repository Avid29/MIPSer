// Adam Dernis 2024

using MIPS.Assembler.Models.Directives.Abstract;

namespace MIPS.Assembler.Models.Directives;

/// <summary>
/// A <see cref="Directive"/> for section changes.
/// </summary>
public class SectionDirective(string section) : Directive
{
    /// <summary>
    /// Gets the new active section.
    /// </summary>
    public string Section { get; } = section;
}
