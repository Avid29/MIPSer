// Avishai Dernis 2025

using MIPS.Emulator.Models.Modules;
using MIPS.Emulator.Models.System;
using MIPS.Emulator.Models.System.Execution;
using MIPS.Emulator.Models.System.Execution.Enum;
using MIPS.Models.Instructions;

namespace MIPS.Emulator;

/// <summary>
/// An emulator of a MIPS machine.
/// </summary>
public class Emulator
{
    /// <summary>
    /// Intializes a new instance of the <see cref="Emulator"/> class.
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
    /// Runs a single instruction
    /// </summary>
    public void StepInstruction()
    {
        var word = Computer.Memory[Computer.Processor.ProgramCounter];
        var instruction = (Instruction)word;

        Computer.Processor.Execute(instruction, out _);
    }

    /// <summary>
    /// Loads an <see cref="IExecutableModule"/> to the interpreter's memory.
    /// </summary>
    /// <param name="module">The module to load.</param>
    public void Load(IExecutableModule module) => module.Load(Computer.Memory.AsStream());

    /// <summary>
    /// Inserts an instruction to execute into the interpreter's current state.
    /// </summary>
    /// <param name="instruction">The instruction to execute</param>
    /// <param name="execution">The execution details from the instruction.</param>
    /// <param name="trap">The trap that occured, if any.</param>
    public void InsertInstructionExecution(Instruction instruction, out Execution execution, out TrapKind trap)
    {
        var oldPC = Computer.Processor.ProgramCounter;
        execution = Computer.Processor.Execute(instruction, out trap);

        // If the instruction execution did not explicitly handle the program counter, restore it to the old value.
        if (!execution.PCHandled)
        {
            Computer.Processor.ProgramCounter = oldPC;
        }
    }
}
