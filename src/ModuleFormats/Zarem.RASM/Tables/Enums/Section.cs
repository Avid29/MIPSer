// Adam Dernis 2024

namespace Zarem.RASM.Tables.Enums;

/// <summary>
/// An enum marking the section an address belongs to.
/// </summary>
public enum Section : byte
{
#pragma warning disable CS1591
    
    // Standard sections
    Text,                   // .text
    ReadOnlyData,           // .rdata
    Data,                   // .data
    SmallInitializedData,   // .sdata
    SmallUninitializedData, // .sbss
    UninitializedData,      // .bss

    // Meta Sections
    RelocationTable,
    ReferenceTable,
    SymbolTable,
    StringTable,

    // RAM
    Heap,
    Stack,

    // Global
    Absolute,
    External,
    Unknown,
    None,

#pragma warning restore CS1591
}
