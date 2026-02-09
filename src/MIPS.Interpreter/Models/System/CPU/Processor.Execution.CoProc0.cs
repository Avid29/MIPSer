// Avishai Dernis 2025

using MIPS.Interpreter.Models.System.Execution;
using MIPS.Models.Instructions;
using MIPS.Models.Instructions.Enums.Registers;
using MIPS.Models.Instructions.Enums.SpecialFunctions.CoProc0;
using System;

namespace MIPS.Interpreter.System.CPU;

public partial class Processor
{
    private Execution CreateExecutionCoProc0(CoProc0Instruction instruction)
    {
        return instruction.CoProc0RSCode switch
        {
            // Coprocessor instructions
            CoProc0RSCode.C0 => instruction.Co0FuncCode switch
            {
                Co0FuncCode.ExceptionReturn => Eret(),
                _ => throw new NotImplementedException()
            },

            _ => throw new NotImplementedException()
        };
    }

    private Execution Eret()
    {
        // Retrieve the status register value
        var status = CoProcessor0.StatusRegister;

        // Determine the target program counter based on the error level
        uint targetPC = status.ErrorLevel
            ? CoProcessor0[CP0Registers.ErrorEPC]
            : CoProcessor0[CP0Registers.ExceptionPC];

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
        return new Execution
        {
            ProgramCounter = targetPC,
            WriteBack = (uint)status,
            CPR0 = CP0Registers.Status,
        };
    }
}
