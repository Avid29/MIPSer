// Adam Dernis 2025

namespace MIPS.Models.Instructions.Enums.SpecialFunctions;

/// <summary>
/// An enum for <see cref="CoProc0RS.MFMC0"/> instruction function codes.
/// </summary>
public enum MFMC0FuncCode
{
    #pragma warning disable CS1591

    DisableInterupts = 0x0,

    EnableVirtualProcessor = 0x4,

    EnableInterupts = 0x20,

    #pragma warning restore CS1591
}
