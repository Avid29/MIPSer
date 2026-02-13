// Avishai Dernis 2026

using CommunityToolkit.Diagnostics;
using System;
using System.Threading;
using Zarem.Emulator.Config;
using Zarem.Emulator.Models.Enums;
using Zarem.Emulator.Models.Modules;

namespace Zarem.Emulator;

/// <summary>
/// A base class for an emulator.
/// </summary>
/// <typeparam name="TConfig">The emulator configuration info</typeparam>
public abstract class Emulator<TConfig>
    where TConfig : EmulatorConfig
{
    private readonly ManualResetEventSlim _runGate = new(false);
    private Thread? _thread;

    /// <summary>
    /// An event invoked when the emulator state changes.
    /// </summary>
    public EventHandler<EmulatorState>? StateChanged;

    /// <summary>
    /// Gets the state of the emulator.
    /// </summary>
    public EmulatorState State
    {
        get => field;
        set
        {
            field = value;
            StateChanged?.Invoke(this, value);
        }
    } = EmulatorState.Stopped;

    /// <summary>
    /// Loads an <see cref="IExecutableModule"/> to the interpreter's memory.
    /// </summary>
    /// <param name="module">The module to load.</param>
    public abstract void Load(IExecutableModule module);

    /// <summary>
    /// Starts the execution loop for the emulator.
    /// </summary>
    public void Start()
    {
        // Nothing to be done
        if (State is EmulatorState.Running)
            return;

        // Forward to resume
        if (State is EmulatorState.Paused)
        {
            Resume();
            return;
        }

        // Execution can only be started from ready
        Guard.IsTrue(State is EmulatorState.Ready);

        // Create and begin the thread
        _thread = new Thread(ExecutionLoop)
        {
            IsBackground = true,
        };
        _thread.Start();

        // Update the 
        State = EmulatorState.Running;
        _runGate.Set();
    }

    /// <summary>
    /// Resume the execution loop if paused.
    /// </summary>
    public void Resume()
    {
        // Nothing to be done
        if (State is EmulatorState.Running)
            return;

        // Execution can only be resumed from being paused
        Guard.IsTrue(State is EmulatorState.Paused);

        State = EmulatorState.Running;
        _runGate.Set();
    }

    /// <summary>
    /// Stops execution
    /// </summary>
    public void Pause()
    {
        // Schedule pause
        State = EmulatorState.Pausing;
        _runGate.Reset();
    }

    /// <summary>
    /// Shuts down the emulation.
    /// </summary>
    public void ShutDown()
    {
        // Schedule the shutdown
        State = EmulatorState.Stopping;
        _runGate.Set(); // The thread must run to exit
        _thread?.Join();

        // Shutdown complete
        State = EmulatorState.Stopped;
    }

    /// <summary>
    /// Steps a tick in the processor.
    /// </summary>
    protected abstract void Tick();

    private void ExecutionLoop()
    {
        while (State is not EmulatorState.Stopping)
        {
            // Wait here if paused
            _runGate.Wait();

            // Loop ticks while running
            while (State is EmulatorState.Running)
                Tick();

            // Complete pausing transition
            if (State is EmulatorState.Pausing)
                State = EmulatorState.Paused;
        }
    }
}
