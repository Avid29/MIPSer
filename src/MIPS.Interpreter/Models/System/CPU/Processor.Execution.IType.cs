// Avishai Dernis 2025

using MIPS.Interpreter.Models.System.Execution;
using MIPS.Interpreter.Models.System.Execution.Enum;
using MIPS.Models.Instructions;
using MIPS.Models.Instructions.Enums.Operations;
using MIPS.Models.Instructions.Enums.Registers;
using System;

namespace MIPS.Interpreter.System.CPU;

public partial class Processor
{
    delegate uint BasicIDelegate(uint rs, short imm);

    private Execution CreateExecutionIType(Instruction instruction)
    {
        return instruction.OpCode switch
        {
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

            OperationCode.RegisterImmediate => throw new NotImplementedException(),
            
            // Branch
            OperationCode.BranchOnEquals or
            OperationCode.BranchOnEqualLikely => Branch(instruction, (rs, rt) => rs == rt),
            OperationCode.BranchOnNotEquals or 
            OperationCode.BranchOnNotEqualLikely => Branch(instruction, (rs, rt) => rs != rt),
            OperationCode.BranchOnLessThanOrEqualToZero or
            OperationCode.BranchOnLessThanOrEqualToZeroLikely => throw new NotImplementedException(),
            OperationCode.BranchOnGreaterThanZero or
            OperationCode.BranchOnGreaterThanZeroLikely => throw new NotImplementedException(),

            // Arithmetic
            OperationCode.AddImmediate => BasicI(instruction,
                (rs, imm) => (uint)((int)rs + imm),
                (a, b, r) => ((a ^ r) & (b ^ r)) < 0),
            OperationCode.AddImmediateUnsigned => BasicI(instruction, (rs, imm) => rs + (ushort)imm),

            // Compare
            OperationCode.SetLessThanImmediate => BasicI(instruction, (rs, imm) => (uint)((int)rs < imm ? 1 : 0)),
            OperationCode.SetLessThanImmediateUnsigned => BasicI(instruction, (rs, imm) => (uint)(rs < imm ? 1 : 0)),

            // Logical
            OperationCode.AndImmediate => BasicI(instruction, (rs, imm) => rs & (ushort)imm),
            OperationCode.OrImmediate => BasicI(instruction, (rs, imm) => rs | (ushort)imm),
            OperationCode.ExclusiveOrImmediate => BasicI(instruction, (rs, imm) => rs ^ (ushort)imm),

            OperationCode.LoadUpperImmediate => BasicI(instruction, (rs, imm) => (uint)((ushort)imm << 16)),

            _ => throw new NotImplementedException()
        };
    }

    private Execution BasicI(Instruction instruction, BasicIDelegate func, OverflowCheckDelegate? checkFunc = null)
    {
        // Retrieve the source register and immediate values
        var rs = _regFile[instruction.RS];
        var imm = instruction.ImmediateValue;

        // Determine the destination register and
        // compute the result using the provided function
        var dest = instruction.RT;
        uint value = func(rs, imm);


        // Check for overflow if a check function is provided
        // Return a trap execution if overflow is detected
        if (checkFunc is not null &&
            checkFunc((int)rs, imm, (int)value))
        {
            return new Execution
            {
                Destination = null,
                Trap = TrapKind.ArithmeticOverflow,
            };
        }

        // No overflow detected
        // Return the execution with the computed value and destination
        return new Execution
        {
            Destination = dest,
            WriteBack = value,
        };
    }

    private Execution Branch(Instruction instruction, BranchDelegate func)
    {
        var rs = _regFile[instruction.RS];
        var rt = _regFile[instruction.RT];
        var imm = instruction.Offset;

        if (func(rs, rt))
        {
            return new Execution
            {
                ProgramCounter = (uint)(ProgramCounter + imm),
            };
        }

        return default;
    }
}
