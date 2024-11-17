// Adam Dernis 2024

using MIPS.Models.Addressing;
using MIPS.Models.Modules.Tables.Enums;
using System.Diagnostics.CodeAnalysis;

namespace MIPS.Models.Modules.Tables;

/// <summary>
/// An entry in the load module's symbol table.
/// </summary>
public class SymbolEntry
{
    /// <summary>
    /// Initializes a new instance of the <see cref="SymbolEntry"/> class.
    /// </summary>
    /// <param name="symbol"></param>
    /// <param name="address"></param>
    public SymbolEntry(string? symbol, Address? address)
    {
        Symbol = symbol;
        Address = address;
    }


    /// <summary>
    /// Gets or sets the symbol name of the entry.
    /// </summary>
    public string? Symbol { get; set; }

    /// <summary>
    /// Gets or sets the value of the symbol as an <see cref="Addressing.Address"/>.
    /// </summary>
    public Address? Address { get; set; }

    /// <summary>
    /// Gets or sets the symbol type.
    /// </summary>
    public SymbolType Type { get; set; }

    /// <summary>
    /// Gets or sets if the symbol is global.
    /// </summary>
    public bool Global { get; set; }

    /// <summary>
    /// Gets whether or not the symbol is defined.
    /// </summary>
    [MemberNotNullWhen(true, nameof(Address))]
    public bool IsDefined => Address.HasValue;
}
