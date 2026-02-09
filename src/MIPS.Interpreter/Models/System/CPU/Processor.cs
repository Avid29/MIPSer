// Avishai Dernis 2025

using MIPS.Interpreter.Models.System;
using MIPS.Interpreter.Models.System.CPU.CoProcessors;
using MIPS.Interpreter.Models.System.CPU.Registers;
using MIPS.Interpreter.Models.System.Memory;
using MIPS.Models.Instructions.Enums.Registers;

namespace MIPS.Interpreter.System.CPU;

/// <summary>
/// A class representing a processor unit.
/// </summary>
public partial class Processor
{
    private readonly Computer _computer;
    private readonly RegisterFile _regFile;

    /// <summary>
    /// Initializes a new instance of the <see cref="Processor"/> class.
    /// </summary>
    public Processor(Computer computer)
    {
        _computer = computer;

        _regFile = new RegisterFile();
        CoProcessor0 = new CoProcessor0();
    }

    /// <summary>
    /// Gets or sets the value in the high register.
    /// </summary>
    public (uint High, uint Low) HighLow { get; set; }

    /// <summary>
    /// Gets or sets the value in the program counter register.
    /// </summary>
    public uint ProgramCounter { get; set; }

    /// <summary>
    /// Gets the coprocessor 0 unit of the computer system.
    /// </summary>
    public CoProcessor0 CoProcessor0 { get; }

    /// <summary>
    /// Gets or sets the value of a general-purpose register on the processor.
    /// </summary>
    /// <param name="reg">The register to get or set.</param>
    /// <returns>The value of the register.</returns>
    public uint this[GPRegister reg]
    {
        get => _regFile[reg];
        set => _regFile[reg] = value;
    }
}
