// Adam Dernis 2024

namespace MIPS.Models.Instructions.Enums;

/// <summary>
/// These values go in the <see cref="Argument.RT"/> field of instructions with <see cref="OperationCode.BranchConditional"/>.
/// </summary>
public enum BranchCode : byte
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
    BranchOnGreaterOrEqualToThanZero = 0x01,

    BranchOnLessThanZeroAndLink = 0x10,
    BranchOnGreaterThanOrEqualToZeroAndLink = 0x11,

#pragma warning restore CS1591
}
