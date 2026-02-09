// Avishai Dernis 2025

using MIPS.Interpreter.Models.Modules;
using MIPS.Interpreter.Models.System;
using MIPS.Interpreter.Models.System.Execution;
using MIPS.Interpreter.Models.System.Execution.Enum;
using MIPS.Models.Instructions;
using MIPS.Models.Instructions.Enums.Registers;

namespace MIPS.Interpreter;

/// <summary>
/// A MIPS Interpreter.
/// </summary>
public class Interpreter
{
    /// <summary>
    /// Intializes a new instance of the <see cref="Interpreter"/> class.
    /// </summary>
    public Interpreter()
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

        Computer.Processor.Execute(instruction);
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
    public void InsertInstructionExecution(Instruction instruction, out Execution execution)
    {
        var oldPC = Computer.Processor.ProgramCounter;
        execution = Computer.Processor.Execute(instruction);

        // If the instruction execution did not explicitly handle the program counter, restore it to the old value.
        if (!execution.PCHandled)
        {
            Computer.Processor.ProgramCounter = oldPC;
        }
    }
}
