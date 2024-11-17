// Adam Dernis 2024

using MIPS.Assembler.Models.Modules.Interfaces.Tables;
using MIPS.Helpers;
using MIPS.Models.Addressing;
using MIPS.Models.Addressing.Enums;
using RASM.Modules.Tables.Enums;
using System.Runtime.InteropServices;

using CommonEntry = MIPS.Models.Modules.Tables.RelocationEntry;
using CommonType = MIPS.Models.Modules.Tables.Enums.ReferenceType;

namespace RASM.Modules.Tables;

/// <summary>
/// An entry in the RASM load module's relocation table.
/// </summary>
[StructLayout(LayoutKind.Explicit)]
public struct RelocationEntry : IRelocationEntry<RelocationEntry>, IBigEndianReadWritable<RelocationEntry>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="RelocationEntry"/> struct.
    /// </summary>
    public RelocationEntry(Address address, RelocationType type) : this((uint)address.Value, address.Section, type)
    {
    }

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

    /// <inheritdoc/>
    public readonly RelocationEntry Read(Stream stream)
    {
        stream.TryRead<uint>(out var address);
        stream.TryRead<byte>(out var section);
        stream.TryRead<byte>(out var type);
        stream.TryRead<ushort>(out _);

        return new(address, (Section)section, (RelocationType)type);
    }
    
    /// <inheritdoc/>
    public readonly void Write(Stream stream)
    {
        stream.TryWrite(Address);
        stream.TryWrite((byte)Section);
        stream.TryWrite((byte)Type);
        stream.TryWrite(_filler);
    }
    
    /// <inheritdoc/>
    public static RelocationEntry Convert(CommonEntry entry)
    {
        var type = entry.Type switch
        {
            CommonType.FullWord => RelocationType.FullWord,
            CommonType.Lower => RelocationType.SimpleImmediate,
            CommonType.Address => RelocationType.Address,
            _ => RelocationType.SimpleImmediate,
        };

        return new RelocationEntry(entry.Address, type);
    }
}
