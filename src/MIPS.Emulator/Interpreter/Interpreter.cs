// Avishai Dernis 2026

using MIPS.Emulator.Components;
using MIPS.Emulator.Components.CPU;
using MIPS.Emulator.Events;
using MIPS.Emulator.Executor.Enum;
using MIPS.Models.Instructions.Enums.Registers;

namespace MIPS.Emulator.Interpreter;

/// <summary>
/// An interface for an interpreter, which handles traps as the host-layer
/// </summary>
public abstract class InterpreterBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="InterpreterBase"/> class
    /// </summary>
    public InterpreterBase(Computer computer)
    {
        Computer = computer;

        // Register for the trap event
        Computer.Processor.TrapOccurring += Processor_TrapOccurring;
    }

    /// <summary>
    /// Finalizes an instance of the <see cref="InterpreterBase"/> class.
    /// </summary>
    ~InterpreterBase()
    {
        // Unregister the trap event
        Computer.Processor.TrapOccurring -= Processor_TrapOccurring;
    }

    /// <summary>
    /// Gets the computer the traps occur on.
    /// </summary>
    protected Computer Computer { get; }

    /// <summary>
    /// A method to direct a syscall to the appropriate interpretation.
    /// </summary>
    /// <param name="code"></param>
    protected abstract void HandleSyscall(uint code);

    /// <summary>
    /// A method to direct trap handling.
    /// </summary>
    /// <param name="trap">The type of trap that occurred.</param>
    protected abstract void HandleTrap(TrapKind trap);

    private void Processor_TrapOccurring(Processor sender, TrapOccurringEventArgs e)
    {
        // The emulator is handling the trap
        // No need to interpret
        if (!e.Unhandled)
            return;

        if (e.Trap is TrapKind.Syscall)
        {
            HandleSyscall(sender.RegisterFile[GPRegister.ReturnValue0]);
        }
    }
}
