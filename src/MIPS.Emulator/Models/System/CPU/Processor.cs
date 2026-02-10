// Avishai Dernis 2025

using MIPS.Emulator.Executor;
using MIPS.Emulator.Models;
using MIPS.Emulator.Models.System;
using MIPS.Emulator.Models.System.CPU.CoProcessors;
using MIPS.Emulator.Models.System.CPU.Registers;
using MIPS.Emulator.Models.System.Execution;
using MIPS.Emulator.Models.System.Execution.Enum;
using MIPS.Models.Instructions;
using MIPS.Models.Instructions.Enums.Registers;

namespace MIPS.Emulator.System.CPU;

/// <summary>
/// A class representing a processor unit.
/// </summary>
public partial class Processor
{
    private readonly Computer _computer;

    /// <summary>
    /// Initializes a new instance of the <see cref="Processor"/> class.
    /// </summary>
    public Processor(Computer computer)
    {
        _computer = computer;

        RegisterFile = new RegisterFile(true);
        CoProcessor0 = new CoProcessor0();
    }

    internal RegisterFile RegisterFile { get; }

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
        get => RegisterFile[reg];
        set => RegisterFile[reg] = value;
    }

    /// <summary>
    /// Gets or sets the value in the low register.
    /// </summary>
    public uint Low { get; set; }

    /// <summary>
    /// Gets or sets the value in the high register.
    /// </summary>
    public uint High { get; set; }

    /// <summary>
    /// Advances the state of the emulator by one step.
    /// </summary>
    public void Step()
    {
        // Pre-define everything to avoid unset variable accusations
        TrapKind trap = TrapKind.None;
        Instruction instruction = default;

        // Immitate the MIPS instruction pipeline
        // Fetch, Decode, Execute, Mem, WriteBack
        // Except Decode is handled by a simple cast here
        trap = trap is not TrapKind.None ? Fetch(out instruction) : trap;
        trap = trap is not TrapKind.None ? ExecuteAndApply(instruction, out _) : trap;

        // Handle trap, if any occured
        if (trap is not TrapKind.None)
        {

        }
    }

    /// <summary>
    /// Executes an instruction on the current state of the processor.
    /// </summary>
    public void Insert(Instruction instruction, out Execution execution, out TrapKind trap)
        => trap = ExecuteAndApply(instruction, out execution);

    /// <remarks>
    /// Immitates the fetch step in a MIPS cpu, reading an instruction from memory.
    /// </remarks>
    private TrapKind Fetch(out Instruction instruction)
    {
        instruction = default;

        if (ProgramCounter % 4 is not 0)
        {
            return TrapKind.AddressErrorLoad;
        }

        instruction = (Instruction)_computer.Memory.Read<uint>(ProgramCounter);
        return TrapKind.None;
    }

    /// <remarks>
    /// Wraps the last 3 stages of the instruction pipeline.
    /// This allows for executing instructions that were not fetched.
    /// </remarks>
    private TrapKind ExecuteAndApply(Instruction instruction, out Execution execution)
    {
        // Pre-define everything to avoid unset variable accusations
        TrapKind trap = TrapKind.None;
        uint memRead = default;
        execution = default;

        trap = trap is not TrapKind.None ? Execute(instruction, out execution) : trap;
        trap = trap is not TrapKind.None ? MemAccess(execution, out memRead) : trap;
        trap = trap is not TrapKind.None ? WriteBack(execution, memRead) : trap;

        // Handle trap, if any occured
        if (trap is not TrapKind.None)
        {

        }

        return trap;
    }

    /// <summary>
    /// Immitates the execute step in a MIPS cpu, constructing the modifications to apply in the following stages.
    /// </summary>
    private TrapKind Execute(Instruction instruction, out Execution execution)
        => InstructionExecutor.Execute(instruction, this, out execution);

    private TrapKind MemAccess(Execution execution, out uint read)
    {
        read = default;

        return TrapKind.None;
    }

    private TrapKind WriteBack(Execution execution, uint memRead)
    {
        return TrapKind.None;
    }
}
