// Adam Dernis 2024

using CommunityToolkit.Diagnostics;
using MIPS.Models.Addressing;
using MIPS.Models.Addressing.Enums;
using MIPS.Models.Modules.Tables.Enums;
using System.Numerics;
using System.Runtime.InteropServices;

namespace MIPS.Models.Modules.Tables;

/// <summary>
/// An entry in the load module's symbol table.
/// </summary>
[StructLayout(LayoutKind.Explicit)]
public struct SymbolEntry
{
    /// <summary>
    /// Initializes a new instance of the <see cref="SymbolEntry"/> struct.
    /// </summary>
    public SymbolEntry(Address address)
    {
        Value = (uint)address.Value;
        Section = address.Section;
        Flags = SymbolFlags.Defined;
    }

    /// <summary>
    /// Gets flags on the symbol entry.
    /// </summary>
    [field: FieldOffset(0)]
    public SymbolFlags Flags { get; set; }

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
    /// <remarks>
    /// This is not part of RASM, but a convenience of mine.
    /// </remarks>
    [field: FieldOffset(14)]
    private Section Section { get; set; }

    /// <summary>
    /// Fills the struct to 16 bytes.
    /// </summary>
    [field: FieldOffset(15)]
    private readonly byte _filler;

    /// <summary>
    /// Gets if the symbol is defined.
    /// </summary>
    public readonly bool IsDefined => Flags.HasFlag(SymbolFlags.Defined);

    /// <summary>
    /// Gets if a flag is set on the symbol entry.
    /// </summary>
    /// <param name="flag">The flag to check.</param>
    /// <returns>True if the flag is set, false otherwise.</returns>
    public readonly bool CheckFlag(SymbolFlags flag) => Flags.HasFlag(flag);

    /// <summary>
    /// Sets a flag on the symbol entry.
    /// </summary>
    /// <param name="flag">The flag to set.</param>
    /// <param name="state">The new state of the flag.</param>
    public void SetFlag(SymbolFlags flag, bool state)
    {
        if (!BitOperations.IsPow2((int)flag))
            ThrowHelper.ThrowArgumentException("Only one flag may be set at a time");
        
        // Clear the flag.
        Flags &= ~flag;

        // Set the flag to true.
        if (state)
        {
            Flags |= flag;
        }
    }

    /// <summary>
    /// Gets the address of the symbol entry.
    /// </summary>
    public readonly Address Address => new(Value, Section);

    /// <inheritdoc/>
    public readonly SymbolEntry Read(Stream stream)
    {
        stream.TryRead<uint>(out var flags);
        stream.TryRead<uint>(out var value);
        stream.TryRead<uint>(out var symbolIndex);
        stream.TryRead<ushort>(out var objFileIndex);
        stream.TryRead<byte>(out var section);
        stream.TryRead<byte>(out _);

        return new()
        {
            Flags = (SymbolFlags)flags,
            Value = value,
            SymbolIndex = symbolIndex,
            ObjectFileIndex = objFileIndex,
            Section = Section
        };
    }
    
    /// <inheritdoc/>
    public readonly void Write(Stream stream)
    {
        stream.TryWrite((uint)Flags);
        stream.TryWrite(Value);
        stream.TryWrite(SymbolIndex);
        stream.TryWrite(ObjectFileIndex);
        stream.TryWrite((byte)Section);
        stream.TryWrite(_filler);
    }
}
