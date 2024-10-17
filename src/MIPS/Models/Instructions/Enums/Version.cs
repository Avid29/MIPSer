// Adam Dernis 2024

namespace MIPS.Models.Instructions.Enums;

/// <summary>
/// An enum for which version(s) a MIPS feature is supported.
/// </summary>
[Flags]
public enum Version : byte
{
#pragma warning disable CS1591
    All = byte.MaxValue,

    MipsI = 0x1,
    MipsII = 0x2,
    MipsIII = 0x4,
    MipsIV = 0x8,
    MipsV = 0x10,
    MipsVI = 0x20,

    /// <summary>
    /// MIPS I and II.
    /// </summary>
    MipsItoII = MipsI | MipsII,
    
    /// <summary>
    /// MIPS I through V.
    /// </summary>
    MipsItoV = MipsI | MipsII | MipsIII | MipsIV | MipsV,
    
    /// <summary>
    /// MIPS II through V.
    /// </summary>
    MipsIItoV = MipsII | MipsIII | MipsIV | MipsV,
    
    /// <summary>
    /// MIPS II through VI.
    /// </summary>
    MipsIItoVI = MipsII | MipsIII | MipsIV | MipsV | MipsVI,

#pragma warning restore CS1591
}
