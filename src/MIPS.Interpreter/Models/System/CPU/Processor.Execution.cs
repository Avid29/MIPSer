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
    delegate uint BasicRDelegate(uint rs, uint rt);
    delegate ulong MultRDelegate(uint rs, uint rt);
    delegate uint ShiftRDelegate(uint rs, byte shift);
    delegate uint BasicIDelegate(uint rs, short imm);
    delegate uint MemoryDelegate(uint rs, byte mem);
    delegate bool BranchDelegate(uint rs, uint rt);

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
            RegisterSet.Numbered => null,
            RegisterSet.GeneralPurpose => RegisterFile,
            _ => ThrowHelper.ThrowNotSupportedException<RegisterFile>(),
        };

        // Write to the register file if needed, some instructions will not write to a register
        if (regFile is not null)
        {
            regFile[execution.Destination] = execution.Output;
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
                _memory[execution.MemAddress] = execution.Output;
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
            OperationCode.Special => instruction.FuncCode switch
            {
                // Shift
                FunctionCode.ShiftLeftLogical => ShiftR(instruction, (rt, shift) => rt << shift),
                FunctionCode.ShiftRightLogical => ShiftR(instruction, (rt, shift) => rt >> shift),
                FunctionCode.ShiftRightArithmetic => ShiftR(instruction, (rt, shift) => (uint)((int)rt >> shift)),
                FunctionCode.ShiftLeftLogicalVariable => BasicR(instruction, (rs, rt) => rt << (int)rs),
                FunctionCode.ShiftRightLogicalVariable => BasicR(instruction, (rs, rt) => rt >> (int)rs),
                FunctionCode.ShiftRightArithmeticVariable => BasicR(instruction, (rs, rt) => (uint)((int)rt >> (int)rs)),
                
                // Arithmetic
                FunctionCode.Add => BasicR(instruction, (rs, rt) => (uint)((int)rs + (int)rt)),
                FunctionCode.AddUnsigned => BasicR(instruction, (rs, rt) => rs + rt),
                FunctionCode.Subtract => BasicR(instruction, (rs, rt) => (uint)((int)rs - (int)rt)),
                FunctionCode.SubtractUnsigned => BasicR(instruction, (rs, rt) => rs - rt),
                
                // Logical
                FunctionCode.And => BasicR(instruction, (rs, rt) => rs & rt),
                FunctionCode.Or => BasicR(instruction, (rs, rt) => rs | rt),
                FunctionCode.ExclusiveOr => BasicR(instruction, (rs, rt) => rs ^ rt),
                FunctionCode.Nor => BasicR(instruction, (rs, rt) => ~(rs | rt)),

                // Compare
                FunctionCode.SetLessThan => BasicR(instruction, (rs, rt) => (uint)((int)rs < (int)rt ? 1 : 0)),
                FunctionCode.SetLessThanUnsigned => BasicR(instruction, (rs, rt) => (uint)(rs < rt ? 1 : 0)),

                // Jump register
                FunctionCode.JumpRegister => JumpR(instruction),
                FunctionCode.JumpAndLinkRegister => JumpR(instruction, instruction.RD),

                FunctionCode.SystemCall => throw new NotImplementedException(),
                FunctionCode.Break => throw new NotImplementedException(),
                FunctionCode.Sync => throw new NotImplementedException(),

                FunctionCode.MoveFromHigh => new Execution
                {
                    Output = High,
                    Destination = instruction.RD,
                },
                FunctionCode.MoveToHigh => new Execution
                {
                    High = rs,
                },
                FunctionCode.MoveFromLow => new Execution
                {
                    Output = Low,
                    Destination = instruction.RD,
                },
                FunctionCode.MoveToLow => new Execution
                {
                    Low = rs,
                },

                FunctionCode.Multiply => MultR(instruction, (rs, rt) => (ulong)((long)(int)rs * (int)rt)),
                FunctionCode.MultiplyUnsigned => MultR(instruction, (rs, rt) => (ulong)rs * rt),

                FunctionCode.Divide => throw new NotImplementedException(),
                FunctionCode.DivideUnsigned => throw new NotImplementedException(),
                FunctionCode.TrapOnGreaterOrEqual => throw new NotImplementedException(),
                FunctionCode.TrapOnGreaterOrEqualUnsigned => throw new NotImplementedException(),
                FunctionCode.TrapOnLessThan => throw new NotImplementedException(),
                FunctionCode.TrapOnLessThanUnsigned => throw new NotImplementedException(),
                FunctionCode.TrapOnEquals => throw new NotImplementedException(),
                FunctionCode.TrapOnNotEquals => throw new NotImplementedException(),
                _ => throw new NotImplementedException(),
            },

            // Special 2 (R-Type)
            OperationCode.Special2 => instruction.Func2Code switch
            {
                Func2Code.CountLeadingZeros => BasicR(instruction, (rs, _) => (uint)BitOperations.LeadingZeroCount(rs)),
                Func2Code.CountLeadingOnes => BasicR(instruction, (rs, _) => (uint)BitOperations.LeadingZeroCount(~rs)),
                _ => throw new NotImplementedException(),
            },

            OperationCode.Jump => new Execution
            {
                ProgramCounter = instruction.Address,
            },
            OperationCode.JumpAndLink => new Execution
            {
                ProgramCounter = instruction.Address,
                Destination = GPRegister.ReturnAddress,
                Output = ProgramCounter,
            },

            OperationCode.RegisterImmediate => throw new NotImplementedException(),
            
            // Branch
            OperationCode.BranchOnEquals => Branch(instruction, (rs, rt) => rs == rt),
            OperationCode.BranchOnNotEquals => Branch(instruction, (rs, rt) => rs != rt),
            OperationCode.BranchOnLessThanOrEqualToZero => throw new NotImplementedException(),
            OperationCode.BranchOnGreaterThanZero => throw new NotImplementedException(),

            // Arithmetic
            OperationCode.AddImmediate => BasicI(instruction, (rs, imm) => (uint)((int)rs + imm)),
            OperationCode.AddImmediateUnsigned => BasicI(instruction, (rs, imm) => rs + (ushort)imm),

            // Compare
            OperationCode.SetLessThanImmediate => BasicI(instruction, (rs, imm) => (uint)((int)rs < imm ? 1 : 0)),
            OperationCode.SetLessThanImmediateUnsigned => BasicI(instruction, (rs, imm) => (uint)(rs < imm ? 1 : 0)),

            // Logical
            OperationCode.AndImmediate => BasicI(instruction, (rs, imm) => rs & (ushort)imm),
            OperationCode.OrImmediate => BasicI(instruction, (rs, imm) => rs | (ushort)imm),
            OperationCode.ExclusiveOrImmediate => BasicI(instruction, (rs, imm) => rs ^ (ushort)imm),

            OperationCode.LoadUpperImmediate => BasicI(instruction, (rs, imm) => (uint)((ushort)imm << 16)),

            OperationCode.Coprocessor0 => throw new NotImplementedException(),
            OperationCode.Coprocessor1 => throw new NotImplementedException(),
            OperationCode.Coprocessor2 => throw new NotImplementedException(),
            OperationCode.Coprocessor3 => throw new NotImplementedException(),
            OperationCode.BranchOnEqualLikely => throw new NotImplementedException(),
            OperationCode.BranchOnNotEqualLikely => throw new NotImplementedException(),
            OperationCode.BranchOnLessThanOrEqualToZeroLikely => throw new NotImplementedException(),
            OperationCode.BranchOnGreaterThanZeroLikely => throw new NotImplementedException(),
            OperationCode.Trap => throw new NotImplementedException(),
            OperationCode.JumpAndLinkX => throw new NotImplementedException(),
            OperationCode.SIMD => throw new NotImplementedException(),
            OperationCode.Special3 => throw new NotImplementedException(),
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
            _ => throw new NotImplementedException()
        };
    }

    private Execution BasicR(Instruction instruction, BasicRDelegate func)
    {
        var rs = RegisterFile[instruction.RS];
        var rt = RegisterFile[instruction.RT];
        var shift = instruction.ShiftAmount;

        var dest = instruction.RD;
        uint value = func(rs, rt);

        return new Execution
        {
            Destination = dest,
            Output = value,
        };
    }

    private Execution ShiftR(Instruction instruction, ShiftRDelegate func)
    {
        var rs = RegisterFile[instruction.RS];
        var rt = RegisterFile[instruction.RT];
        var shift = instruction.ShiftAmount;

        var dest = instruction.RD;
        uint value = func(rt, shift);

        return new Execution
        {
            Destination = dest,
            Output = value,
        };
    }

    private Execution MultR(Instruction instruction, MultRDelegate func)
    {
        var rs = RegisterFile[instruction.RS];
        var rt = RegisterFile[instruction.RT];
        
        var dest = instruction.RD;
        ulong value = func(rs, rt);

        return new Execution
        {
            HighLow = value,
        };
    }

    private Execution JumpR(Instruction instruction, GPRegister link = GPRegister.Zero)
    {
        var rs = RegisterFile[instruction.RS];

        return new Execution
        {
            ProgramCounter = rs,
            Destination = link,
            Output = ProgramCounter,
        };
    }

    private Execution BasicI(Instruction instruction, BasicIDelegate func)
    {
        var rs = RegisterFile[instruction.RS];
        var imm = instruction.ImmediateValue;

        var dest = instruction.RT;
        uint value = func(rs, imm);

        return new Execution
        {
            Destination = dest,
            Output = value,
        };
    }

    private Execution Branch(Instruction instruction, BranchDelegate func)
    {
        var rs = RegisterFile[instruction.RS];
        var rt = RegisterFile[instruction.RT];
        var imm = instruction.Offset;

        if (func(rs, rt))
        {
            return new Execution
            {
                ProgramCounter = (uint)(ProgramCounter + imm),
            };
        }

        return Execution.NoOp;
    }
}
