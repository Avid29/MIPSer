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
    private readonly Computer _computer;

    /// <summary>
    /// Intializes a new instance of the <see cref="Interpreter"/> class.
    /// </summary>
    public Interpreter()
    {
        _computer = new Computer();
    }

    /// <summary>
    /// Gets or sets the program counter location
    /// </summary>
    public uint ProgramCounter
    {
        get => _computer.Processor.ProgramCounter;
        set => _computer.Processor.ProgramCounter = value;
    }

    /// <summary>
    /// Runs a single instruction
    /// </summary>
    public void StepInstruction()
    {
        var word = _computer.Memory[ProgramCounter];
        var instruction = (Instruction)word;

        _computer.Processor.Execute(instruction);
    }

    /// <summary>
    /// Loads an <see cref="IExecutableModule"/> to the interpreter's memory.
    /// </summary>
    /// <param name="module">The module to load.</param>
    public void Load(IExecutableModule module) => module.Load(_computer.Memory.AsStream());

    /// <summary>
    /// Inserts an instruction to execute into the interpreter's current state.
    /// </summary>
    /// <param name="instruction">The instruction to execute</param>
    /// <param name="execution">The execution details from the instruction.</param>
    public void InsertInstructionExecution(Instruction instruction, out Execution execution)
    {
        var oldPC = ProgramCounter;
        execution = _computer.Processor.Execute(instruction);

        // If the instruction execution did not explicitly handle the program counter, restore it to the old value.
        if (!execution.PCHandled)
        {
            ProgramCounter = oldPC;
        }
    }

    /// <summary>
    /// Gets the specified general-purpose register's value.
    /// </summary>
    /// <param name="register"></param>
    /// <returns></returns>
    public uint GetRegister(GPRegister register)
        => _computer.Processor.RegisterFile[register];

    /// <summary>
    /// Sets the specified general-purpose register to the given value.
    /// </summary>
    /// <param name="register">The general-purpose register to update.</param>
    /// <param name="value">The value to assign to the specified register.</param>
    public void SetRegister(GPRegister register, uint value)
        => _computer.Processor.RegisterFile[register] = value;
}
