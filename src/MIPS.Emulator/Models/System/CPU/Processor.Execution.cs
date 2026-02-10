// Avishai Dernis 2025

using CommunityToolkit.Diagnostics;
using MIPS.Emulator.Executor;
using MIPS.Emulator.Models.System.CPU.Registers;
using MIPS.Emulator.Models.System.Execution;
using MIPS.Emulator.Models.System.Execution.Enum;
using MIPS.Models.Instructions;
using MIPS.Models.Instructions.Enums.Operations;
using MIPS.Models.Instructions.Enums.Registers;
using System;

namespace MIPS.Emulator.System.CPU;

public partial class Processor
{
    /// <summary>
    /// Executes an instruction.
    /// </summary>
    /// <param name="instruction">The instruction to execute.</param>
    /// <param name="trap">The trap that occured, if any.</param>
    public Execution Execute(Instruction instruction, out TrapKind trap)
    {
        // Create the exeuction
        trap = InstructionExecutor.Execute(instruction, this, out var execution);

        // Handle exceptions and traps first, if any
        if (trap is not TrapKind.None)
        {
            // Status and cause registers
            CoProcessor0.StatusRegister = CoProcessor0.StatusRegister with { ExceptionLevel = true };
            CoProcessor0.CauseRegister = CoProcessor0.CauseRegister with
            {
                ExecptionCode = trap,
                //IsBranchDelayed = // TODO: Handle delay slots
            };

            // Track the current program counter in the EPC register
            // before jumping to the exception handler
            CoProcessor0[CP0Registers.ExceptionPC] = ProgramCounter;
            ProgramCounter = CoProcessor0.ExceptionVector;

            // No writebacks. No side-effects. Periodt. Full stop.
            return execution;
        }

        // Handle write base
        RegisterFile[execution.GPR] = execution.WriteBack;

        // Increment the program counter by default, some instructions will override this
        var programCounter = ProgramCounter + 4;

        // Apply side effects
        switch (execution.SideEffects)
        {
            case SecondaryEffect.Low:
                Low = execution.Low;
                break;
            case SecondaryEffect.High:
                High = execution.High;
                break;
            case SecondaryEffect.HighLow:
                (High, Low) = (execution.High, execution.Low);
                break;
            case SecondaryEffect.ProgramCounter:
                programCounter = execution.ProgramCounter;
                break;
            case SecondaryEffect.WriteMemory:
                _computer.Memory[execution.MemAddress] = execution.WriteBack;
                break;
            case SecondaryEffect.WriteCoProc:
                (execution.CoProcRegisterSet switch
                {
                    RegisterSet.GeneralPurpose => RegisterFile,
                    RegisterSet.CoProc0 => CoProcessor0.RegisterFile,
                    _ => throw new ArgumentOutOfRangeException(nameof(execution.CoProcRegisterSet)),
                })[execution.CoProcReg] = execution.CoProcWriteBack;
                break;
        }

        // Apply the program counter update
        ProgramCounter = programCounter;

        return execution;
    }
}
