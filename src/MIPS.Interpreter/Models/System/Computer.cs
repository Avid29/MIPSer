// Avishai Dernis 2025

using MIPS.Interpreter.Models.System.Memory;
using MIPS.Interpreter.System.CPU;

namespace MIPS.Interpreter.Models.System;

/// <summary>
/// A class representing a computer system in the MIPS interpreter.
/// </summary>
public class Computer
{
    private readonly RAM _memory;

    /// <summary>
    /// Initializes a new instance of the <see cref="Computer"/> class.
    /// </summary>
    public Computer()
    {
        _memory = new RAM();
        Processor = new Processor(_memory);
    }

    /// <summary>
    /// Gets the processor of the computer system.
    /// </summary>
    public Processor Processor { get; }
}
