// Adam Dernis 2024

using MIPS.Assembler.Models.Instructions;
using MIPS.Models.Instructions.Enums;
using MIPS.Models.Instructions.Enums.SpecialFunctions;
using System.Collections.Generic;

namespace MIPS.Assembler.Helpers.Tables;

public static partial class InstructionsTable
{
    private static readonly Argument[] LeadingCountPattern = [Argument.RD, Argument.RS];            // <instr>  $rd, $rs
    private static readonly Argument[] MulAddPattern = [Argument.RD, Argument.RS];                  // <instr>  $rd, $rs
    private static readonly Argument[] MulToGPRPattern = [Argument.RD, Argument.RS, Argument.RT];   // <instr>  $rd, $rs

    private static readonly Dictionary<string, InstructionMetadata> _special2InstructionTable = new()
    {
        { "clz",    new("clz",      Func2Code.CountLeadingZeros,            LeadingCountPattern,    Version.MipsItoV) }, // clz      $rd, $rs
        { "clo",    new("clo",      Func2Code.CountLeadingOnes,             LeadingCountPattern,    Version.MipsItoV) }, // clo      $rd, $rs

        { "madd",   new("madd",     Func2Code.MultiplyAndAddHiLow,          MulAddPattern,          Version.MipsItoV) }, // madd     $rd, $rs
        { "maddu",  new("maddu",    Func2Code.MultiplyAndAddHiLowUnsigned,  MulAddPattern,          Version.MipsItoV) }, // maddu    $rd, $rs
        { "msub",   new("msub",     Func2Code.MultiplyAndSubtractHiLow,     MulAddPattern,          Version.MipsItoV) }, // msub     $rd, $rs
        { "msubu",  new("msubu",    Func2Code.MultiplyAndAddHiLowUnsigned,  MulAddPattern,          Version.MipsItoV) }, // msubu    $rd, $rs

        { "mul",    new("mul",      Func2Code.MultiplyToGPR,                MulToGPRPattern,        Version.MipsItoV) }, // mul      $rd, $rs, $rt

        //{ "sdbbp", new("sdbbp", Func2Code.SoftwareDebugBreakpoint) }
    };
}
