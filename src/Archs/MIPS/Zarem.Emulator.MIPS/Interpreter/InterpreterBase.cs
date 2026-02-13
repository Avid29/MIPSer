// Avishai Dernis 2026

using Zarem.Emulator.Machine;
using Zarem.Emulator.Machine.CPU;
using Zarem.Emulator.Events;
using Zarem.Emulator.Executor.Enum;
using Zarem.Models.Instructions.Enums.Registers;

namespace Zarem.Emulator.Interpreter;

/// <summary>
/// An interface for an interpreter, which handles traps as the host-layer
/// </summary>
public abstract class InterpreterBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="InterpreterBase"/> class
    /// </summary>
    public InterpreterBase(MIPSComputer computer)
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
    protected MIPSComputer Computer { get; }

    /// <summary>
    /// Gets the value of first argument register.
    /// </summary>
    protected uint A0 => Computer.Processor[GPRegister.Argument0];

    /// <summary>
    /// Gets the value of first argument register.
    /// </summary>
    protected uint A1 => Computer.Processor[GPRegister.Argument1];

    /// <summary>
    /// Gets the value of first argument register.
    /// </summary>
    protected uint A2 => Computer.Processor[GPRegister.Argument2];

    /// <summary>
    /// Gets the value of first argument register.
    /// </summary>
    protected uint A3 => Computer.Processor[GPRegister.Argument3];

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
