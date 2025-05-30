﻿// Adam Dernis 2024

using MIPS.Models.Instructions;
using MIPS.Models.Instructions.Enums;
using MIPS.Models.Instructions.Enums.Operations;
using MIPS.Models.Instructions.Enums.SpecialFunctions;
using MIPS.Models.Instructions.Enums.SpecialFunctions.CoProc0;

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
    /// <param name="rsFuncCode">The rsFuncCode of the instruction.</param>
    /// <returns>The <see cref="InstructionType"/> associated to an <see cref="Instruction"/>.</returns>
    public static InstructionType GetInstructionType(OperationCode? opCode, RegImmFuncCode? rtFuncCode = null, CoProc0RSCode? rsFuncCode = null)
    {
        if (!opCode.HasValue)
            return InstructionType.Pseudo;

        return opCode switch
        {
            // R Type 
            OperationCode.Special => InstructionType.BasicR,
            OperationCode.Special2 => InstructionType.Special2R,
            OperationCode.Special3 => InstructionType.Special3R,

            OperationCode.RegisterImmediate
                when rtFuncCode is <= RegImmFuncCode.BranchOnGreaterThanZeroLikely
                or >= RegImmFuncCode.BranchOnLessThanZeroAndLink => InstructionType.RegisterImmediateBranch,

            OperationCode.RegisterImmediate => InstructionType.RegisterImmediate,

            OperationCode.Jump or
            OperationCode.JumpAndLink => InstructionType.BasicJ,

            OperationCode.Coprocessor0
                when rsFuncCode.HasValue => rsFuncCode.Value switch
            {
                CoProc0RSCode.MFMC0 => InstructionType.Coproc0MFMC0,
                CoProc0RSCode.C0 => InstructionType.Coproc0C0,
                _ => InstructionType.Coproc0,
            },
            OperationCode.Coprocessor1 => InstructionType.Float,
            _ => InstructionType.BasicI,
        };
    }

    /// <summary>
    /// Gets the <see cref="InstructionPattern"/> of an <see cref="Instruction"/>.
    /// </summary>
    /// <param name="opCode">The instruction to get the type of.</param>
    /// <returns>The <see cref="InstructionPattern"/> associated to an <see cref="Instruction"/>.</returns>
    public static InstructionPattern GetInstructionPattern(OperationCode? opCode)
    {
        if (!opCode.HasValue)
            return InstructionPattern.Other;

        return opCode switch
        {
            OperationCode.Special => InstructionPattern.R,
            OperationCode.RegisterImmediate => InstructionPattern.R,
            OperationCode.Special2 => InstructionPattern.R,
            OperationCode.Special3 => InstructionPattern.R,
            OperationCode.Jump or
            OperationCode.JumpAndLink => InstructionPattern.J,
            OperationCode.Coprocessor0 => InstructionPattern.Other,
            _ => InstructionPattern.I,
        };
    }
}
