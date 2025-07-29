// Avishai Dernis 2025

namespace MIPS.Models.Instructions.Enums;

/// <summary>
/// An enum for potential argument types.
/// </summary>
public enum Argument
{
    #pragma warning disable CS1591

    // General Registers
    RS,
    RT,
    RD,

    Shift,

    /// <summary>
    /// The 16 bit immediate value.
    /// </summary>
    Immediate,

    /// <summary>
    /// A branch's offset.
    /// </summary>
    Offset,

    /// <summary>
    /// The 26-bit immediate value.
    /// </summary>
    Address,

    /// <summary>
    /// An base memory address from a register, and a 16-bit offset.
    /// </summary>
    AddressBase,

    /// <summary>
    /// A 32 bit immediate value
    /// </summary>
    FullImmediate,

    // Floating Point Registers
    FS,
    FT,
    FD,

    // RT Register argument for coprocessors. Must use numbered register name.
    RT_Numbered,

    #pragma warning restore CS1591
}
