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
        { "bltz",       new("bltz",     RTFuncCode.BranchOnLessThanZero,                           BranchPattern) },                               // bltz     $rs, offset
        { "bgez",       new("bgez",     RTFuncCode.BranchOnGreaterOrEqualToThanZero,               BranchPattern) },                               // bgez     $rs, offset
        { "bltzl",      new("bltzl",    RTFuncCode.BranchOnLessThanZeroLikely,                     BranchPattern,          Version.MipsIIUp) },    // bltzl    $rs, offset
        { "bgezl",      new("bgezl",    RTFuncCode.BranchOnGreaterThanZeroLikely,                  BranchPattern,          Version.MipsIIUp) },    // bgezl    $rs, offset
                                                                                                                                             
        { "tegi",       new("tegi",     RTFuncCode.TrapOnGreaterOrEqualImmediate,                  TrapImmediatePattern,   Version.MipsIIUp) },    // tge      $rs, imm
        { "tegiu",      new("tegiu",    RTFuncCode.TrapOnGreaterOrEqualImmediateUnisigned,         TrapImmediatePattern,   Version.MipsIIUp) },    // tegiu    $rs, imm
        { "tlti",       new("tlti",     RTFuncCode.TrapOnLessThanImmediate,                        TrapImmediatePattern,   Version.MipsIIUp) },    // tlti     $rs, imm
        { "tltiu",      new("tltiu",    RTFuncCode.TrapOnLessThanImmediateUnisigned,               TrapImmediatePattern,   Version.MipsIIUp) },    // tlltiu   $rs, imm
        { "teqi",       new("teqi",     RTFuncCode.TrapOnEqualsImmediate,                          TrapImmediatePattern,   Version.MipsIIUp) },    // teqi     $rs, imm
        { "tnei",       new("tnei",     RTFuncCode.TrapOnNotEqualsImmediate,                       TrapImmediatePattern,   Version.MipsIIUp) },    // tnei     $rs, imm

        { "bltzal",     new("bltzal",   RTFuncCode.BranchOnLessThanZeroAndLink,                    BranchPattern)},                             // bltzal   $rs, offset
        { "bgezal",     new("bgezal",   RTFuncCode.BranchOnGreaterThanOrEqualToZeroAndLink,        BranchPattern)},                             // bgezal   $rs, offset
        { "blezall",    new("blezall",  RTFuncCode.BranchOnLessThanZeroLikelyAndLink,              BranchPattern)},                             // bltzall  $rs, offset
        { "bgezall",    new("bgezal",   RTFuncCode.BranchOnGreaterThanOrEqualToZeroLikelyAndLink,  BranchPattern,         Version.MipsIIUp) },  // bgezall  $rs, offset
    };
}
