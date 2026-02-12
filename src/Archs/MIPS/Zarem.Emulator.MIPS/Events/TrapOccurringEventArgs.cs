// Avishai Dernis 2026

using System;
using System.Threading;
using Zarem.Emulator.Executor.Enum;

namespace Zarem.Emulator.Events;

/// <summary>
/// The event args for when a trap occurs in emulation.
/// </summary>
public class TrapOccurringEventArgs : EventArgs
{
    private readonly ManualResetEventSlim? _resumeEvent;

    /// <summary>
    /// Initializes a new instance of the <see cref="TrapOccurringEventArgs"/> class.
    /// </summary>
    public TrapOccurringEventArgs(TrapKind trap, bool unhandled)
    {
        Trap = trap;
        Unhandled = unhandled;

        if (Unhandled)
        {
            _resumeEvent = new(false);
        }
    }

    /// <summary>
    /// Gets the trap that occurred.
    /// </summary>
    public TrapKind Trap { get; }

    /// <summary>
    /// Gets whether or not the trap will be handled by the emulation.
    /// </summary>
    public bool Unhandled { get; }

    /// <summary>
    /// Mark the trap handled, and resume the emulator.
    /// </summary>
    public void Resume() => _resumeEvent?.Set();

    internal void Wait() => _resumeEvent?.Wait();
}
