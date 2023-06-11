// Adam Dernis 2023

using CommunityToolkit.Diagnostics;
using MIPS.Models.Instructions;
using MIPS.Models.Instructions.Enums;
using System;

namespace MIPS.Emulator.System.CPU;

public partial class ProcessingUnit
{
    private struct Execution
    {
        public Execution(uint output, Register destination, bool pcHandled)
        {
            Output = output;
            Destination = destination;
            PCHandled = pcHandled;
        }

        /// <summary>
        /// Gets the execution output.
        /// </summary>
        public uint Output { get; }

        /// <summary>
        /// Gets the register destination of the output.
        /// </summary>
        /// <remarks>
        /// <see cref="Register.Zero"/> if none.
        /// </remarks>
        public Register Destination { get; }

        /// <summary>
        /// Gets a value indicating whether or not execution handled the PC changing.
        /// </summary>
        public bool PCHandled { get; }
    }

    /// <summary>
    /// Executes an instruction.
    /// </summary>
    /// <param name="instruction">The instruction to execute.</param>
    private void Execute(Instruction instruction)
    {
        // Run operation
        Execution execution = instruction.Type switch
        {
            InstructionType.R => ExecuteR(instruction),
            InstructionType.J => ExecuteJ(instruction),
            InstructionType.I => ExecuteI(instruction),
            _ => ThrowHelper.ThrowInvalidDataException<Execution>($"Invalid instruction type '{instruction.Type}'."),
        };

        // Apply register write back
        _regFile[execution.Destination] = execution.Output;

        // Increment program counter if not handled by execution
        if (!execution.PCHandled)
        {
            _programCounter += 4;
        }
    }

    private Execution ExecuteR(Instruction instruction)
    {
        var dest = instruction.RD;
        bool pcHandled = instruction.FuncCode is FunctionCode.JumpRegister or FunctionCode.JumpAndLinkRegister;
        var rs = _regFile[instruction.RS];
        var rt = _regFile[instruction.RT];
        var shift = instruction.ShiftAmount;

        Func<uint, uint, byte, uint> operation = instruction.FuncCode switch
        {
            FunctionCode.ShiftLeftLogical => SLL,
            FunctionCode.ShiftRightLogical => SRL,
            FunctionCode.ShiftRightArithmetic => SRA,

            FunctionCode.ShiftLeftLogicalVariable => SLLV,
            FunctionCode.ShiftRightLogicalVariable => SRLV,
            FunctionCode.ShiftRightArithmeticVariable => SRAV,

            FunctionCode.JumpRegister => JR,
            FunctionCode.JumpAndLinkRegister => JALR,

            FunctionCode.SystemCall => SYSCALL,

            FunctionCode.MoveFromHigh => MFHI,
            FunctionCode.MoveToHigh => MTHI,
            FunctionCode.MoveFromLow => MFLO,
            FunctionCode.MoveToLow => MTLO,

            FunctionCode.Multiply => MULT,
            FunctionCode.MultiplyUnsigned => MULTU,
            FunctionCode.Divide => DIV,
            FunctionCode.DivideUnsigned => DIVU,

            FunctionCode.Add => ADD,
            FunctionCode.AddUnsigned => ADDU,
            FunctionCode.Subtract => SUB,
            FunctionCode.SubtractUnsigned => SUBU,

            FunctionCode.And => AND,
            FunctionCode.Or => OR,
            FunctionCode.ExclusiveOr => XOR,
            FunctionCode.Nor => NOR,

            FunctionCode.SetLessThan => SLT,
            FunctionCode.SetLessThanUnsigned => SLTU,

            _ => ThrowHelper.ThrowInvalidDataException<Func<uint, uint, byte, uint>>($"Invalid function code '{instruction.FuncCode}'."),
        };

        var output = operation(rs, rt, shift);
        return new Execution(output, dest, pcHandled);
    }

    private Execution ExecuteJ(Instruction instruction)
    {
        Action<uint> operation = instruction.OpCode switch
        {
            OperationCode.Jump => J,
            OperationCode.JumpAndLink => JAL,
            _ => ThrowHelper.ThrowInvalidDataException<Action<uint>>($"Invalid operation code '{instruction.OpCode}'.")
        };

        operation(instruction.Address);
        return new Execution(0, Register.Zero, true);
    }

    private Execution ExecuteI(Instruction instruction)
    {
        var dest = instruction.RT;
        var rs = _regFile[instruction.RS];
        var rt = _regFile[instruction.RT];
        var immediate = instruction.ImmediateValue;

        Func<uint, uint, short, uint> operation = instruction.OpCode switch
        {
            OperationCode.BranchOnEquals => BEQ,
            OperationCode.BranchOnNotEquals => BNE,
            OperationCode.BranchOnLessThanOrEqualToZero => BLEZ,
            OperationCode.BranchGreaterThanZero => BGTZ,

            OperationCode.AddImmediate => ADDI,
            OperationCode.AddImmediateUnsigned => ADDIU,
            OperationCode.SetLessThanImmediate => SLTI,
            OperationCode.SetLessThanImmediateUnsigned => SLTIU,

            OperationCode.AndImmediate => ANDI,
            OperationCode.OrImmediate => ORI,
            OperationCode.ExclusiveOrImmediate => XORI,
            _ => ThrowHelper.ThrowInvalidDataException<Func<uint, uint, short, uint>>($"Invalid operation code '{instruction.OpCode}'.")
        };

        var output = operation(rs, rt, immediate);
        
        // TODO: Overload write back for branch 
        // TODO: Set PCHandled for branch
        bool pcHandled = false;
        return new Execution(output, dest, pcHandled);
    }

    // R Type functions
    private uint SLL(uint rs, uint rt, byte shift) => rt << shift;
    private uint SRL(uint rs, uint rt, byte shift) => rt >> shift;
    private uint SRA(uint rs, uint rt, byte shift) => rt >> shift; // TODO: Arithmetic shift

    private uint SLLV(uint rs, uint rt, byte shift) => rt << (int)rs;
    private uint SRLV(uint rs, uint rt, byte shift) => rt >> (int)rs;
    private uint SRAV(uint rs, uint rt, byte shift) => rt >> (int)rs; // TODO: Arithmetic shift

    private uint JR(uint rs, uint rt, byte shift)
    {
        Jump(rs);
        return 0;
    }
    private uint JALR(uint rs, uint rt, byte shift)
    {
        Link();
        Jump(rs);
        return 0;
    }

    private uint SYSCALL(uint rs, uint rt, byte shift)
    {
        throw new NotImplementedException();
    }

    private uint MFHI(uint rs, uint rt, byte shift) => _regFile.High;
    private uint MTHI(uint rs, uint rt, byte shift)
    {
        _regFile.High = rs;
        return 0;
    }
    private uint MFLO(uint rs, uint rt, byte shift) => _regFile.Low;
    private uint MTLO(uint rs, uint rt, byte shift)
    {
        _regFile.Low = rs;
        return 0;
    }

    private uint MULT(uint rs, uint rt, byte shift)
    {
        _regFile.Low = (uint)((int)rs * (int)rt);
        // TODO: High bits
        return 0;
    }
    private uint MULTU(uint rs, uint rt, byte shift)
    {
        _regFile.Low = rs * rt;
        // TODO: High bits
        return 0;
    }
    private uint DIV(uint rs, uint rt, byte shift)
    {
        _regFile.Low = (uint)((int)rs / (int)rt);
        _regFile.High = (uint)((int)rs % (int)rt);
        return 0;
    }
    private uint DIVU(uint rs, uint rt, byte shift)
    {
        _regFile.Low = rs / rt;
        _regFile.High = rs % rt;
        return 0;
    }

    private uint ADD(uint rs, uint rt, byte shift) => (uint)((int)rs + (int)rt);
    private uint ADDU(uint rs, uint rt, byte shift) => rs + rt;
    private uint SUB(uint rs, uint rt, byte shift) => (uint)((int)rs - (int)rt);
    private uint SUBU(uint rs, uint rt, byte shift) => rs - rt;

    private uint AND(uint rs, uint rt, byte shift) => rs & rt;
    private uint OR(uint rs, uint rt, byte shift) => rs | rt;
    private uint XOR(uint rs, uint rt, byte shift) => rs ^ rt;
    private uint NOR(uint rs, uint rt, byte shift) => ~(rs | rt);

    private uint SLT(uint rs, uint rt, byte shift) => (uint)((int)rs < (int)rt ? 1 : 0);
    private uint SLTU(uint rs, uint rt, byte shift) => (uint)(rs < rt ? 1 : 0);

    // J Type functions
    private void J(uint addr) => JumpPartial(addr);
    private void JAL(uint addr)
    {
        Link();
        JumpPartial(addr);
    }

    // I Type functions
    private uint BEQ(uint rs, uint rt, short offset) => BranchConditionally(rs == rt, offset);
    private uint BNE(uint rs, uint rt, short offset) => BranchConditionally(rs != rt, offset);
    private uint BLEZ(uint rs, uint rt, short offset) => BranchConditionally(rs <= 0, offset);
    private uint BGTZ(uint rs, uint rt, short offset) => BranchConditionally(rs > 0, offset);

    private uint ADDI(uint rs, uint rt, short immediate) => (uint)((int)rs + (short)immediate);
    private uint ADDIU(uint rs, uint rt, short immediate) => (uint)(rs + immediate);

    private uint SLTI(uint rs, uint rt, short immediate) => (uint)((int)rs < (short)immediate ? 1 : 0);
    private uint SLTIU(uint rs, uint rt, short immediate) => (uint) (rs < immediate ? 1 : 0);

    private uint ANDI(uint rs, uint rt, short immediate) => (uint)(rs & immediate);
    private uint ORI(uint rs, uint rt, short immediate) => (rs | (ushort)immediate);
    private uint XORI(uint rs, uint rt, short immediate) => (uint)(rs ^ immediate);

    private uint BranchConditionally(bool branch, short offset)
    {
        if (branch)
        {
            JumpOffset((short)offset);
        }

        return (uint)(branch ? 1 : 0);
    }
}
