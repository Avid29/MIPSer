// Avishai Dernis 2025

using MIPS.Interpreter.Models.System.CPU.CoProcessors;
using MIPS.Interpreter.Models.System.Memory;
using MIPS.Interpreter.System.CPU;

namespace MIPS.Interpreter.Models.System;

/// <summary>
/// A class representing a computer system in the MIPS interpreter.
/// </summary>
public class Computer
{
    /// <summary>
    /// Initializes a new instance of the <see cref="Computer"/> class.
    /// </summary>
    public Computer()
    {
        Memory = new RAM();
        Processor = new Processor(this);
    }

    /// <summary>
    /// Gets the processor of the computer system.
    /// </summary>
    public Processor Processor { get; }

    /// <summary>
    /// Gets the memory of the computer system.
    /// </summary>
    public RAM Memory { get; }
}
