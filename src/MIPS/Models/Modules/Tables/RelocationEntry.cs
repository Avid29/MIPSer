// Adam Dernis 2024

using MIPS.Models.Addressing.Enums;
using MIPS.Models.Modules.Tables.Enums;
using System.Runtime.InteropServices;

namespace MIPS.Models.Modules.Tables;

/// <summary>
/// An entry in the load module's relocation table.
/// </summary>
[StructLayout(LayoutKind.Explicit)]
public struct RelocationEntry
{
    /// <summary>
    /// Initializes a new instance of the <see cref="RelocationEntry"/> struct.
    /// </summary>
    public RelocationEntry(uint address, Section section, RelocationType type)
    {
        Address = address;
        Section = section;
        Type = type;
    }

    /// <summary>
    /// Gets the address to be relocated.
    /// </summary>
    [field: FieldOffset(0)]
    public uint Address { get; set; }

    /// <summary>
    /// Gets the section number.
    /// </summary>
    [field: FieldOffset(4)]
    public Section Section { get; set; }

    /// <summary>
    /// Gets a <see cref="RelocationType"/> describing how to preform the relocation.
    /// </summary>
    [field: FieldOffset(5)]
    public RelocationType Type { get; set; }

    /// <summary>
    /// Fills the struct to 8 bytes.
    /// </summary>
    [field: FieldOffset(6)]
    private readonly ushort _filler;

}
