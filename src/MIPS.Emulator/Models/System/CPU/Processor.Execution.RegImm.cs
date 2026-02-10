// Avishai Dernis 2025

using MIPS.Emulator.Models.System.Execution;
using MIPS.Emulator.Models.System.Execution.Enum;
using MIPS.Models.Instructions;
using MIPS.Models.Instructions.Enums.SpecialFunctions;
using System;

namespace MIPS.Emulator.System.CPU;

public partial class Processor
{
    delegate bool TrapIDelegate(uint rs, short rt);

    private Execution CreateExecutionRegImm(Instruction instruction)
    {
        return instruction.RTFuncCode switch
        {
            // Branch
            RegImmFuncCode.BranchOnLessThanZero or
            RegImmFuncCode.BranchOnLessThanZeroLikely => Branch(instruction, (rs, _) => rs < 0),
            RegImmFuncCode.BranchOnGreaterThanOrEqualToZero or
            RegImmFuncCode.BranchOnGreaterThanOrEqualToZeroLikely => Branch(instruction, (rs, _) => rs >= 0),

            // Trap
            RegImmFuncCode.TrapOnGreaterOrEqualImmediate => TrapI(instruction, (rs, imm) => (int)rs >= imm),
            RegImmFuncCode.TrapOnGreaterOrEqualImmediateUnisigned => TrapI(instruction, (rs, imm) => rs >= (ushort)imm),
            RegImmFuncCode.TrapOnLessThanImmediate => TrapI(instruction, (rs, imm) => (int)rs < imm),
            RegImmFuncCode.TrapOnLessThanImmediateUnsigned => TrapI(instruction, (rs, imm) => rs < (ushort)imm),
            RegImmFuncCode.TrapOnEqualsImmediate => TrapI(instruction, (rs, imm) => rs == imm),
            RegImmFuncCode.TrapOnNotEqualsImmediate => TrapI(instruction, (rs, imm) => rs != imm),

            _ => throw new NotImplementedException()
        };
    }

    private Execution TrapI(Instruction instruction, TrapIDelegate func)
    {
        var rs = RegisterFile[instruction.RS];
        var imm = instruction.ImmediateValue;

        if (func(rs, imm))
        {
            return new Execution(TrapKind.Trap);
        }

        return default;
    }
}
