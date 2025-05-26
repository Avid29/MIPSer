// Adam Dernis 2025

namespace EFL.Modules.Header.Enums;

/// <summary>
/// An enum for the class field of the ELF header format.
/// It signifies bit count.
/// </summary>
public enum Class : byte
{
    /// <summary>
    /// The executabe is in a 32-bit format.
    /// </summary>
    Bit32 = 1,

    /// <summary>
    /// The executabe is in a 64-bit format.
    /// </summary>
    Bit64 = 2,
}
