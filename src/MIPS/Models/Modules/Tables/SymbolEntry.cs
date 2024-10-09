// Adam Dernis 2024

using System.Runtime.InteropServices;

namespace MIPS.Models.Modules.Tables;

/// <summary>
/// An entry in the load module's symbol table.
/// </summary>
[StructLayout(LayoutKind.Explicit)]
public struct SymbolEntry
{
    /// <summary>
    /// Gets flags on the symbol entry.
    /// </summary>
    [field: FieldOffset(0)]
    public uint Flags { get; set; }

    /// <summary>
    /// Gets the symbol's value.
    /// </summary>
    [field: FieldOffset(4)]
    public uint Value { get; set; }

    /// <summary>
    /// Gets the index of the symbol name in the string table.
    /// </summary>
    [field: FieldOffset(8)]
    public uint SymbolIndex { get; set; }

    /// <summary>
    /// Gets the index of the object file in the object file table.
    /// </summary>
    [field: FieldOffset(12)]
    public ushort ObjectFileIndex { get; set; }

    /// <summary>
    /// Fills the struct to 16 bytes.
    /// </summary>
    [field: FieldOffset(14)]
    private readonly ushort _filler;

}
