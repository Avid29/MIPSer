// Adam Dernis 2024

using CommunityToolkit.Diagnostics;
using Zarem.Models.Instructions.Enums.Operations;
using Zarem.Models.Instructions.Enums.Registers;
using Zarem.Models.Instructions.Enums.SpecialFunctions;

namespace Zarem.Models.Instructions;

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
    public PseudoOp PseudoOp { get; init; }

    /// <summary>
    /// Gets or sets the pseudo-instruction rs register.
    /// </summary>
    public GPRegister RS { get; init; }
    
    /// <summary>
    /// Gets or sets the pseudo-instruction rt register.
    /// </summary>
    public GPRegister RT { get; init; }
    
    /// <summary>
    /// Gets or sets the pseudo-instruction rd register.
    /// </summary>
    public GPRegister RD { get; init; }
    
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
                Instruction.Create(FunctionCode.ShiftLeftLogical, GPRegister.Zero, GPRegister.Zero, GPRegister.Zero, 0),
            ],
            PseudoOp.SuperScalarNoOperation =>
            [
                Instruction.Create(FunctionCode.ShiftLeftLogical, GPRegister.Zero, GPRegister.Zero, GPRegister.Zero, 1),
            ],
            PseudoOp.UnconditionalBranch =>
            [
                Instruction.Create(OperationCode.BranchOnEquals, GPRegister.Zero, GPRegister.Zero, (short)Immediate),
            ],
            PseudoOp.BranchOnLessThan =>
            [
                Instruction.Create(FunctionCode.SetLessThan, RS, RT, GPRegister.AssemblerTemporary),
                Instruction.Create(OperationCode.BranchOnNotEquals, GPRegister.AssemblerTemporary, GPRegister.Zero, (short)Immediate)
            ],
            PseudoOp.LoadImmediate =>
            [
                Instruction.Create(OperationCode.LoadUpperImmediate, GPRegister.AssemblerTemporary, (short)(Immediate >> 16)),
                Instruction.Create(OperationCode.OrImmediate, RT, GPRegister.AssemblerTemporary, (short)Immediate)
            ],
            PseudoOp.AbsoluteValue =>
            [
                Instruction.Create(FunctionCode.AddUnsigned, RS, GPRegister.Zero, RT),
                Instruction.Create(RegImmFuncCode.BranchOnGreaterThanOrEqualToZero, RS, 8),
                Instruction.Create(FunctionCode.Subtract, GPRegister.Zero, RS, RT),
            ],
            PseudoOp.Move =>
            [
                Instruction.Create(FunctionCode.Add, RS, GPRegister.Zero, RT),
            ],
            PseudoOp.LoadAddress =>
            [
                Instruction.Create(OperationCode.LoadUpperImmediate, GPRegister.AssemblerTemporary, (short)(Address >> 16)),
                Instruction.Create(OperationCode.OrImmediate, RT, GPRegister.AssemblerTemporary, (short)Address)
            ],
            PseudoOp.SetGreaterThanOrEqual =>
            [
                Instruction.Create(OperationCode.AddImmediateUnsigned, RT, RT, (short)-1),
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
