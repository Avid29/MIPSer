// Adam Dernis 2023

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
    /// Fills the struct to 8 bytes.
    /// </summary>
    [field: FieldOffset(6)] private readonly ushort _filler;

    /// <summary>
    /// Gets the address to be relocated.
    /// </summary>
    [field: FieldOffset(0)]
    public uint Address { get; set; }

    /// <summary>
    /// Gets the section number.
    /// </summary>
    [field: FieldOffset(4)]
    public byte Section { get; set; }

    /// <summary>
    /// Gets a <see cref="RelocationType"/> describing how to preform the relocation.
    /// </summary>
    [field: FieldOffset(5)]
    public RelocationType Type { get; set; }
}
