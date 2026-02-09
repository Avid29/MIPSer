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

    private Execution CreateExecutionRType(Instruction instruction)
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
                FunctionCode.Add => BasicR(instruction,
                    (rs, rt) => (uint)((int)rs + (int)rt),
                    (a, b, r) => ((a ^ r) & (b ^ r)) < 0),
                FunctionCode.AddUnsigned => BasicR(instruction, (rs, rt) => rs + rt),
                FunctionCode.Subtract => BasicR(instruction,
                    (rs, rt) => (uint)((int)rs - (int)rt),
                    (a, b, r) => ((a ^ b) & (a ^ r)) < 0),
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
                    WriteBack = High,
                    Destination = instruction.RD,
                },
                FunctionCode.MoveToHigh => new Execution
                {
                    High = rs,
                },
                FunctionCode.MoveFromLow => new Execution
                {
                    WriteBack = Low,
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

            _ => throw new NotImplementedException()
        };
    }

    private Execution BasicR(Instruction instruction, BasicRDelegate func, OverflowCheckDelegate? checkFunc = null)
    {
        var rs = RegisterFile[instruction.RS];
        var rt = RegisterFile[instruction.RT];

        var dest = instruction.RD;
        uint value = func(rs, rt);

        bool overflow = false;
        if (checkFunc is not null)
        {
            overflow = checkFunc((int)rs, (int)rt, (int)value);
        }

        return new Execution
        {
            // TODO: This is a hack to prevent writing to the destination register if there is an overflow, we should handle this better in the future.
            Destination = overflow ? GPRegister.Zero : dest,
            WriteBack = value,
            Trap = overflow ? TrapKind.ArithmeticOverflow : TrapKind.None,
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
            WriteBack = value,
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

    private Execution JumpR(Instruction instruction, GPRegister? link = null)
    {
        var rs = RegisterFile[instruction.RS];

        return new Execution
        {
            ProgramCounter = rs,
            Destination = link ?? GPRegister.Zero,
            WriteBack = link is null ? null : ProgramCounter,
        };
    }
}
