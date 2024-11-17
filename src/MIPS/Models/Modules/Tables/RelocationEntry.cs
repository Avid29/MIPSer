// Adam Dernis 2024

using MIPS.Models.Addressing;
using MIPS.Models.Modules.Tables.Enums;

namespace MIPS.Models.Modules.Tables;

/// <summary>
/// An entry in the load module's relocation table.
/// </summary>
public class RelocationEntry
{
    /// <summary>
    /// Initializes a new instance of the <see cref="RelocationEntry"/> class.
    /// </summary>
    public RelocationEntry(Address address, ReferenceType type)
    {
        Address = address;
        Type = type;
    }

    /// <summary>
    /// Gets or sets the address to be relocated.
    /// </summary>
    public Address Address { get; set; }
    
    /// <summary>
    /// Gets or sets a <see cref="ReferenceType"/> describing how to preform the reference.
    /// </summary>
    public ReferenceType Type { get; set; }
}
