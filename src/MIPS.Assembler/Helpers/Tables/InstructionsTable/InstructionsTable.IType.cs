// Adam Dernis 2024

using MIPS.Assembler.Models.Instructions;
using MIPS.Models.Instructions.Enums;
using System.Collections.Generic;

namespace MIPS.Assembler.Helpers.Tables;

public static partial class InstructionsTable
{
    private static readonly Argument[] StandardIPattern = [Argument.RT, Argument.RS, Argument.Immediate];  // <instr>  $rt, $rs, imm
    private static readonly Argument[] BranchComparePattern = [Argument.RS, Argument.RT, Argument.Offset]; // <instr>  $rs, $rt, offset
    private static readonly Argument[] LoadImmediatePattern = [Argument.RT, Argument.Immediate];           // <instr>  $rt, imm
    private static readonly Argument[] MemoryPattern = [Argument.RT, Argument.AddressOffset];              // <instr>  $rt, offset($rs)
    private static readonly Argument[] BranchPattern = [Argument.RS, Argument.Offset];                     // <instr>  $rs, offset
    
    /// <summary>
    /// I-Type instruction table.
    /// </summary>
    private static readonly Dictionary<string, InstructionMetadata> _iTypeInstructionTable = new()
    {
        { "beq",    new("beq",      OperationCode.BranchOnEquals,                       BranchComparePattern) },                    // beq      $rs, $rt, offset
        { "bne",    new("bne",      OperationCode.BranchOnNotEquals,                    BranchComparePattern) },                    // bne      $rs, $rt, offset
        { "blez",   new("blez",     OperationCode.BranchOnLessThanOrEqualToZero,        BranchPattern) },                           // blez     $rs, offset
        { "bgtz",   new("bgtz",     OperationCode.BranchGreaterThanZero,                BranchPattern) },                           // bgtz     $rs, offset

        { "addi",   new("addi",     OperationCode.AddImmediate,                         StandardIPattern) },                        // addi     $rt, $rs, imm
        { "addiu",  new("addiu",    OperationCode.AddImmediateUnsigned,                 StandardIPattern) },                        // addiu    $rt, $rs, imm

        { "slti",   new("slti",     OperationCode.SetLessThanImmediate,                 StandardIPattern) },                        // slti     $rt, $rs, imm
        { "sltiu",  new("sltiu",    OperationCode.SetLessThanImmediateUnsigned,         StandardIPattern) },                        // sltiu    $rt, $rs, imm

        { "andi",   new("andi",     OperationCode.AndImmediate,                         StandardIPattern) },                        // andi     $rt, $rs, imm
        { "ori",    new("ori",      OperationCode.OrImmediate,                          StandardIPattern) },                        // ori      $rt, $rs, imm
        { "xori",   new("xori",     OperationCode.ExclusiveOrImmediate,                 StandardIPattern) },                        // xori     $rt, $rs, imm

        { "lui",    new("lui",      OperationCode.LoadUpperImmediate,                   LoadImmediatePattern) },                    // lui      $rt, imm

        { "beql",   new("beql",     OperationCode.BranchOnEqualLikely,                  BranchPattern,      Version.MipsIIUp)},     // beql     $rs, offset
        { "bnel",   new("bnel",     OperationCode.BranchOnNotEqualLikely,               BranchPattern,      Version.MipsIIUp)},     // bnel     $rs, offset
        { "blezl",  new("blezl",    OperationCode.BranchOnLessThanOrEqualToZeroLikely,  BranchPattern,      Version.MipsIIUp)},     // blezl    $rs, offset
        { "bgtzl",  new("bgtzl",    OperationCode.BranchOnGreaterThanZeroLikely,        BranchPattern,      Version.MipsIIUp)},     // bgtzl    $rs, offset

        { "lb",     new("lb",       OperationCode.LoadByte,                             MemoryPattern) },                           // lb       $rt, offset($rs)
        { "lh",     new("lh",       OperationCode.LoadHalfWord,                         MemoryPattern) },                           // lh       $rt, offset($rs)
        { "lwl",    new("lwl",      OperationCode.LoadWordLeft,                         MemoryPattern) },                           // lwl      $rt, offset($rs)
        { "lw",     new("lw",       OperationCode.LoadWord,                             MemoryPattern) },                           // lw       $rt, offset($rs)
        { "lbu",    new("lbu",      OperationCode.LoadByteUnsigned,                     MemoryPattern) },                           // lbu      $rt, offset($rs)
        { "lhu",    new("lhu",      OperationCode.LoadHalfWordUnsigned,                 MemoryPattern) },                           // lhu      $rt, offset($rs)
        { "lwr",    new("lwr",      OperationCode.LoadWordRight,                        MemoryPattern) },                           // lwr      $rt, offset($rs)
                                                                                                                                           
        { "sb",     new("sb",       OperationCode.StoreByte,                            MemoryPattern) },                           // sb       $rt, offset($rs)
        { "sh",     new("sh",       OperationCode.StoreHalfWord,                        MemoryPattern) },                           // sh       $rt, offset($rs)
        { "swl",    new("swl",      OperationCode.StoreWordLeft,                        MemoryPattern) },                           // swl      $rt, offset($rs)
        { "sw",     new("sw",       OperationCode.StoreWord,                            MemoryPattern) },                           // sw       $rt, offset($rs)
        { "swr",    new("swr",      OperationCode.StoreWordRight,                       MemoryPattern) },                           // swr      $rt, offset($rs)
    };
}
