// Avishai Dernis 2025

using MIPS.Emulator.Models.System.Memory;
using MIPS.Emulator.System.CPU;

namespace MIPS.Emulator.Models.System;

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
        Memory = new SystemMemory();
        Processor = new Processor(this);
    }

    /// <summary>
    /// Gets the processor of the computer system.
    /// </summary>
    public Processor Processor { get; }

    /// <summary>
    /// Gets the memory of the computer system.
    /// </summary>
    public SystemMemory Memory { get; }
}
