// Avishai Dernis 2025

using CommunityToolkit.Diagnostics;
using MIPS.Models.Instructions;
using MIPS.Models.Instructions.Enums;
using MIPS.Models.Instructions.Enums.Operations;
using MIPS.Models.Instructions.Enums.Registers;
using MIPS.Models.Instructions.Enums.SpecialFunctions;
using System;
using System.Net.Http.Headers;

namespace MIPS.Interpreter.System.CPU;

public partial class Processor
{
    delegate uint RTypeDelegate(uint rs, uint rt, byte shift, out uint? pc);
    delegate uint ITypeDelegate(uint rs, short immediate);
    delegate uint JTypeDelegate(uint rs, out uint? pc);
    delegate void BTypeDelegate(uint rs, uint rt, int offset, out uint? pc);

    private readonly struct Execution(uint output, Register destination, uint? newPc)
    {
        /// <summary>
        /// Gets the execution output.
        /// </summary>
        public uint Output { get; } = output;

        /// <summary>
        /// Gets the register destination of the output.
        /// </summary>
        /// <remarks>
        /// <see cref="Register.Zero"/> if none.
        /// </remarks>
        public Register Destination { get; } = destination;

        public uint? NewPc { get; } = newPc;

        /// <summary>
        /// Gets a value indicating whether or not execution handled the PC changing.
        /// </summary>
        public bool PCHandled => NewPc is not null;
    }

    /// <summary>
    /// Executes an instruction.
    /// </summary>
    /// <param name="instruction">The instruction to execute.</param>
    public void Execute(Instruction instruction)
    {
        // Run operation
        Execution execution = instruction.Type switch
        {
            InstructionType.BasicR => ExecuteR(instruction),
            InstructionType.BasicJ => ExecuteJ(instruction),
            InstructionType.BasicI => ExecuteI(instruction),
            InstructionType.RegisterImmediate or 
            InstructionType.RegisterImmediateBranch => ExecuteB(instruction),
            _ => ThrowHelper.ThrowInvalidDataException<Execution>($"Invalid instruction type '{instruction.Type}'."),
        };

        // Apply register write back
        _regFile[execution.Destination] = execution.Output;

        // Increment program counter if not handled by execution
        if (!execution.PCHandled)
        {
            ProgramCounter += 4;
        }
    }

    private Execution ExecuteR(Instruction instruction)
    {
        var dest = instruction.RD;
        bool pcHandled = instruction.FuncCode is FunctionCode.JumpRegister or FunctionCode.JumpAndLinkRegister;
        var rs = _regFile[instruction.RS];
        var rt = _regFile[instruction.RT];
        var shift = instruction.ShiftAmount;

        RTypeDelegate operation = instruction.FuncCode switch
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
            FunctionCode.Break => BREAK,

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

            _ => ThrowHelper.ThrowInvalidDataException<RTypeDelegate>($"Invalid function code '{instruction.FuncCode}'."),
        };

        var output = operation(rs, rt, shift, out var pc);
        return new Execution(output, dest, pc);
    }

    private Execution ExecuteJ(Instruction instruction)
    {
        JTypeDelegate operation = instruction.OpCode switch
        {
            OperationCode.Jump => J,
            OperationCode.JumpAndLink => JAL,
            _ => ThrowHelper.ThrowInvalidDataException<JTypeDelegate>($"Invalid operation code '{instruction.OpCode}'.")
        };

        var ouput = operation(instruction.Address, out var pc);
        return new Execution(0, Register.Zero, pc);
    }

    private Execution ExecuteI(Instruction instruction)
    {
        var dest = instruction.RT;
        var rs = _regFile[instruction.RS];
        var immediate = instruction.ImmediateValue;

        ITypeDelegate operation = instruction.OpCode switch
        {
            OperationCode.AddImmediate => ADDI,
            OperationCode.AddImmediateUnsigned => ADDIU,
            OperationCode.SetLessThanImmediate => SLTI,
            OperationCode.SetLessThanImmediateUnsigned => SLTIU,

            OperationCode.AndImmediate => ANDI,
            OperationCode.OrImmediate => ORI,
            OperationCode.ExclusiveOrImmediate => XORI,
            _ => ThrowHelper.ThrowInvalidDataException<ITypeDelegate>($"Invalid operation code '{instruction.OpCode}'.")
        };

        var output = operation(rs, immediate);
        return new Execution(output, dest, null);
    }

    private Execution ExecuteB(Instruction instruction)
    {
        var rs = _regFile[instruction.RS];
        var rt = _regFile[instruction.RT];
        var offset = instruction.Offset;

        BTypeDelegate operation = instruction.OpCode switch
        {
            OperationCode.BranchOnEquals => BEQ,
            OperationCode.BranchOnNotEquals => BNE,
            OperationCode.BranchOnLessThanOrEqualToZero => BLEZ,
            OperationCode.BranchOnGreaterThanZero => BGTZ,
            _ => ThrowHelper.ThrowInvalidDataException<BTypeDelegate>($"Invalid operation code '{instruction.OpCode}'.")
        };

        operation(rs, rt, offset, out var pc);
        return new Execution(0, Register.Zero, null);
    }

    // R Type functions
    private uint SLL(uint rs, uint rt, byte shift, out uint? pc)
    {
        pc = null;
        return rt << shift;
    }

    private uint SRL(uint rs, uint rt, byte shift, out uint? pc)
    {
        pc = null;
        return rt >> shift;
    }

    private uint SRA(uint rs, uint rt, byte shift, out uint? pc)
    {
        pc = null;
        return rt >> shift; // TODO: Arithmetic Shift
    }

    private uint SLLV(uint rs, uint rt, byte shift, out uint? pc)
    {
        pc = null;
        return rt << (int)rs;
    }

    private uint SRLV(uint rs, uint rt, byte shift, out uint? pc)
    {
        pc = null;
        return rt >> (int)rs;
    }

    private uint SRAV(uint rs, uint rt, byte shift, out uint? pc)
    {
        pc = null;
        return rt >> (int)rs; // TODO: Arithmetic Shift
    }

    private uint JR(uint rs, uint rt, byte shift, out uint? pc)
    {
        pc = rs;
        return 0;
    }

    private uint JALR(uint rs, uint rt, byte shift, out uint? pc)
    {
        pc = rs;
        return ProgramCounter;
    }

    private uint SYSCALL(uint rs, uint rt, byte shift, out uint? pc)
    {
        throw new NotImplementedException();
    }
    private uint BREAK(uint rs, uint rt, byte shift, out uint? pc)
    {
        throw new NotImplementedException();
    }

    private uint MFHI(uint rs, uint rt, byte shift, out uint? pc)
    {
        pc = null;
        return High;
    }

    private uint MTHI(uint rs, uint rt, byte shift, out uint? pc)
    {
        High = rs;
        pc = null;
        return 0;
    }
    private uint MFLO(uint rs, uint rt, byte shift, out uint? pc)
    {
        pc = null;
        return Low;
    }

    private uint MTLO(uint rs, uint rt, byte shift, out uint? pc)
    {
        Low = rs;
        pc = null;
        return 0;
    }

    private uint MULT(uint rs, uint rt, byte shift, out uint? pc)
    {
        Low = (uint)((int)rs * (int)rt);
        pc = null;
        // TODO: High bits
        return 0;
    }
    private uint MULTU(uint rs, uint rt, byte shift, out uint? pc)
    {
        Low = rs * rt;
        pc = null;
        // TODO: High bits
        return 0;
    }
    private uint DIV(uint rs, uint rt, byte shift, out uint? pc)
    {
        Low = (uint)((int)rs / (int)rt);
        High = (uint)((int)rs % (int)rt);
        pc = null;
        return 0;
    }
    private uint DIVU(uint rs, uint rt, byte shift, out uint? pc)
    {
        Low = rs / rt;
        High = rs % rt;
        pc = null;
        return 0;
    }

    private uint ADD(uint rs, uint rt, byte shift, out uint? pc)
    {
        pc = null;
        return (uint)((int)rs + (int)rt);
    }

    private uint ADDU(uint rs, uint rt, byte shift, out uint? pc)
    {
        pc = null;
        return rs + rt;
    }

    private uint SUB(uint rs, uint rt, byte shift, out uint? pc)
    {
        pc = null;
        return (uint)((int)rs - (int)rt);
    }

    private uint SUBU(uint rs, uint rt, byte shift, out uint? pc)
    {
        pc = null;
        return rs - rt;
    }

    private uint AND(uint rs, uint rt, byte shift, out uint? pc)
    {
        pc = null;
        return rs & rt;
    }

    private uint OR(uint rs, uint rt, byte shift, out uint? pc)
    {
        pc = null;
        return rs | rt;
    }

    private uint XOR(uint rs, uint rt, byte shift, out uint? pc)
    {
        pc = null;
        return rs ^ rt;
    }

    private uint NOR(uint rs, uint rt, byte shift, out uint? pc)
    {
        pc = null;
        return ~(rs | rt);
    }

    private uint SLT(uint rs, uint rt, byte shift, out uint? pc)
    {
        pc = null;
        return (uint)((int)rs < (int)rt ? 1 : 0);
    }

    private uint SLTU(uint rs, uint rt, byte shift, out uint? pc)
    {
        pc = null;
        return (uint)(rs < rt ? 1 : 0);
    }

    // J Type functions
    private uint J(uint addr, out uint? pc)
    {
        pc = addr * 4;
        return 0;
    }

    private uint JAL(uint addr, out uint? pc)
    {
        J(addr, out pc);
        return ProgramCounter;
    }
    
    // B Type functions
    private void BEQ(uint rs, uint rt, int offset, out uint? pc)
    {
        pc = null;
        BranchConditionally(rs == rt, offset, out pc);
    }

    private void BNE(uint rs, uint rt, int offset, out uint? pc)
    {
        pc = null;
        BranchConditionally(rs != rt, offset, out pc);
    }

    private void BLEZ(uint rs, uint rt, int offset, out uint? pc)
    {
        pc = null;
        BranchConditionally(rs <= 0, offset, out pc);
    }

    private void BGTZ(uint rs, uint rt, int offset, out uint? pc)
    {
        pc = null;
        BranchConditionally(rs > 0, offset, out pc);
    }
    
    // I Type functions
    private uint ADDI(uint rs, short immediate)
    {
        return (uint)((int)rs + immediate);
    }

    private uint ADDIU(uint rs, short immediate)
    {
        return (uint)(rs + immediate);
    }

    private uint SLTI(uint rs, short immediate)
    {
        return (uint)((int)rs < immediate ? 1 : 0);
    }

    private uint SLTIU(uint rs, short immediate)
    {
        return (uint)(rs < immediate ? 1 : 0);
    }

    private uint ANDI(uint rs, short immediate)
    {
        return (uint)(rs & immediate);
    }

    private uint ORI(uint rs, short immediate)
    {
        return (rs | (ushort)immediate);
    }

    private uint XORI(uint rs, short immediate)
    {
        return (uint)(rs ^ immediate);
    }

    private void BranchConditionally(bool branch, int offset, out uint? pc)
    {
        pc = null;

        if (branch)
        {
            pc = (uint)(ProgramCounter + offset);
        }
    }
}
