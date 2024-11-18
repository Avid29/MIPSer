// Adam Dernis 2024

using MIPS.Models.Addressing;
using MIPS.Models.Modules.Tables.Enums;

namespace MIPS.Models.Modules.Tables;

/// <summary>
/// An entry in the load module's reference table.
/// </summary>
public class ReferenceEntry
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ReferenceEntry"/> class.
    /// </summary>
    public ReferenceEntry(string? symbol, Address address, ReferenceType type, ReferenceMethod method)
    {
        Symbol = symbol;
        Address = address;
        Type = type;
        Method = method;
    }

    /// <summary>
    /// Gets or sets the symbol name.
    /// </summary>
    public string? Symbol { get; set; }

    /// <summary>
    /// Gets or sets the reference location.
    /// </summary>
    public Address Address { get; set; }

    /// <summary>
    /// Gets or sets a <see cref="ReferenceType"/> describing where to preform the bit modification.
    /// </summary>
    public ReferenceType Type { get; set; }
    
    /// <summary>
    /// Gets or sets a <see cref="ReferenceType"/> describing how to preform the bit modification.
    /// </summary>
    public ReferenceMethod Method { get; set; }

    /// <summary>
    /// Gets whether or not the reference is a relocation.
    /// </summary>
    public bool IsRelocation => Method is ReferenceMethod.Relocate;
}
