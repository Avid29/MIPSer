// Adam Dernis 2024

namespace MIPS.Models.Instructions.Enums;

/// <summary>
/// An enum for which version(s) a MIPS feature is supported.
/// </summary>
[Flags]
public enum Version : byte
{
#pragma warning disable CS1591

    MipsI = 1,
    MipsII = 2, 
    MipsIII = 3,
    MipsIV = 4,
    MipsV = 5,
    MipsVI = 6, 

#pragma warning restore CS1591
}
