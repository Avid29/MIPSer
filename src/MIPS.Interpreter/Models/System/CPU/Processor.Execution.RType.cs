// Avishai Dernis 2025

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
                FunctionCode.Multiply => MultR(instruction, (rs, rt) => (ulong)((long)(int)rs * (int)rt)),
                FunctionCode.MultiplyUnsigned => MultR(instruction, (rs, rt) => (ulong)rs * rt),
                FunctionCode.Divide => DivR(instruction,
                    (rs, rt) => rt is not 0 ? (uint)((int)rs / (int)rt) : 0,
                    (rs, rt) => rt is not 0 ? (uint)((int)rs % (int)rt) : rs),
                FunctionCode.DivideUnsigned => DivR(instruction,
                    (rs, rt) => rt is not 0 ? rs / rt : 0,
                    (rs, rt) => rt is not 0 ? rs % rt : rs),

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

                // System
                FunctionCode.SystemCall => new Execution(TrapKind.Syscall),
                FunctionCode.Break => new Execution(TrapKind.Breakpoint),
                FunctionCode.Sync => throw new NotImplementedException(),

                FunctionCode.MoveFromHigh => new Execution(instruction.RD, HighLow.High),
                FunctionCode.MoveToHigh => new Execution
                {
                    High = rs,
                },
                FunctionCode.MoveFromLow => new Execution(instruction.RD, HighLow.Low),
                FunctionCode.MoveToLow => new Execution
                {
                    Low = rs,
                },

                // Trap
                FunctionCode.TrapOnGreaterOrEqual => TrapR(instruction, (rs, rt) => (int)rs >= (int)rt),
                FunctionCode.TrapOnGreaterOrEqualUnsigned => TrapR(instruction, (rs, rt) => rs >= rt),
                FunctionCode.TrapOnLessThan => TrapR(instruction, (rs, rt) => (int)rs < (int)rt),
                FunctionCode.TrapOnLessThanUnsigned => TrapR(instruction, (rs, rt) => rs < rt),
                FunctionCode.TrapOnEquals => TrapR(instruction, (rs, rt) => rs == rt),
                FunctionCode.TrapOnNotEquals => TrapR(instruction, (rs, rt) => rs != rt),

                _ => throw new NotImplementedException(),
            },

            // Special 2 (R-Type)
            OperationCode.Special2 => instruction.Func2Code switch
            {
                // Multiply
                Func2Code.MultiplyToGPR => BasicR(instruction, (rs, rt) => (uint)((long)(int)rs * (int)rt)),
                Func2Code.MultiplyAndAddHiLow => MultR(instruction, (rs, rt) =>
                {
                    long sum = ((long)(int)HighLow.High << 32) + (int)HighLow.Low;
                    sum += (long)(int)rs * (int)rt;
                    return (ulong)sum;
                }),
                Func2Code.MultiplyAndAddHiLowUnsigned => MultR(instruction, (rs, rt) =>
                {
                    ulong sum = ((ulong)HighLow.High << 32) + HighLow.Low;
                    sum += (ulong)rs * rt;
                    return sum;
                }),
                Func2Code.MultiplyAndSubtractHiLow => MultR(instruction, (rs, rt) =>
                {
                    long diff = ((long)(int)HighLow.High << 32) + (int)HighLow.Low;
                    diff -= (long)(int)rs * (int)rt;
                    return (ulong)diff;
                }),
                Func2Code.MultiplyAndSubtractHiLowUnsigned => MultR(instruction, (rs, rt) =>
                {
                    ulong diff = ((ulong)HighLow.High << 32) + HighLow.Low;
                    diff -= (ulong)rs * rt;
                    return diff;
                }),

                // Bit counting
                Func2Code.CountLeadingZeros => BasicR(instruction, (rs, _) => (uint)BitOperations.LeadingZeroCount(rs)),
                Func2Code.CountLeadingOnes => BasicR(instruction, (rs, _) => (uint)BitOperations.LeadingZeroCount(~rs)),
                _ => throw new NotImplementedException(),
            },

            _ => throw new NotImplementedException()
        };
    }

    private Execution BasicR(Instruction instruction, BasicRDelegate func, OverflowCheckDelegate? checkFunc = null)
    {
        // Retrieve the source register values
        var rs = RegisterFile[instruction.RS];
        var rt = RegisterFile[instruction.RT];

        // Determine the destination register and
        // compute the result using the provided function
        var dest = instruction.RD;
        uint value = func(rs, rt);

        // Check for overflow if a check function is provided
        // Return a trap execution if overflow is detected
        if (checkFunc is not null &&
            checkFunc((int)rs, (int)rt, (int)value))
        {
            return new Execution(TrapKind.ArithmeticOverflow);
        }

        // No overflow detected
        // Return the execution with the computed value and destination
        return new Execution(dest, value);
    }

    private Execution ShiftR(Instruction instruction, ShiftRDelegate func)
    {
        var rt = RegisterFile[instruction.RT];
        var shift = instruction.ShiftAmount;

        var dest = instruction.RD;
        uint value = func(rt, shift);

        return new Execution(dest, value);
    }

    private Execution MultR(Instruction instruction, MultRDelegate func)
    {
        var rs = RegisterFile[instruction.RS];
        var rt = RegisterFile[instruction.RT];

        ulong value = func(rs, rt);

        return new Execution(value);
    }

    private Execution DivR(Instruction instruction, BasicRDelegate divFunc, BasicRDelegate remFunc)
    {
        var rs = RegisterFile[instruction.RS];
        var rt = RegisterFile[instruction.RT];

        uint div = divFunc(rs, rt);
        uint rem = remFunc(rs, rt);

        return new Execution((rem, div));
    }

    private Execution TrapR(Instruction instruction, BranchDelegate func)
    {
        var rs = RegisterFile[instruction.RS];
        var rt = RegisterFile[instruction.RT];

        return func(rs, rt) ? new Execution(TrapKind.Trap) : default;
    }

    private Execution JumpR(Instruction instruction, GPRegister? link = null)
    {
        var rs = RegisterFile[instruction.RS];

        if (link is null)
        {
            // No link register specified, just jump to the target address
            return new Execution(rs);
        }

        // A link register was provided
        // Write the return address to the specificied link register
        // and set the program counter to the jump address
        return new Execution(link.Value, ProgramCounter + 4)
        {
            ProgramCounter = rs,
        };
    }
}
