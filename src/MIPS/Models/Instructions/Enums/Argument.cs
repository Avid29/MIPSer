// Adam Dernis 2023

namespace MIPS.Models.Instructions.Enums;

/// <summary>
/// An enum for potential argument types.
/// </summary>
public enum Argument
{
    #pragma warning disable CS1591

    // Registers
    RS,
    RT,
    RD,

    Shift,
    Immediate,
    Address,
    AddressOffset,

    #pragma warning restore CS1591
}
