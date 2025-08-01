﻿// Avishai Dernis 2025

using MIPS.Interpreter.Models.System.CPU.Registers;
using MIPS.Interpreter.Models.System.Memory;
using MIPS.Models.Instructions.Enums.Registers;

namespace MIPS.Interpreter.System.CPU;

/// <summary>
/// A class representing a processor unit.
/// </summary>
public partial class Processor
{
    private readonly RAM _memory;
    private readonly RegisterFile _regFile;

    /// <summary>
    /// Gets or sets the value in the high register.
    /// </summary>
    public uint High { get ; set; }

    /// <summary>
    /// Gets or sets the value in the low register.
    /// </summary>
    public uint Low { get ; set; }

    /// <summary>
    /// Gets or sets the value in the program counter register.
    /// </summary>
    public uint ProgramCounter { get; set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="Processor"/> class.
    /// </summary>
    public Processor(RAM memory)
    {
        _regFile = new RegisterFile();
        _memory = memory;
    }

    private void Jump(uint address)
        => ProgramCounter = address;

    private void JumpPartial(uint address)
    {
        // TODO: Maintain paging
        Jump(address * 4);
    }

    private void JumpOffset(short offset)
    {
        ProgramCounter = (uint)((int)ProgramCounter + offset);
    }

    private void Link()
    {
        // Assign return address to instruction after caller location 
        _regFile[Register.ReturnAddress] = ProgramCounter + 4;
    }
}
