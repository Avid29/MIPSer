// Avishai Dernis 2026

using MIPS.Emulator.Models.Enum;
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
    public TrapOccurringEventArgs(TrapKind trap)
    {
        Trap = trap;
    }

    /// <summary>
    /// Gets the trap that occurred.
    /// </summary>
    public TrapKind Trap { get; }
}
