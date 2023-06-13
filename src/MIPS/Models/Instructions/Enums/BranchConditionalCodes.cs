// Adam Dernis 2023

namespace MIPS.Models.Instructions.Enums;

/// <summary>
/// These values go in the <see cref="Argument.RT"/> field of instructions with <see cref="OperationCode.BranchConditional"/>.
/// </summary>
public enum BranchConditionalCodes
{
    #pragma warning disable CS1591

    BranchOnLessThanZero = 0x00,
    BranchOnGreaterOrEqualToThanZero = 0x01,

    BranchOnLessThanZeroAndLink = 0x10,
    BranchOnGreaterThanOrEqualToZeroAndLink = 0x11,

    #pragma warning restore CS1591
}
