// Adam Dernis 2024

using MIPS.Assembler.Models.Modules.Interfaces.Tables;
using MIPS.Helpers;
using MIPS.Models.Addressing;
using MIPS.Models.Addressing.Enums;
using RASM.Modules.Tables.Enums;
using System.Runtime.InteropServices;

using CommonEntry = MIPS.Models.Modules.Tables.SymbolEntry;
using SymbolType = MIPS.Models.Modules.Tables.Enums.SymbolType;

namespace RASM.Modules.Tables;

/// <summary>
/// An entry in the RASM load module's symbol table.
/// </summary>
[StructLayout(LayoutKind.Explicit)]
public struct SymbolEntry : ISymbolEntry<SymbolEntry>, IBigEndianReadWritable<SymbolEntry>
{
    private const byte SectionBitCount = 4;
    
    /// <summary>
    /// Initializes a new instance of the <see cref="SymbolEntry"/> struct.
    /// </summary>
    public SymbolEntry(Address? address)
    {
        if (address.HasValue)
        {
            var adr = address.Value;
            Value = (uint)adr.Value;
            Section = adr.Section;
            SetFlags(SymbolFlags.Defined, true);
        }
        else
        {
            Value = 0;
            Section = Section.External;
        }
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
        var symbol = new SymbolEntry(entry.Address);
        
        // Set flags
        symbol.SetFlags(SymbolFlags.Global, entry.Global);
        symbol.SetFlags(SymbolFlags.Forward, entry.ForwardDefined && entry.IsDefined);
        symbol.SetFlags(SymbolFlags.Def_Label, entry.Type is SymbolType.Label);

        // Return
        return symbol;
    }
    
    /// <inheritdoc/>
    /// <remarks>
    /// <see cref="CommonEntry.Symbol"/> will be null.
    /// </remarks>
    public CommonEntry Convert()
    {
        // Get type
        var type = SymbolType.Macro;
        if (CheckFlag(SymbolFlags.Def_Label))
            type = SymbolType.Label;

        // Get address and initialize
        var adr = new Address(Value, Section);
        var symbol = new CommonEntry(null!, type, adr); // Null suppress. We're going to overwrite it once we leave this method.
        
        // Set flags
        symbol.Global = CheckFlag(SymbolFlags.Global);
        symbol.ForwardDefined = CheckFlag(SymbolFlags.Forward);

        return symbol;
    }
}
