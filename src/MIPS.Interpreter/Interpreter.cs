// Avishai Dernis 2025

using MIPS.Interpreter.Models.Modules;
using MIPS.Interpreter.Models.System;

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
    }
}
