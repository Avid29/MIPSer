// Adam Dernis 2023

using MIPS.Assembler.Models.Directives.Abstract;

namespace MIPS.Assembler.Models.Directives;

/// <summary>
/// A <see cref="Directive"/> for global references.
/// </summary>
public class GlobalDirective : Directive
{
    /// <summary>
    /// Initializes a new instance of the <see cref="GlobalDirective"/> class.
    /// </summary>
    public GlobalDirective(string symbol)
    {
        Symbol = symbol;
    }

    /// <summary>
    /// Gets the symbol referenced.
    /// </summary>
    public string Symbol { get; }
}
