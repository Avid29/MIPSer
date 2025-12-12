// Adam Dernis 2024

using MIPS.Models.Addressing;
using MIPS.Models.Modules.Tables.Enums;

namespace MIPS.Models.Modules.Tables;

/// <summary>
/// An entry in the load module's reference table.
/// </summary>
public struct ReferenceEntry
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ReferenceEntry"/> class.
    /// </summary>
    public ReferenceEntry(string? symbol, Address location, MipsReferenceType type, long append = 0)
    {
        Symbol = symbol;
        Location = location;
        Type = type;
        Append = append;
    }

    /// <summary>
    /// Gets or sets the symbol name.
    /// </summary>
    public string? Symbol { get; set; }

    /// <summary>
    /// Gets or sets the reference location.
    /// </summary>
    public Address Location { get; set; }

    /// <summary>
    /// Gets or sets how to perform the relocation.
    /// </summary>
    public MipsReferenceType Type { get; set; }

    /// <summary>
    /// Gets the appended value for
    /// </summary>
    public long Append { get; }
}
