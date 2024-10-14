// Adam Dernis 2024

using MIPS.Assembler.Models.Instructions;
using MIPS.Models.Instructions.Enums;
using System.Collections.Generic;

namespace MIPS.Assembler.Helpers.Tables;

public static partial class InstructionsTable
{
    // J type patterns
    private static readonly Argument[] JumpPattern = [Argument.Address];    // <instr>  addr

    private static readonly Dictionary<string, InstructionMetadata> _jTypeInstructionTable = new()
    {
        // J Type
        { "j",      new("j",    OperationCode.Jump,         JumpPattern) }, // j        addr
        { "jal",    new("jal",  OperationCode.JumpAndLink,  JumpPattern) }, // jal      addr
    };
}
