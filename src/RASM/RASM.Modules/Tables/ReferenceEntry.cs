// Adam Dernis 2024

using MIPS.Assembler.Models.Modules.Interfaces.Tables;
using MIPS.Models.Addressing;
using MIPS.Models.Addressing.Enums;
using RASM.Modules.Tables.Enums;
using System.Runtime.InteropServices;

using CommonEntry = MIPS.Models.Modules.Tables.ReferenceEntry;
using CommonType = MIPS.Models.Modules.Tables.Enums.ReferenceType;
using ReferenceMethod = MIPS.Models.Modules.Tables.Enums.ReferenceMethod;

namespace RASM.Modules.Tables;

/// <summary>
/// An entry in the RASM load module's reference table.
/// </summary>
[StructLayout(LayoutKind.Explicit)]
public struct ReferenceEntry : IReferenceEntry<ReferenceEntry>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="RelocationEntry"/> struct.
    /// </summary>
    public ReferenceEntry(Address address, ReferenceType type)
    {
        Address = (uint)address.Value;
        Section = address.Section;
        Type = type;
    }

    /// <summary>
    /// Gets or sets the location of reference, within its section.
    /// </summary>
    [field: FieldOffset(0)]
    public uint Address { get; set; }

    /// <summary>
    /// Gets or sets the index of the symbol name in the string table.
    /// </summary>
    [field: FieldOffset(4)]
    public uint SymbolIndex { get; set; }

    /// <summary>
    /// Gets or sets the section the <see cref="Address"/> belongs in.
    /// </summary>
    [field: FieldOffset(8)]
    public Section Section { get; set; }

    /// <summary>
    /// Gets or sets a <see cref="ReferenceType"/> describing how to preform the reference.
    /// </summary>
    [field: FieldOffset(9)]
    public ReferenceType Type { get; set; }

    /// <summary>
    /// Gets or sets the index of the module containing the reference.
    /// </summary>
    [field: FieldOffset(10)]
    public ushort ModuleIndex { get; set; }

    /// <inheritdoc/>
    public readonly ReferenceEntry Read(Stream stream)
    {
        stream.TryRead<uint>(out var address);
        stream.TryRead<uint>(out var symbolIndex);
        stream.TryRead<byte>(out var section);
        stream.TryRead<byte>(out var type);
        stream.TryRead<ushort>(out var moduleIndex);

        return new()
        {
            Address = address,
            SymbolIndex = symbolIndex,
            Section = (Section)section,
            Type = (ReferenceType)type,
            ModuleIndex = moduleIndex,
        };
    }
    
    /// <inheritdoc/>
    public readonly void Write(Stream stream)
    {
        stream.TryWrite(Address);
        stream.TryWrite(SymbolIndex);
        stream.TryWrite((byte)Section);
        stream.TryWrite((byte)Type);
        stream.TryWrite(ModuleIndex);
    }
    
    /// <inheritdoc/>
    public static ReferenceEntry Convert(CommonEntry entry)
    {
        // Get reference type
        var type = entry.Type switch
        {
            CommonType.FullWord => ReferenceType.FullWord,
            CommonType.Lower => ReferenceType.SimpleImmediate,
            CommonType.Address => ReferenceType.Address,
            _ => ReferenceType.SimpleImmediate,
        };

        // Get reference method
        var method = entry.Method switch
        {
            ReferenceMethod.Add => ReferenceType.Add,
            ReferenceMethod.Replace => ReferenceType.Replace,
            ReferenceMethod.Subtract => ReferenceType.Subtract,
            _ => ReferenceType.Replace,
        };

        // Construct new entry
        return new ReferenceEntry(entry.Address, type | method);
    }
}
