// Adam Dernis 2024

namespace MIPS.Models.Instructions.Enums;

/// <summary>
/// An enum for potential argument types.
/// </summary>
public enum Argument
{
    /// <summary>
    /// The rs register.
    /// </summary>
    RS,

    /// <summary>
    /// The rt register.
    /// </summary>
    RT,

    /// <summary>
    /// The rd register.
    /// </summary>
    RD,

    /// <summary>
    /// The shift component.
    /// </summary>
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
    /// An memory address from a register, and a 16-bit offset.
    /// </summary>
    AddressOffset,

    /// <summary>
    /// A 32 bit immediate value
    /// </summary>
    FullImmediate,
}
