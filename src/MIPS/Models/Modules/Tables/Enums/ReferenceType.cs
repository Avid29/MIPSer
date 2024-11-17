// Adam Dernis 2024

namespace MIPS.Models.Modules.Tables.Enums;

/// <summary>
/// An enum for where to update references and relocations.
/// </summary>
public enum ReferenceType
{
    /// <summary>
    /// Update all 32 bits.
    /// </summary>
    FullWord = 0x00,

    /// <summary>
    /// Update the lower 16 bits.
    /// </summary>
    Lower = 0x01,

    /// <summary>
    /// Update the lower 26-bits.
    /// </summary>
    Address = 0x02,
}
