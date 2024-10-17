// Adam Dernis 2024

using MIPS.Assembler.Models.Instructions;
using MIPS.Models.Instructions.Enums;
using System.Collections.Generic;

namespace MIPS.Assembler.Helpers.Tables;

public static partial class InstructionsTable
{
    // Pseudo-Instruction patterns
    private static readonly Argument[] BranchOnLessThanPattern = [Argument.RS, Argument.RT, Argument.Offset];
    private static readonly Argument[] LoadFullImmediatePattern = [Argument.RT, Argument.FullImmediate];
    private static readonly Argument[] AbsoluteValuePattern = [Argument.RT, Argument.RS];
    private static readonly Argument[] MovePattern = [Argument.RT, Argument.RS];
    private static readonly Argument[] SetGreaterThanOrEqualPattern = [Argument.RD, Argument.RS, Argument.RT];

    /// <summary>
    /// Pseudo Instruction table.
    /// </summary>
    private static readonly Dictionary<string, InstructionMetadata> _pseudoInstructionTable = new()
    {
        { "nop",    new("nop",      PseudoOp.NoOperation,               [],                             1) },   // nop
        
        { "ssnop",  new("ssnop",    PseudoOp.SuperScalarNoOperation,    [],                             1) },   // ssnop

        { "bal",    new("bal",      PseudoOp.BranchAndLink,             [],                             1)},    // bal      $rs, offset

        { "blt",    new("blt",      PseudoOp.BranchOnLessThan,          BranchOnLessThanPattern,        2) },   // blt      $rs, $rt, offset
                                                                                                                //    slt   $at, $rs, $rt     
                                                                                                                //    bne   $at, $zero, offset

        { "li",     new("li",       PseudoOp.LoadImmediate,             LoadFullImmediatePattern,       2) },   // li       $rt, immediate
                                                                                                                //    lui   $at, upper
                                                                                                                //    ori   $rt, $at, lower

        { "abs",    new("abs",      PseudoOp.AbsoluteValue,             AbsoluteValuePattern,           3) },   // abs      $rt, $rs
                                                                                                                //    addu  $rt, $rs, $zero
                                                                                                                //    bgez  $rs, 8
                                                                                                                //    sub   $rt, $zero, $rs

        { "move",   new("move",     PseudoOp.Move,                      MovePattern,                    1) },   // move     $rt, $rs
                                                                                                                //    add     $rt, $rs, $zero

        { "la",     new("la",       PseudoOp.LoadAddress,               LoadFullImmediatePattern,       2) },   // la       $rt, address
                                                                                                                //    lui   $at, upper
                                                                                                                //    ori   $rt, $at, lower

        { "sge",    new("sge",      PseudoOp.SetGreaterThanOrEqual,     SetGreaterThanOrEqualPattern,   2) },   // sge      $rd, $rs, $rt
                                                                                                                //    addiu $rt, $rt, -1
                                                                                                                //    slt   $rd, $rt, $rs
    };
}
