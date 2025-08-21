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
    private readonly IExecutableModule _module;
    private readonly Computer _computer;

    /// <summary>
    /// Intializes a new instance of the <see cref="Interpreter"/> class.
    /// </summary>
    public Interpreter(IExecutableModule module)
    {
        _module = module;
        _computer = new Computer();

        // Initialize memory and pc
        Load();
        _computer.Processor.ProgramCounter = _module.EntryAdress;
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

    private void Load()
    {
        var source = _module.Contents;
        source.Position = 0;
        while (source.Position < source.Length)
        {
            var destination = _computer.Memory.AsStream((uint)source.Position);
            source.CopyTo(destination, (int)long.Min(destination.Length, source.Length - source.Position));
        }
    }
}
