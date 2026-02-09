// Avishai Dernis 2025

using CommunityToolkit.Diagnostics;
using MIPS.Interpreter.Models.System.CPU.Registers;
using MIPS.Interpreter.Models.System.Execution;
using MIPS.Interpreter.Models.System.Execution.Enum;
using MIPS.Models.Instructions;
using MIPS.Models.Instructions.Enums.Operations;
using MIPS.Models.Instructions.Enums.Registers;
using MIPS.Models.Instructions.Enums.SpecialFunctions;
using System;
using System.Numerics;

namespace MIPS.Interpreter.System.CPU;

public partial class Processor
{
    delegate uint MemoryDelegate(uint rs, byte mem);
    delegate bool OverflowCheckDelegate(int a, int b, int r);

    /// <summary>
    /// Executes an instruction.
    /// </summary>
    /// <param name="instruction">The instruction to execute.</param>
    public Execution Execute(Instruction instruction)
    {
        // Create the exeuction
        var execution = CreateExecution(instruction);

        // Select the register file to write to, if any.
        var regFile = execution.RegisterSet switch
        {
            RegisterSet.None => null,
            RegisterSet.GeneralPurpose => RegisterFile,
            _ => ThrowHelper.ThrowNotSupportedException<RegisterFile>(),
        };

        // Write to the register file if needed, some instructions will not write to a register
        if (regFile is not null)
        {
            regFile[execution.GPR] = execution.WriteBack;
        }

        // Increment the program counter by default, some instructions will override this
        var programCounter = ProgramCounter + 4;

        // Apply side effects
        switch (execution.SideEffects)
        {
            case SecondaryWritebacks.Low:
                Low = execution.Low;
                break;
            case SecondaryWritebacks.High:
                High = execution.High;
                break;
            case SecondaryWritebacks.HighLow:
                Low = execution.Low;
                High = execution.High;
                break;
            case SecondaryWritebacks.ProgramCounter:
                programCounter = execution.ProgramCounter;
                break;
            case SecondaryWritebacks.Memory:
                _memory[execution.MemAddress] = execution.WriteBack;
                break;
        }

        // Apply the program counter update
        ProgramCounter = programCounter;

        return execution;
    }

    private Execution CreateExecution(Instruction instruction)
    {
        var rs = RegisterFile[instruction.RS];
        var rt = RegisterFile[instruction.RT];
        var shift = instruction.ShiftAmount;

        return instruction.OpCode switch
        {
            // Special (R-Type)
            OperationCode.Special or
            OperationCode.Special2 or
            OperationCode.Special3 => CreateExecutionRType(instruction),

            // Jump (J-Type)
            OperationCode.Jump => new Execution
            {
                ProgramCounter = instruction.Address,
            },
            OperationCode.JumpAndLink => new Execution
            {
                ProgramCounter = instruction.Address,
                Destination = GPRegister.ReturnAddress,
                WriteBack = ProgramCounter,
            },
            OperationCode.JumpAndLinkX => throw new NotImplementedException(),

            // Branch/Trap type (B-Type)
            OperationCode.RegisterImmediate => throw new NotImplementedException(),

            // Coprocessor instructions
            OperationCode.Coprocessor0 => throw new NotImplementedException(),
            OperationCode.Coprocessor1 => throw new NotImplementedException(),
            OperationCode.Coprocessor2 => throw new NotImplementedException(),
            OperationCode.Coprocessor3 => throw new NotImplementedException(),

            OperationCode.Trap => throw new NotImplementedException(),
            OperationCode.SIMD => throw new NotImplementedException(),
            OperationCode.LoadByte => throw new NotImplementedException(),
            OperationCode.LoadHalfWord => throw new NotImplementedException(),
            OperationCode.LoadWordLeft => throw new NotImplementedException(),
            OperationCode.LoadWord => throw new NotImplementedException(),
            OperationCode.LoadByteUnsigned => throw new NotImplementedException(),
            OperationCode.LoadHalfWordUnsigned => throw new NotImplementedException(),
            OperationCode.LoadWordRight => throw new NotImplementedException(),
            OperationCode.StoreByte => throw new NotImplementedException(),
            OperationCode.StoreHalfWord => throw new NotImplementedException(),
            OperationCode.StoreWordLeft => throw new NotImplementedException(),
            OperationCode.StoreWord => throw new NotImplementedException(),
            OperationCode.StoreWordRight => throw new NotImplementedException(),
            OperationCode.LoadLinkedWord => throw new NotImplementedException(),
            OperationCode.LoadWordCoprocessor1 => throw new NotImplementedException(),
            OperationCode.LoadWordCoprocessor2 => throw new NotImplementedException(),
            OperationCode.LoadWordCoprocessor3 => throw new NotImplementedException(),
            OperationCode.LoadDoubleWordCoprocessor1 => throw new NotImplementedException(),
            OperationCode.LoadDoubleWordCoprocessor2 => throw new NotImplementedException(),
            OperationCode.LoadDoubleWordCoprocessor3 => throw new NotImplementedException(),
            OperationCode.StoreConditionalWord => throw new NotImplementedException(),
            OperationCode.StoreWordCoprocessor1 => throw new NotImplementedException(),
            OperationCode.StoreWordCoprocessor2 => throw new NotImplementedException(),
            OperationCode.StoreWordCoprocessor3 => throw new NotImplementedException(),

            // Fall-through to I-Type by default. Most instructions are I-Type and they take up many opcodes.
            _ => CreateExecutionIType(instruction),
        };
    }
}
