// Adam Dernis 2025

namespace ELF.Modules.Models.Enums;

/// <summary>
/// An enum for relocation types in the ELF format.
/// </summary>
public enum RelocationTypes : byte
{
    #pragma warning disable CS1591
    None = 0,       // No relocation
    Absolute = 1,   // Absolute relocation (symbol + offset)
    PCRelative = 2, // PC-relative relocation (symbol + offset - pc)
#pragma warning restore CS1591
}
