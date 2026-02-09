// Avishai Dernis 2026

namespace MIPS.Interpreter.Models.System.Execution.Enum;

/// <summary>
/// An enum describing the kind of trap that occurred during an <see cref="Execution"/>.
/// </summary>
public enum TrapKind
{
    /// <summary>
    /// No trap occurred.
    /// </summary>
    None,

    /// <summary>
    /// A syscall was made.
    /// </summary>
    Syscall,

    /// <summary>
    /// A break was requested.
    /// </summary>
    Break,

    /// <summary>
    /// An arithmetic overflow occurred.
    /// </summary>
    ArithmeticOverflow,
}
