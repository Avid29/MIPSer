// Adam Dernis 2024

using MIPS.Models.Instructions;
using MIPS.Models.Instructions.Enums;
using MIPS.Models.Instructions.Enums.SpecialFunctions;

namespace MIPS.Helpers.Instructions;

/// <summary>
/// A static class containing helper methods related to instruction types.
/// </summary>
public static class InstructionTypeHelper
{
    /// <summary>
    /// Gets the <see cref="InstructionType"/> of an <see cref="Instruction"/>.
    /// </summary>
    /// <param name="opCode">The instruction to get the type of.</param>
    /// <param name="rtFuncCode">The rtFunction of the instruction.</param>
    /// <returns>The <see cref="InstructionType"/> associated to an <see cref="Instruction"/>.</returns>
    public static InstructionType GetInstructionType(OperationCode opCode, RTFuncCode rtFuncCode)
    {
        return opCode switch
        {
            OperationCode.Special => InstructionType.BasicR,
            OperationCode.RegisterImmediate when rtFuncCode is <= RTFuncCode.BranchOnGreaterThanZeroLikely
            or >= RTFuncCode.BranchOnLessThanZeroAndLink => InstructionType.RegisterImmediateBranch,
            OperationCode.RegisterImmediate => InstructionType.RegisterImmediateBranch,
            OperationCode.Special2 => InstructionType.Special2R,
            OperationCode.Jump or
            OperationCode.JumpAndLink => InstructionType.BasicJ,
            _ => InstructionType.BasicI,
        };
    }
}
