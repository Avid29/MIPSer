// Avishai Dernis 2026

using Zarem.Emulator.Models.Enums;
using Zarem.Emulator.Models.Modules;

namespace Zarem.Emulator;

/// <summary>
/// An interface for an emulator.
/// </summary>
public interface IEmulator
{
    /// <summary>
    /// Gets the state of the emulator.
    /// </summary>
    EmulatorState State { get; set; }

    /// <summary>
    /// Loads an <see cref="IExecutableModule"/> to the interpreter's memory.
    /// </summary>
    /// <param name="module">The module to load.</param>
    void Load(IExecutableModule module);

    /// <summary>
    /// Starts the execution loop for the emulator.
    /// </summary>
    void Start();

    /// <summary>
    /// Resume the execution loop if paused.
    /// </summary>
    void Resume();

    /// <summary>
    /// Stops execution
    /// </summary>
    void Pause();

    /// <summary>
    /// Shuts down the emulation.
    /// </summary>
    void ShutDown();
}
