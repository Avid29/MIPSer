// Avishai Dernis 2026

using MIPS.Emulator.Executor.Enum;
using System;

namespace MIPS.Emulator.Events;

/// <summary>
/// The event args for when a trap occurs in emulation.
/// </summary>
public class TrapOccurringEventArgs : EventArgs
{
    /// <summary>
    /// Initializes a new instance of the <see cref="TrapOccurringEventArgs"/> class.
    /// </summary>
    public TrapOccurringEventArgs(TrapKind trap, bool unhandled)
    {
        Trap = trap;
        Unhandled = unhandled;
    }

    /// <summary>
    /// Gets the trap that occurred.
    /// </summary>
    public TrapKind Trap { get; }

    /// <summary>
    /// Gets whether or not the trap will be handled by the emulation.
    /// </summary>
    public bool Unhandled { get; }
}
