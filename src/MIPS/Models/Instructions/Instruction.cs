// Adam Dernis 2024

using MIPS.Models.Instructions.Enums;
using MIPS.Models.Instructions.Enums.Registers;
using MIPS.Models.Instructions.Enums.SpecialFunctions;
using System.Runtime.CompilerServices;

namespace MIPS.Models.Instructions;

//                               MIPS Instructions
// ----------------------------------------------------------------------------
//      All instructions in MIPS are 4-byte, or 32 bits.
//
// There are 3 types of instructions.
// - R type
// - I type
// - J type.
// 
//                            R Type Instructions Summary
// ----------------------------------------------------------------------------
//      R type instructions split the field into an Operation Code (6 bits),
// RS register (5 bits), RT register (5 bits), RD register (5 bits),
// Shift Amount (5 bits), and Function Code (6 bits).
//
//
//                           R Type Instruction Components
// ----------------------------------------------------------------------------
//
// Operation Code:
//      R type instructions have an OP code of 0, and the function code is instead
//      used to differentiate instructions.
// 
// RS Register:
//      R type instructions *usually* use RS as the first input register argument,
//      immediate shift instructions being the exception.
// 
// RT Register:
//      R type instructions *usually* use RT as the second input register argument,
//      immediate shift instruction again being the exception.
//
// RD Register:
//      RD is the write back register of the value calculated by the instruction.
//
// Shift Amount:
//      Shift amount is only used for immediate shift instructions. It specifies the
//      number of bits (unsigned) to shift in a given direction.
// 
// Function Code:
//      The function code is used to differentiate R type instructions.
//
//                       R Type Instruction Assembled Examples
// ----------------------------------------------------------------------------
// > add $t0, $s0, $s1
//         |  Oper  |  $rs  |  $rt  |  $rd  | Shift |  Func  |
//  ------ + ------ + ----- + ----- + ----- + ----- + ------ |
// Binary  | 000000 | 10000 | 10001 | 01000 | 00000 | 100000 |
// Hex     |     00 |    10 |    11 |    08 |    00 |     20 |
// Decimal |      0 |    16 |    17 |     8 |     0 |     32 |
// ------- + ------ + ----- + ----- + ----- + ----- + ------ +
// Meaning | R Type |   $s0 |   $s1 |   $t0 |   N/A |    add |
// ------- + ------ + ----- + ----- + ----- + ----- + ------ +
//
//
// > sll $t0, $s0, 3
//         |  Oper  |  $rs  |  $rt  |  $rd  | Shift |  Func  |
//  ------ + ------ + ----- + ----- + ----- + ----- + ------ |
// Binary  | 000000 | 00000 | 10000 | 01000 | 00011 | 000000 |
// Hex     |     00 |    00 |    10 |    08 |    03 |     00 |
// Decimal |      0 |     0 |    16 |     8 |     3 |      0 |
// ------- + ------ + ----- + ----- + ----- + ----- + ------ +
// Meaning | R Type |   N/A |   $s0 |   $t0 |     3 |    sll |
// ------- + ------ + ----- + ----- + ----- + ----- + ------ +
//
// 
//                            I Type Instructions Summary
// ----------------------------------------------------------------------------
//      I type instructions split the field into an Operation Code (6 bits),
// RS register (5 bits), RT register (5 bits), and an immediate value (16 bits).
//
//
//                           I Type Instruction Components
// ----------------------------------------------------------------------------
//
// Operation Code:
//      The OP code is used to identify the instruction to run.
// 
// RS Register:
//      I type instructions use RS as the first input register argument if required.
// 
// RT Register:
//      I type instructions use RT as the write back register, except in memory saving
//      and in branch comparing where two argument registers are required.

// Immediate Value:
//      The function code is used to differentiate R type instructions.
//
//                       I Type Instruction Assembled Examples
// ----------------------------------------------------------------------------
// > addi $t0, $s0, 20
//         |  Oper  |  $rs  |  $rt  | Immediate Value  |
//  ------ + ------ + ----- + ----- + ---------------- |
// Binary  | 001000 | 10000 | 01000 | 0000000000010100 |
// Hex     |     08 |    10 |    08 |               14 |
// Decimal |      8 |    16 |     8 |               20 |
// ------- + ------ + ----- + ----- + ---------------- +
// Meaning |   addi |   $s0 |   $t0 |               20 |
// ------- + ------ + ----- + ----- + ---------------- +
//
//
// > bltz $a0, $t0, (label of +64)
//         |  Oper  |  $rs  |  $rt  | Immediate Value  |
//  ------ + ------ + ----- + ----- + ---------------- |
// Binary  | 000110 | 00100 | 01000 | 0000000000010000 |
// Hex     |     06 |    04 |    08 |               10 |
// Decimal |      6 |    04 |     8 |               16 |
// ------- + ------ + ----- + ----- + ---------------- +
// Meaning |   bltz |   $a0 |   $t0 |               64 |
// ------- + ------ + ----- + ----- + ---------------- +
//
// Note: The meaning of the immediate value is 4x the actual value because the instructions are aligned
// to the 4 byte boundary. As a result, the last 4 bits can be dropped and can be utilized in the front
// to represent an effectively 20 bit offset.
//
//
// > sw $s0, 24($sp)
//         |  Oper  |  $rs  |  $rt  | Immediate Value  |
//  ------ + ------ + ----- + ----- + ---------------- |
// Binary  | 000110 | 11101 | 10000 | 0000000000011000 |
// Hex     |     06 |    1D |    10 |               18 |
// Decimal |      6 |    29 |    16 |               24 |
// ------- + ------ + ----- + ----- + ---------------- +
// Meaning |     sw |   $sp |   $s0 |               24 |
// ------- + ------ + ----- + ----- + ---------------- +
//
//
// 
//                            J Type Instructions Summary
// ----------------------------------------------------------------------------
//      I type instructions split the field into an Operation Code (6 bits),
//  and an address value (26 bits).
//
//                           J Type Instruction Components
// ----------------------------------------------------------------------------
//
// Operation Code:
//      The OP code is used to identify the instruction to run.

// Address:
//      The jump address. This address must be word aligned, so the last 2 bits are dropped, making
//      it effectively a 28 bit address. The remaining 4 bits are assumed to be equivalent to that
//      of the current program counter.
//
//                       J Type Instruction Assembled Examples
// ----------------------------------------------------------------------------
// > j (label at 9,912)
//         |  Oper  |           Address          |
//  ------ + ------ + -------------------------- |
// Binary  | 000010 | 00000000000000100110101110 |
// Hex     |     02 |                       09AE |
// Decimal |      2 |                       2478 |
// ------- + ------ + -------------------------- +
// Meaning |      j |                       9912 |
// ------- + ------ + -------------------------- +
//
//
// > jal (label at 6,808)
//         |  Oper  |           Address          |
//  ------ + ------ + -------------------------- |
// Binary  | 000011 | 00000000000000011010100110 |
// Hex     |     03 |                       06A6 |
// Decimal |      3 |                       1702 |
// ------- + ------ + -------------------------- +
// Meaning |    jal |                       6808 |
// ------- + ------ + -------------------------- +

/// <summary>
/// A struct representing an instruction.
/// </summary>
public partial struct Instruction
{
    private uint _inst;

    /// <summary>
    /// Creates a new r-type instruction.
    /// </summary>
    public static Instruction Create(FunctionCode funcCode, Register rs, Register rt, Register rd, byte shiftAmount = 0)
    {
        Instruction value = default;
        value.OpCode = OperationCode.Special;
        value.RS = rs;
        value.RT = rt;
        value.RD = rd;
        value.ShiftAmount = shiftAmount;
        value.FuncCode = funcCode;
        return value;
    }
    
    /// <summary>
    /// Creates a new special2 instruction.
    /// </summary>
    public static Instruction Create(Func2Code func2Code, Register rs, Register rt, Register rd, byte shiftAmount = 0)
    {
        Instruction value = default;
        value.OpCode = OperationCode.Special2;
        value.RS = rs;
        value.RT = rt;
        value.RD = rd;
        value.ShiftAmount = shiftAmount;
        value.Func2Code = func2Code;
        return value;
    }

    /// <summary>
    /// Creates a new i-type instruction.
    /// </summary>
    public static Instruction Create(OperationCode opCode, Register rs, Register rt, short immediate)
    {
        Instruction value = default;
        value.OpCode = opCode;
        value.RS = rs;
        value.RT = rt;
        value.ImmediateValue = immediate;
        return value;
    }

    /// <summary>
    /// Creates a new i-type instruction.
    /// </summary>
    /// <remarks>
    /// This is just for load upper immediate.
    /// </remarks>
    public static Instruction Create(OperationCode opCode, Register rt, short immediate)
    {
        Instruction value = default;
        value.OpCode = opCode;
        value.RT = rt;
        value.ImmediateValue = immediate;
        return value;
    }

    /// <summary>
    /// Creates a new j-type instruction.
    /// </summary>
    public static Instruction Create(OperationCode opCode, uint address)
    {
        Instruction value = default;
        value.OpCode = opCode;
        value.Address = address;
        return value;
    }
    
    /// <summary>
    /// Creates a new <see cref="OperationCode.RegisterImmediate"/> instruction.
    /// </summary>
    public static Instruction Create(RegImmFuncCode code, Register rs, short immediate)
    {
        Instruction value = default;
        value.OpCode = OperationCode.RegisterImmediate;
        value.RTFuncCode = code;
        value.RS = rs;
        value.ImmediateValue = immediate;
        return value;
    }

    /// <summary>
    /// Creates a new <see cref="OperationCode.RegisterImmediate"/> branch instruction.
    /// </summary>
    public static Instruction Create(RegImmFuncCode code, Register rs, int offset)
    {
        Instruction value = default;
        value.OpCode = OperationCode.RegisterImmediate;
        value.RTFuncCode = code;
        value.RS = rs;
        value.Offset = offset;
        return value;
    }

    /// <summary>
    /// Casts a <see cref="uint"/> to a <see cref="Instruction"/>.
    /// </summary>
    public static unsafe explicit operator Instruction(uint value) => Unsafe.As<uint, Instruction>(ref value);

    /// <summary>
    /// Casts a <see cref="uint"/> to a <see cref="Instruction"/>.
    /// </summary>
    public static unsafe explicit operator uint(Instruction value) => Unsafe.As<Instruction, uint>(ref value);
}
