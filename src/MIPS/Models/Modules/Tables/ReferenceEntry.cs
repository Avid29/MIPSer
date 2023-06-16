// Adam Dernis 2023

using MIPS.Models.Addressing.Enums;
using MIPS.Models.Modules.Tables.Enums;
using System.Runtime.InteropServices;

namespace MIPS.Models.Modules.Tables;

/// <summary>
/// An entry in the load module's reference table.
/// </summary>
[StructLayout(LayoutKind.Explicit)]
public struct ReferenceEntry
{
    /// <summary>
    /// Gets the location of reference, within its section.
    /// </summary>
    [field: FieldOffset(0)]
    public uint Address { get; set; }

    /// <summary>
    /// Gets the index of the symbol name in the string table.
    /// </summary>
    [field: FieldOffset(4)]
    public uint SymbolIndex { get; set; }

    /// <summary>
    /// Gets the section the <see cref="Address"/> belongs in.
    /// </summary>
    [field: FieldOffset(8)]
    public Section Section { get; set; }
    
    /// <summary>
    /// Gets a <see cref="ReferenceType"/> describing how to preform the reference.
    /// </summary>
    [field: FieldOffset(9)]
    public ReferenceType Type { get; set; }
    
    /// <summary>
    /// Gets the index of the module containing the reference.
    /// </summary>
    [field: FieldOffset(10)]
    public ushort ModuleIndex { get; set; }
}
