// Adam Dernis 2024

using CommunityToolkit.Diagnostics;
using MIPS.Models.Instructions.Enums;
using MIPS.Models.Instructions.Enums.Registers;
using MIPS.Models.Instructions.Enums.SpecialFunctions;

namespace MIPS.Models.Instructions;

/// <summary>
/// A struct representing a pseudo instruction.
/// </summary>
public readonly struct PseudoInstruction
{
    /// <summary>
    /// Initializes a new instance of the <see cref="PseudoInstruction"/> struct.
    /// </summary>
    /// <param name="op"></param>
    public PseudoInstruction(PseudoOp op)
    {
        PseudoOp = op;
    }

    /// <summary>
    /// Gets or sets the psudo operation code
    /// </summary>
    public PseudoOp PseudoOp { get; }

    /// <summary>
    /// Gets or sets the pseudo-instruction rs register.
    /// </summary>
    public Register RS { get; init; }
    
    /// <summary>
    /// Gets or sets the pseudo-instruction rt register.
    /// </summary>
    public Register RT { get; init; }
    
    /// <summary>
    /// Gets or sets the pseudo-instruction rd register.
    /// </summary>
    public Register RD { get; init; }
    
    /// <summary>
    /// Gets or sets the pseudo-instruction immediate value.
    /// </summary>
    public int Immediate { get; init; }
    
    /// <summary>
    /// Gets or sets the pseudo-instruction address.
    /// </summary>
    public uint Address { get; init; }

    /// <summary>
    /// Expands the pseudo-instruction into an array of real instructions.
    /// </summary>
    public readonly Instruction[] Expand()
    {
        return PseudoOp switch
        {
            PseudoOp.NoOperation =>
            [
                Instruction.Create(FunctionCode.ShiftLeftLogical, Register.Zero, Register.Zero, Register.Zero, 0),
            ],
            PseudoOp.SuperScalarNoOperation =>
            [
                Instruction.Create(FunctionCode.ShiftLeftLogical, Register.Zero, Register.Zero, Register.Zero, 1),
            ],
            PseudoOp.BranchOnLessThan =>
            [
                Instruction.Create(FunctionCode.SetLessThan, RS, RT, Register.AssemblerTemporary),
                Instruction.Create(OperationCode.BranchOnNotEquals, Register.AssemblerTemporary, Register.Zero, (short)Immediate)
            ],
            PseudoOp.LoadImmediate =>
            [
                Instruction.Create(OperationCode.LoadUpperImmediate, Register.AssemblerTemporary, (short)(Immediate >> 16)),
                Instruction.Create(OperationCode.OrImmediate, RT, Register.AssemblerTemporary, (short)Immediate)
            ],
            PseudoOp.AbsoluteValue =>
            [
                Instruction.Create(FunctionCode.AddUnsigned, RS, Register.Zero, RT),
                Instruction.Create(RegImmFuncCode.BranchOnGreaterOrEqualToThanZero, RS, 8),
                Instruction.Create(FunctionCode.Subtract, Register.Zero, RS, RT),
            ],
            PseudoOp.Move =>
            [
                Instruction.Create(FunctionCode.Add, RS, Register.Zero, RT),
            ],
            PseudoOp.LoadAddress =>
            [
                Instruction.Create(OperationCode.LoadUpperImmediate, Register.AssemblerTemporary, (short)(Address >> 16)),
                Instruction.Create(OperationCode.OrImmediate, RT, Register.AssemblerTemporary, (short)Address)
            ],
            PseudoOp.SetGreaterThanOrEqual =>
            [
                Instruction.Create(OperationCode.AddImmediateUnsigned, RT, RT, -1),
                Instruction.Create(FunctionCode.SetLessThan, RS, RT, RD),
            ],
            _ => ThrowHelper.ThrowArgumentOutOfRangeException<Instruction[]>(),
        };
    }

    /// <summary>
    /// Gets the number of real instructions required to implement the peudo instruction.
    /// </summary>
    public readonly int RealInstructionCount =>
        PseudoOp switch
        {
            PseudoOp.Move => 1,
            PseudoOp.BranchOnLessThan or PseudoOp.LoadImmediate or
            PseudoOp.LoadAddress or PseudoOp.SetGreaterThanOrEqual => 2,
            PseudoOp.AbsoluteValue => 3,
            _ => ThrowHelper.ThrowArgumentOutOfRangeException<int>(),
        };
}
