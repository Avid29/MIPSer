// Avishai Dernis 2025

using MIPS.Interpreter.Models.Modules;
using MIPS.Interpreter.Models.System;
using MIPS.Models.Instructions;

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
    public uint ProgramCount
    {
        get => _computer.Processor.ProgramCounter;
        set => _computer.Processor.ProgramCounter = value;
    }

    /// <summary>
    /// Runs a single instruction
    /// </summary>
    public void StepInstruction()
    {
        var word = _computer.Memory[_computer.Processor.ProgramCounter];
        var instruction = (Instruction)word;

        _computer.Processor.Execute(instruction);
    }

    /// <summary>
    /// Loads an <see cref="IExecutableModule"/> to the interpreter's memory.
    /// </summary>
    /// <param name="module">The module to load.</param>
    public void Load(IExecutableModule module) => module.Load(_computer.Memory.AsStream());
}
