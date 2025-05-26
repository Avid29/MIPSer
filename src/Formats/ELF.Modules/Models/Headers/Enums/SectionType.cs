// Adam Dernis 2025

namespace ELF.Modules.Models.Headers.Enums;

/// <summary>
/// An enum for the type field of the ELF section header format.
/// </summary>
public enum SectionType : uint
{
    #pragma warning disable CS1591

    Null = 0,
    ProgramData,
    SymbolTable,
    StringTable,
    RelocationsWA,
    SymbolHashTable,
    DynamicLinkInfo,
    Notes,
    BlockStartingSpace,
    Relocations,
    SharedLibrary,
    DynamicLinkerSymbolTable,
    ConstructorArray,
    DestructorArray,
    PreConstructors,
    SectionGroup,
    ExtendedSectionIndicies,
    Number,

    #pragma warning restore CS1591
}
