// Adam Dernis 2023

using MIPS.Assembler.Models.Instructions;
using MIPS.Models.Instructions.Enums;
using System.Collections.Generic;

namespace MIPS.Assembler.Helpers;

/// <summary>
/// A class containing a dictionary of instructions to metadata.
/// </summary>
public static class InstructionTable
{
    private static readonly Dictionary<string, InstructionMetadata> _table = new()
    {
        { "sll", new InstructionMetadata("sll", FunctionCode.ShiftLeftLogical) },
        { "srl", new InstructionMetadata("srl", FunctionCode.ShiftRightLogical) },
        { "sra", new InstructionMetadata("sra", FunctionCode.ShiftRightArithmetic) },

        { "jr", new InstructionMetadata("jr", FunctionCode.JumpRegister) },
        { "jalr", new InstructionMetadata("jalr", FunctionCode.JumpAndLinkRegister) },

        { "syscall", new InstructionMetadata("syscall", FunctionCode.SystemCall) },

        { "mfhi", new InstructionMetadata("mfhi", FunctionCode.MoveFromHi) },
        { "mflo", new InstructionMetadata("mflo", FunctionCode.MoveFromLow) },

        { "mult", new InstructionMetadata("mult", FunctionCode.Multiply) },
        { "multu", new InstructionMetadata("multu", FunctionCode.MultiplyUnsigned) },

        { "add", new InstructionMetadata("add", FunctionCode.Add) },
        { "addu", new InstructionMetadata("addu", FunctionCode.AddUnsigned) },
        { "sub", new InstructionMetadata("sub", FunctionCode.Subtract) },
        { "subu", new InstructionMetadata("subu", FunctionCode.SubtractUnsigned) },

        { "and", new InstructionMetadata("and", FunctionCode.And) },
        { "or", new InstructionMetadata("or", FunctionCode.Or) },
        { "xor", new InstructionMetadata("xor", FunctionCode.ExclusiveOr) },
        { "nor", new InstructionMetadata("nor", FunctionCode.Nor) },

        { "slt", new InstructionMetadata("slt", FunctionCode.SetLessThan) },
        { "sltu", new InstructionMetadata("sltu", FunctionCode.SetLessThanUnsigned) },
        
        { "j", new InstructionMetadata("j", OperationCode.Jump) },
        { "jal", new InstructionMetadata("jal", OperationCode.JumpAndLink) },

        { "beq", new InstructionMetadata("beq", OperationCode.BranchOnEquals) },
        { "bne", new InstructionMetadata("bne", OperationCode.BranchOnNotEquals) },
        { "blez", new InstructionMetadata("blez", OperationCode.BranchOnLessThanOrEqualToZero) },
        { "bgtz", new InstructionMetadata("bgtz", OperationCode.BranchGreaterThanZero) },

        { "addi", new InstructionMetadata("addi", OperationCode.AddImmediate) },
        { "addiu", new InstructionMetadata("addiu", OperationCode.AddImmediateUnsigned) },

        { "slti", new InstructionMetadata("slti", OperationCode.SetLessThanImmediate) },
        { "sltiu", new InstructionMetadata("sltiu", OperationCode.SetLessThanImmediateUnsigned) },

        { "andi", new InstructionMetadata("andi", OperationCode.AndImmediate) },
        { "ori", new InstructionMetadata("ori", OperationCode.OrImmediate) },
        { "xori", new InstructionMetadata("xori", OperationCode.ExclusiveOrImmediate) },

        { "lui", new InstructionMetadata("lui", OperationCode.LoadUpperImmediate) },

        // TODO: CoProcessing
        
        { "lb", new InstructionMetadata("lb", OperationCode.LoadByte) },
        { "lh", new InstructionMetadata("lh", OperationCode.LoadHalfWord) },
        { "lw", new InstructionMetadata("lw", OperationCode.LoadWord) },
        { "lbu", new InstructionMetadata("lbu", OperationCode.LoadByteUnsigned) },
        { "lhu", new InstructionMetadata("lhu", OperationCode.LoadHalfWordUnsigned) },

        { "sb", new InstructionMetadata("sb", OperationCode.StoreByte) },
        { "sh", new InstructionMetadata("sh", OperationCode.StoreHalfWord) },
        { "sw", new InstructionMetadata("sw", OperationCode.StoreWord) },
    };

    /// <summary>
    /// Attempts to get an instruction by name.
    /// </summary>
    /// <param name="name">The name of the instruction.</param>
    /// <param name="metadata">The instruction metadata.</param>
    /// <returns>Whether or not an instruction exists by that name</returns>
    public static bool TryGetInstruction(string name, out InstructionMetadata metadata)
        => _table.TryGetValue(name, out metadata);
}
