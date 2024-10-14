// Adam Dernis 2024

using MIPS.Assembler.Models.Instructions;
using MIPS.Models.Instructions.Enums;
using System.Collections.Generic;

namespace MIPS.Assembler.Helpers.Tables;

public static partial class InstructionsTable
{
    private static readonly Argument[] TrapImmediatePattern = [Argument.RS, Argument.Immediate];    // <instr>  $rs, offset
    
    private static readonly Dictionary<string, InstructionMetadata> _regImmInstructionTable = new()
    {
        { "bltz",       new("bltz",     RegImmCode.BranchOnLessThanZero,                           BranchPattern) },                               // bltz     $rs, offset
        { "bgez",       new("bgez",     RegImmCode.BranchOnGreaterOrEqualToThanZero,               BranchPattern) },                               // bgez     $rs, offset
        { "bltzl",      new("bltzl",    RegImmCode.BranchOnLessThanZeroLikely,                     BranchPattern,          Version.MipsIIUp) },    // bltzl    $rs, offset
        { "bgezl",      new("bgezl",    RegImmCode.BranchOnGreaterThanZeroLikely,                  BranchPattern,          Version.MipsIIUp) },    // bgezl    $rs, offset
                                                                                                                                             
        { "tegi",       new("tegi",     RegImmCode.TrapOnGreaterOrEqualImmediate,                  TrapImmediatePattern,   Version.MipsIIUp) },    // tge      $rs, imm
        { "tegiu",      new("tegiu",    RegImmCode.TrapOnGreaterOrEqualImmediateUnisigned,         TrapImmediatePattern,   Version.MipsIIUp) },    // tegiu    $rs, imm
        { "tlti",       new("tlti",     RegImmCode.TrapOnLessThanImmediate,                        TrapImmediatePattern,   Version.MipsIIUp) },    // tlti     $rs, imm
        { "tltiu",      new("tltiu",    RegImmCode.TrapOnLessThanImmediateUnisigned,               TrapImmediatePattern,   Version.MipsIIUp) },    // tlltiu   $rs, imm
        { "teqi",       new("teqi",     RegImmCode.TrapOnEqualsImmediate,                          TrapImmediatePattern,   Version.MipsIIUp) },    // teqi     $rs, imm
        { "tnei",       new("tnei",     RegImmCode.TrapOnNotEqualsImmediate,                       TrapImmediatePattern,   Version.MipsIIUp) },    // tnei     $rs, imm

        { "bltzal",     new("bltzal",  RegImmCode.BranchOnLessThanZeroAndLink,                     BranchPattern)},                                // bltzal   $rs, offset
        { "bgezal",     new("bgezal",  RegImmCode.BranchOnGreaterThanOrEqualToZeroAndLink,         BranchPattern)},                                // bgezal   $rs, offset
        { "blezall",    new("blezall", RegImmCode.BranchOnLessThanZeroLikelyAndLink,               BranchPattern)},                                // bltzall  $rs, offset
        { "bgezall",    new("bgezal",  RegImmCode.BranchOnGreaterThanOrEqualToZeroLikelyAndLink,   BranchPattern,           Version.MipsIIUp) },   // bgezall  $rs, offset
    };
}
