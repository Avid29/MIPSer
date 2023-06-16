// Adam Dernis 2023

namespace MIPS.Models.Addressing.Enums;

/// <summary>
/// An enum marking the section an address belongs to.
/// </summary>
public enum Section : byte
{
#pragma warning disable CS1591
    
    Text,                   // text
    ReadOnlyData,           // rdata
    Data,                   // data
    SmallInitializedData,   // sdata
    SmallUninitializedData, // sbss
    UninitializedData,      // bss

    // Meta Sections
    RelocationTable,
    ReferenceTable,
    SymbolTable,
    SymbolNameTable,


#pragma warning restore CS1591
}
