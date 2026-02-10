// Avishai Dernis 2026

using MIPS.Emulator.Models;
using MIPS.Emulator.Models.Enum;
using MIPS.Models.Instructions;
using MIPS.Models.Instructions.Enums.SpecialFunctions;
using System;

namespace MIPS.Emulator.Executor;

/// <summary>
/// A class which handles converting decoded instructions into <see cref="Execution"/> models.
/// </summary>
public partial class InstructionExecutor
{
    private Execution CreateRegImmExecution()
    {
        return Instruction.RTFuncCode switch
        {
            // Branch
            RegImmFuncCode.BranchOnLessThanZero or
            RegImmFuncCode.BranchOnLessThanZeroLikely => Branch((rs, _) => rs < 0),
            RegImmFuncCode.BranchOnGreaterThanOrEqualToZero or
            RegImmFuncCode.BranchOnGreaterThanOrEqualToZeroLikely => Branch((rs, _) => rs >= 0),

            // Trap
            RegImmFuncCode.TrapOnGreaterOrEqualImmediate => TrapI((rs, imm) => (int)rs >= imm),
            RegImmFuncCode.TrapOnGreaterOrEqualImmediateUnisigned => TrapI((rs, imm) => rs >= (ushort)imm),
            RegImmFuncCode.TrapOnLessThanImmediate => TrapI((rs, imm) => (int)rs < imm),
            RegImmFuncCode.TrapOnLessThanImmediateUnsigned => TrapI((rs, imm) => rs < (ushort)imm),
            RegImmFuncCode.TrapOnEqualsImmediate => TrapI((rs, imm) => rs == imm),
            RegImmFuncCode.TrapOnNotEqualsImmediate => TrapI((rs, imm) => rs != imm),

            _ => throw new NotImplementedException()
        };
    }

    private Execution Branch(BranchDelegate func)
    {
        if (func(RS, RT))
        {
            return new Execution((uint)(Processor.ProgramCounter + Instruction.Offset));
        }

        return default;
    }

    private Execution TrapI(TrapIDelegate func)
    {
        if (func(RS, Instruction.ImmediateValue))
            Trap = TrapKind.Trap;

        return default;
    }
}
