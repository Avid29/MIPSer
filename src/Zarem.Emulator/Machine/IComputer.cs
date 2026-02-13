// Avishai Dernis 2026

namespace Zarem.Emulator.Machine;

/// <summary>
/// An interface for a computer system used for emulation.
/// </summary>
public interface IComputer
{
    /// <summary>
    /// Advance the computer one tick.
    /// </summary>
    /// <remarks>
    /// NOTE:
    /// Machines don't work on one big clock. 
    /// Ideally the processor, devices, and everything should be running on seperate threads.
    /// </remarks>
    public void Tick();
}
