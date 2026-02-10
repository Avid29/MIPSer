// Avishai Dernis 2025

using CommunityToolkit.Diagnostics;
using MIPS.Emulator.Components;
using MIPS.Emulator.Models.Enums;
using MIPS.Emulator.Models.Modules;
using System;
using System.Threading;

namespace MIPS.Emulator;

/// <summary>
/// An emulator of a MIPS machine.
/// </summary>
public class Emulator
{
    private readonly ManualResetEventSlim _runGate = new(false);
    private Thread? _thread;

    /// <summary>
    /// An event invoked when the emulator state changes.
    /// </summary>
    public EventHandler<EmulatorState>? StateChanged;

    /// <summary>
    /// Initializes a new instance of the <see cref="Emulator"/> class.
    /// </summary>
    public Emulator()
    {
        Computer = new Computer();
    }

    /// <summary>
    /// Gets the computer system the interpreter is emulating.
    /// </summary>
    public Computer Computer { get; }

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
    public void Load(IExecutableModule module)
    {
        module.Load(Computer.Memory.AsStream());
        State = EmulatorState.Ready;
    }

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

    private void ExecutionLoop()
    {
        while (State is not EmulatorState.Stopping)
        {
            // Wait here if paused
            _runGate.Wait();

            // Loop ticks while running
            while (State is EmulatorState.Running)
                Computer.Tick();

            // Complete pausing transition
            if (State is EmulatorState.Pausing)
                State = EmulatorState.Paused;
        }
    }
}
