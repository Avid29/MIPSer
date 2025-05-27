// Adam Dernis 2024

using MIPS.Models.Instructions.Enums.Operations;
using MIPS.Models.Instructions.Enums.Registers;

namespace MIPS.Models.Instructions.Enums.SpecialFunctions;

/// <summary>
/// These values go in the <see cref="Argument.RT"/> field of instructions with <see cref="OperationCode.RegisterImmediate"/>.
/// </summary>
public enum RegImmFuncCode : byte
{
    /// <summary>
    /// Marks that there is no function code.
    /// </summary>
    /// <remarks>
    /// This value is too large to encode in a real instruction. If by accident
    /// this were encoded into an <see cref="Instruction"/> struct, it would become 
    /// <see cref="BranchOnLessThanZero"/> (or <see cref="Register.Zero"/>) upon unencoding.
    /// </remarks>
    None = 0x20,

#pragma warning disable CS1591

    BranchOnLessThanZero = 0x00,
    BranchOnGreaterThanOrEqualToZero = 0x01,
    BranchOnLessThanZeroLikely = 0x02,
    BranchOnGreaterThanZeroLikely = 0x03,

    TrapOnGreaterOrEqualImmediate = 0x08,
    TrapOnGreaterOrEqualImmediateUnisigned = 0x09,
    TrapOnLessThanImmediate = 0x0a,
    TrapOnLessThanImmediateUnisigned = 0x0b,
    TrapOnEqualsImmediate = 0x0c,
    TrapOnNotEqualsImmediate = 0x0e,

    BranchOnLessThanZeroAndLink = 0x10,
    BranchOnGreaterThanOrEqualToZeroAndLink = 0x11,
    BranchOnLessThanZeroLikelyAndLink = 0x12,
    BranchOnGreaterThanOrEqualToZeroLikelyAndLink = 0x13,

#pragma warning restore CS1591
}
