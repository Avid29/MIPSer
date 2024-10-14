// Adam Dernis 2024

namespace MIPS.Models.Instructions.Enums;

/// <summary>
/// 
/// </summary>
public enum Version
{
#pragma warning disable CS1591
    MipsI = 0x1,
    MipsII = 0x2,
    MipsIII = 0x4,
    MipsIV = 0x8,
    MipsV = 0x10,

    All = MipsI | MipsIIUp,
    MipsIIUp = MipsII | MipsIIIUp,
    MipsIIIUp = MipsIII | MipsIVUp,
    MipsIVUp = MipsIV | MipsVUp,
    MipsVUp = MipsV,

    /// <summary>
    /// Just MIPS I and MIPS II.
    /// </summary>
    MipsIaII = MipsI | MipsII,
#pragma warning restore CS1591
}
