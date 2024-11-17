// Adam Dernis 2024

using MIPS.Assembler.Models.Modules.Interfaces.Tables;
using MIPS.Helpers;
using MIPS.Models.Addressing;
using MIPS.Models.Addressing.Enums;
using RASM.Modules.Tables.Enums;
using System.Runtime.InteropServices;

using CommonEntry = MIPS.Models.Modules.Tables.SymbolEntry;

namespace RASM.Modules.Tables;

/// <summary>
/// An entry in the RASM load module's symbol table.
/// </summary>
[StructLayout(LayoutKind.Explicit)]
public struct SymbolEntry : ISymbolEntry<SymbolEntry>
{
    private const byte SectionBitCount = 4;

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
    /// Gets or sets flags on the symbol entry.
    /// </summary>
    [field: FieldOffset(0)]
    public SymbolFlags Flags { get; set; }

    /// <summary>
    /// Gets or sets the symbol's value.
    /// </summary>
    [field: FieldOffset(4)]
    public uint Value { get; set; }

    /// <summary>
    /// Gets or sets the index of the symbol name in the string table.
    /// </summary>
    [field: FieldOffset(8)]
    public uint SymbolIndex { get; set; }

    /// <summary>
    /// Gets or sets the index of the object file in the object file table.
    /// </summary>
    [field: FieldOffset(12)]
    public ushort ObjectFileIndex { get; set; }

    /// <summary>
    /// Fills the struct to 16 bytes.
    /// </summary>
    [field: FieldOffset(14)]
    private readonly ushort _filler;

    /// <summary>
    /// Gets or sets the section specified by the 
    /// </summary>
    public Section Section
    {
        readonly get => (Section)UintMasking.GetShiftMask((uint)Flags, SectionBitCount, 0);
        set
        {
            var cast = (uint)Flags;
            UintMasking.SetShiftMask(ref cast, SectionBitCount, 0, (uint)value);
            Flags = (SymbolFlags)cast;
        }
    }

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
    /// Sets a set of flags on the symbol entry.
    /// </summary>
    /// <param name="flags">The flags to set.</param>
    /// <param name="state">The new state of the flag.</param>
    public void SetFlags(SymbolFlags flags, bool state)
    {
        // Clear the flag.
        Flags &= ~flags;

        // Set the flag to true.
        if (state)
        {
            Flags |= flags;
        }
    }

    /// <summary>
    /// Gets the address of the symbol entry.
    /// </summary>
    public Address Address
    {
        get => new(Value, Section);
        set
        {
            Value = (uint)value.Value;
            Section = value.Section;
        }
    }

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
        };
    }
    
    /// <inheritdoc/>
    public readonly void Write(Stream stream)
    {
        stream.TryWrite((uint)Flags);
        stream.TryWrite(Value);
        stream.TryWrite(SymbolIndex);
        stream.TryWrite(ObjectFileIndex);
        stream.TryWrite(_filler);
    }
    
    /// <inheritdoc/>
    public static SymbolEntry Convert(CommonEntry entry)
    {
        // Initialize symbol
        var symbol = new SymbolEntry();
        if (entry.IsDefined)
        {
            symbol = new SymbolEntry(entry.Address.Value);
        }
        
        // Set flags
        symbol.SetFlags(SymbolFlags.Global, entry.Global);

        // Return
        return symbol;
    }
}
