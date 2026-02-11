// Avishai Dernis 2026

namespace MIPS.Emulator.Models.Enums;

/// <summary>
/// An enum for the state of the emulator.
/// </summary>
public enum EmulatorState
{

#pragma warning disable CS1591

    Ready,
    Running,
    Pausing,
    Paused,
    Stopping,
    Stopped,

#pragma warning restore CS1591

}
