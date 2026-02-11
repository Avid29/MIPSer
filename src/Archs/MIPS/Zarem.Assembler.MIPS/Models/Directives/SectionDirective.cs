// Adam Dernis 2024

using Zarem.Assembler.MIPS.Models.Directives.Abstract;

namespace Zarem.Assembler.MIPS.Models.Directives;

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
