// Avishai Dernis 2026

using MIPS.Emulator.Models.System.Execution;
using MIPS.Emulator.Models.System.Execution.Enum;
using MIPS.Models.Instructions;
using MIPS.Models.Instructions.Enums.Registers;
using MIPS.Models.Instructions.Enums.SpecialFunctions.CoProc0;
using System;

namespace MIPS.Emulator.Executor;

/// <summary>
/// A class which handles converting decoded instructions into <see cref="Execution"/> models.
/// </summary>
public partial class InstructionExecutor
{
    private Execution CreateCo0Execution()
    {
        // Check if the current privilege mode allows executing coprocessor instructions
        // NOTE: Make mfc0 permissions in user mode configurable?
        if (Processor.CoProcessor0.PrivilegeMode is not PrivilegeMode.Kernel)
            return CreateTrap(TrapKind.ReservedInstruction);

        return CoProc0Instruction.CoProc0RSCode switch
        {
            // C0 Instructions
            CoProc0RSCode.C0 => CoProc0Instruction.Co0FuncCode switch
            {
                Co0FuncCode.ExceptionReturn => Eret(),

                _ => throw new NotImplementedException()
            },

            // MFMC0 Instructions
            CoProc0RSCode.MFMC0 => CoProc0Instruction.MFMC0FuncCode switch
            {
                MFMC0FuncCode.EnableInterupts => StatusUpdateInstruction((ref status) => status.InteruptEnabled = true),
                MFMC0FuncCode.DisableInterupts => StatusUpdateInstruction((ref status) => status.InteruptEnabled = false),

                _ => throw new NotImplementedException()
            },

            // Move instructions
            CoProc0RSCode.MFC0 => new Execution(Instruction.RT, Processor.CoProcessor0[(CP0Registers)Instruction.RD]),
            CoProc0RSCode.MTC0 => new Execution((CP0Registers)Instruction.RD, RT),

            _ => throw new NotImplementedException()
        };
    }

    private Execution Eret()
    {
        // Retrieve the status register value
        var status = Processor.CoProcessor0.StatusRegister;

        // Determine the target program counter based on the error level
        uint targetPC = status.ErrorLevel
            ? Processor.CoProcessor0[CP0Registers.ErrorEPC]
            : Processor.CoProcessor0[CP0Registers.ExceptionPC];

        // Clear the appropriate level bit in the status register
        if (status.ErrorLevel)
        {
            status.ErrorLevel = false;
        }
        else
        {
            status.ExceptionLevel = false;
        }

        // TODO: Explorer special commit phase to avoid setting
        // the status register as a writeback
        return new Execution(CP0Registers.Status, (uint)status)
        {
            ProgramCounter = targetPC
        };
    }

    private Execution StatusUpdateInstruction(StatusUpdateDelegate func)
    {
        // Retrieve the status register
        var status = Processor.CoProcessor0.StatusRegister;

        // Apply the update function
        func(ref status);

        if (Instruction.RT is not GPRegister.Zero)
        {
            // Write the updated status register value back to the specified GPR
            return new Execution(CP0Registers.Status, (uint)status)
            {
                RTDump = (uint)status,
            };
        }

        return new Execution(CP0Registers.Status, (uint)status);
    }
}
