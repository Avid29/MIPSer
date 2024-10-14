// Adam Dernis 2024

using MIPS.Assembler.Models.Instructions;
using MIPS.Models.Instructions.Enums;
using System.Collections.Generic;

namespace MIPS.Assembler.Helpers.Tables;

public static partial class InstructionsTable
{
    private static readonly Argument[] ShiftPattern = [Argument.RD, Argument.RT, Argument.Shift];       // <instr>  $rd, $rt, sa
    private static readonly Argument[] JumpRegisterPattern = [Argument.RS];                             // <instr>  $rs
    private static readonly Argument[] VariableShiftPattern = [Argument.RD, Argument.RT, Argument.RS];  // <instr>  $rd, $rt, $rs
    private static readonly Argument[] StandardRPattern = [Argument.RD, Argument.RS, Argument.RT];      // <instr>  $rd, $rs, $rt
    private static readonly Argument[] MultiplyRPattern = [Argument.RS, Argument.RT];                   // <instr>  $rs, $rt
    private static readonly Argument[] MoveFromPattern = [Argument.RD];                                 // <instr>  $rd
    private static readonly Argument[] MoveToPattern = [Argument.RS];                                   // <instr>  $rs
    private static readonly Argument[] TrapComparePattern = [Argument.RS, Argument.RT];                 // <instr>  $rs, $rt

    private static readonly Dictionary<string, InstructionMetadata> _rTypeInstructionTable = new()
    {
        { "sll",    new("sll",      FunctionCode.ShiftLeftLogical,              ShiftPattern) },                                // sll      $rd, $rt, sa
        { "srl",    new("srl",      FunctionCode.ShiftRightLogical,             ShiftPattern) },                                // ssl      $rd, $rt, sa
        { "sra",    new("sra",      FunctionCode.ShiftRightArithmetic,          ShiftPattern) },                                // sra      $rd, $rt, sa

        { "sllv",   new("sllv",     FunctionCode.ShiftLeftLogicalVariable,      VariableShiftPattern) },                        // sllv     $rd, $rt, $rs
        { "srlv",   new("srlv",     FunctionCode.ShiftRightLogicalVariable,     VariableShiftPattern) },                        // srlv     $rd, $rt, $rs
        { "srav",   new("srav",     FunctionCode.ShiftRightArithmeticVariable,  VariableShiftPattern) },                        // srav     $rd, $rt, $rs

        { "jr",     new("jr",       FunctionCode.JumpRegister,                  JumpRegisterPattern) },                         // jr       $rs
        { "jalr",   new("jalr",     FunctionCode.JumpAndLinkRegister,           JumpRegisterPattern) },                         // jalr     $rs

        { "syscall",new("syscall",  FunctionCode.SystemCall,                    []) },                                          // syscall
        { "break",  new("break",    FunctionCode.SystemCall,                    []) },                                          // break
        { "sync",   new("sync",     FunctionCode.Sync,                          [],                     Version.MipsIIUp) },    // sync
        
        { "mfhi",   new("mfhi",     FunctionCode.MoveFromHigh,                  MoveFromPattern) },                             // mfhi     $rd
        { "mthi",   new("mthi",     FunctionCode.MoveToHigh,                    MoveToPattern) },                               // mthi     $rs
        { "mflo",   new("mflo",     FunctionCode.MoveFromLow,                   MoveFromPattern) },                             // mflo     $rd
        { "mtlo",   new("mtlo",     FunctionCode.MoveToLow,                     MoveToPattern) },                               // mtlo     $rs

        { "mult",   new("mult",     FunctionCode.Multiply,                      MultiplyRPattern) },                            // mult     $rs, $rt
        { "multu",  new("multu",    FunctionCode.MultiplyUnsigned,              MultiplyRPattern) },                            // multu    $rs, $rt
        { "div",    new("div",      FunctionCode.Divide,                        MultiplyRPattern) },                            // div      $rs, $rt
        { "divu",   new("divu",     FunctionCode.DivideUnsigned,                MultiplyRPattern) },                            // divu     $rs, $rt

        { "add",    new("add",      FunctionCode.Add,                           StandardRPattern) },                            // add      $rd, $rs, $rt
        { "addu",   new("addu",     FunctionCode.AddUnsigned,                   StandardRPattern) },                            // addu     $rd, $rs, $rt
        { "sub",    new("sub",      FunctionCode.Subtract,                      StandardRPattern) },                            // sub      $rd, $rs, $rt
        { "subu",   new("subu",     FunctionCode.SubtractUnsigned,              StandardRPattern) },                            // subu     $rd, $rs, $rt

        { "and",    new("and",      FunctionCode.And,                           StandardRPattern) },                            // and      $rd, $rs, $rt
        { "or",     new("or",       FunctionCode.Or,                            StandardRPattern) },                            // or       $rd, $rs, $rt
        { "xor",    new("xor",      FunctionCode.ExclusiveOr,                   StandardRPattern) },                            // xor      $rd, $rs, $rt
        { "nor",    new("nor",      FunctionCode.Nor,                           StandardRPattern) },                            // nor      $rd, $rs, $rt

        { "slt",    new("slt",      FunctionCode.SetLessThan,                   StandardRPattern) },                            // slt      $rd, $rs, $rt
        { "sltu",   new("sltu",     FunctionCode.SetLessThanUnsigned,           StandardRPattern) },                            // sltu     $rd, $rs, $rt

        { "tge",    new("tge",      FunctionCode.TrapOnGreaterOrEqual,          TrapComparePattern,     Version.MipsIIUp) },
    };
}
