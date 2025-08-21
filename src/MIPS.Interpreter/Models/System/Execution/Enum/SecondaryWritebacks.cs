// Avishai Dernis 2025

namespace MIPS.Interpreter.Models.System.Execution.Enum;

/// <summary>
/// An enum describing the secondary writebacks of an <see cref="Execution"/>.
/// </summary>
public enum SecondaryWritebacks
{
    /// <summary>
    /// No secondary writebacks.
    /// </summary>
    None,

    /// <summary>
    /// Writes to the low register.
    /// </summary>
    /// <remarks>
    /// Not flagged.
    /// </remarks>
    Low = 0x1,

    /// <summary>
    /// Writes to the high register.
    /// </summary>
    /// <remarks>
    /// Not flagged.
    /// </remarks>
    High = 0x2,

    /// <summary>
    /// Writes to both the low and high register.
    /// </summary>
    HighLow = Low | High,

    /// <summary>
    /// Writes to the program counter.
    /// </summary>
    ProgramCounter,

    /// <summary>
    /// Writes to memory.
    /// </summary>
    Memory,
}
