// Adam Dernis 2024

using Zarem.MIPS.Models.Addressing;
using Zarem.MIPS.Models.Modules.Tables.Enums;
using System.Diagnostics.CodeAnalysis;

namespace Zarem.MIPS.Models.Modules.Tables;

/// <summary>
/// An entry in the load module's symbol table.
/// </summary>
public class SymbolEntry
{
    /// <summary>
    /// Initializes a new instance of the <see cref="SymbolEntry"/> class.
    /// </summary>
    public SymbolEntry(string name, Address? address, SymbolType type = SymbolType.Unknown, SymbolBinding binding = SymbolBinding.Local)
    {
        Name = name;
        Address = address;
        Type = type;
        Binding = binding;
    }

    /// <summary>
    /// Gets the symbol name of the entry.
    /// </summary>
    public string Name { get; }

    /// <summary>
    /// Gets or sets the address info for the symbol.
    /// </summary>
    public Address? Address { get; set; }

    /// <summary>
    /// Gets or sets the symbol type.
    /// </summary>
    public SymbolType Type { get; set; }

    /// <summary>
    /// Gets or sets the symbol flag info.
    /// </summary>
    public SymbolBinding Binding { get; set; }

    /// <summary>
    /// Gets or sets if the symbol was forward declared.
    /// </summary>
    public bool ForwardDeclared { get; set; }

    /// <summary>
    /// Gets whether or not the address is defined.
    /// </summary>
    [MemberNotNullWhen(true, nameof(Address))]
    public bool IsDefined => Address is not null;
}
