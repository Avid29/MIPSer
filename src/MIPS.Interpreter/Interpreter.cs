// Avishai Dernis 2025

using MIPS.Interpreter.Models.Modules;
using MIPS.Interpreter.Models.System;
using MIPS.Models.Instructions;
using System.IO;

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

        _computer.Processor.ProgramCounter = _module.EntryAdress;
    }

    /// <summary>
    /// Runs a single instruction
    /// </summary>
    public void StepInstruction()
    {
        _module.Contents.Seek(_computer.Processor.ProgramCounter, SeekOrigin.Begin);
        if(!_module.Contents.TryRead(out uint word))
            return;

        var instruction = (Instruction)word;
        _computer.Processor.Execute(instruction);
    }
}
