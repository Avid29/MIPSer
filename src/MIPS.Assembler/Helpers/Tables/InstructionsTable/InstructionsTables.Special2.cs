// Adam Dernis 2024

using MIPS.Assembler.Models.Instructions;
using MIPS.Models.Instructions.Enums;
using System.Collections.Generic;

namespace MIPS.Assembler.Helpers.Tables;

public static partial class InstructionsTable
{
    private static readonly Argument[] LeadingCountPattern = [Argument.RD, Argument.RS];    // <instr>  $rd, $rs

    private static readonly Dictionary<string, InstructionMetadata> _special2InstructionTable = new()
    {
        { "clz",    new("clz",      Func2Code.CountLeadingZeros,            LeadingCountPattern) }, // clz      $rd, $rs
        { "clo",    new("clo",      Func2Code.CountLeadingOnes,             LeadingCountPattern) }, // clo      $rd, $rs

        { "madd",   new("madd",     Func2Code.MultiplyAndAddHiLow,          MultiplyRPattern) },    // madd     $rd, $rs
        { "maddu",  new("maddu",    Func2Code.MultiplyAndAddHiLowUnsigned,  MultiplyRPattern) },    // maddu    $rd, $rs
        { "msub",   new("msub",     Func2Code.MultiplyAndSubtractHiLow,     MultiplyRPattern) },    // msub     $rd, $rs
        { "msubu",  new("msubu",    Func2Code.MultiplyAndAddHiLowUnsigned,  MultiplyRPattern) },    // msubu    $rd, $rs

        { "mul",    new("mul",      Func2Code.MultiplyToGPR,                StandardRPattern) },    // mul      $rd, $rs, $rt

        //{ "sdbbp", new("sdbbp", Func2Code.SoftwareDebugBreakpoint) }
    };
}
