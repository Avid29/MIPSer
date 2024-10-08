// Adam Dernis 2023

using MIPS.Helpers.Instructions;
using MIPS.Models.Instructions.Enums;

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
//      The jump address. This address must be word aligned, so the last 4 bits are dropped, making
//      it effectively a 30 bit address. The remaining 2 bits are assumed to be equivalent to that
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
public struct Instruction
{
    // Universal
    private const int OPCODE_BIT_SIZE = 6;
    private const int REGISTER_ADDRESS_BIT_SIZE = 5;
    private const int OPCODE_BIT_OFFSET = ADDRESS_BIT_SIZE;

    // R Type
    private const int SHIFT_AMOUNT_BIT_SIZE = 5;
    private const int FUNCTION_BIT_SIZE = 6;

    private const int RS_BIT_OFFSET = REGISTER_ADDRESS_BIT_SIZE + RT_BIT_OFFSET;
    private const int RT_BIT_OFFSET = (REGISTER_ADDRESS_BIT_SIZE + RD_BIT_OFFSET);
    private const int RD_BIT_OFFSET = (SHIFT_AMOUNT_BIT_SIZE + SHIFT_AMOUNT_BIT_OFFSET);

    private const int SHIFT_AMOUNT_BIT_OFFSET = (FUNCTION_BIT_SIZE + FUNCTION_BIT_OFFSET);
    private const int FUNCTION_BIT_OFFSET = 0;

    // I Type
    private const int IMMEDIATE_BIT_SIZE = 16;
    private const int IMMEDIATE_BIT_OFFSET = 0;

    // J Type
    private const int ADDRESS_BIT_SIZE = 26;
    private const int ADDRESS_BIT_OFFSET = 0;

    private uint _inst;

    /// <summary>
    /// Creates a new r-type instruction.
    /// </summary>
    public static Instruction Create(FunctionCode funcCode, Register rs, Register rt, Register rd, byte shiftAmount = 0)
    {
        Instruction value = default;
        value.OpCode = OperationCode.RType;
        value.RS = rs;
        value.RT = rt;
        value.RD = rd;
        value.ShiftAmount = shiftAmount;
        value.FuncCode = funcCode;
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
    /// Creates a new branch instruction.
    /// </summary>
    public static Instruction Create(BranchCode code, Register rs, short immediate)
    {
        Instruction value = default;
        value.OpCode = OperationCode.BranchConditional;
        value.BranchCode = code;
        value.ImmediateValue = immediate;
        return value;
    }

    /// <summary>
    /// Gets a no operation instruction.
    /// </summary>
    public static Instruction NOP => (Instruction)0;

    /// <summary>
    /// Gets the instruction type.
    /// </summary>
    public InstructionType Type => InstructionTypeHelper.GetInstructionType(OpCode);

    /// <summary>
    /// Gets the instruction's operation code.
    /// </summary>
    public OperationCode OpCode
    {
        get => (OperationCode)GetShiftMask(OPCODE_BIT_SIZE, OPCODE_BIT_OFFSET);
        private set => SetShiftMask(OPCODE_BIT_SIZE, OPCODE_BIT_OFFSET, (uint)value);
    }

    /// <summary>
    /// Gets the instruction's RS Register 
    /// </summary>
    public Register RS
    {
        get => (Register)GetShiftMask(REGISTER_ADDRESS_BIT_SIZE, RS_BIT_OFFSET);
        private set => SetShiftMask(REGISTER_ADDRESS_BIT_SIZE, RS_BIT_OFFSET, (uint)value);
    }

    /// <summary>
    /// Gets the instruction's RT Register 
    /// </summary>
    public Register RT
    {
        get => (Register)GetShiftMask(REGISTER_ADDRESS_BIT_SIZE, RT_BIT_OFFSET);
        private set => SetShiftMask(REGISTER_ADDRESS_BIT_SIZE, RT_BIT_OFFSET, (uint)value);
    }

    /// <summary>
    /// Gets the instruction's branch code.
    /// </summary>
    /// <remarks>
    /// This is stored in the RT register space for an instruction where
    /// the <see cref="OpCode"/> is <see cref="OperationCode.BranchConditional"/>.
    /// </remarks>
    public BranchCode BranchCode
    {
        get => (BranchCode)RT;
        set => RT = (Register)value;
    }

    /// <summary>
    /// Gets the instruction's RD Register 
    /// </summary>
    public Register RD
    {
        get => (Register)GetShiftMask(REGISTER_ADDRESS_BIT_SIZE, RD_BIT_OFFSET);
        private set => SetShiftMask(REGISTER_ADDRESS_BIT_SIZE, RD_BIT_OFFSET, (uint)value);
    }

    /// <summary>
    /// Gets the instruction's shift amount 
    /// </summary>
    public byte ShiftAmount
    {
        get => (byte)GetShiftMask(SHIFT_AMOUNT_BIT_SIZE, SHIFT_AMOUNT_BIT_OFFSET);
        private set => SetShiftMask(SHIFT_AMOUNT_BIT_SIZE, SHIFT_AMOUNT_BIT_OFFSET, (uint)value);
    }

    /// <summary>
    /// Gets the instruction's function code.
    /// </summary>
    /// <remarks>
    /// Instruction may or may not have function code.
    /// </remarks>
    public FunctionCode FuncCode
    {
        get => (FunctionCode)GetShiftMask(FUNCTION_BIT_SIZE, FUNCTION_BIT_OFFSET);
        private set => SetShiftMask(FUNCTION_BIT_SIZE, FUNCTION_BIT_OFFSET, (uint)value);
    }

    /// <summary>
    /// Gets the instruction's immediate value.
    /// </summary>
    public short ImmediateValue
    {
        get => (short)GetShiftMask(IMMEDIATE_BIT_SIZE, IMMEDIATE_BIT_OFFSET);
        private set => SetShiftMask(IMMEDIATE_BIT_SIZE, IMMEDIATE_BIT_OFFSET, (ushort)value);
    }

    /// <summary>
    /// Gets the instruction's immediate value.
    /// </summary>
    public uint Address
    {
        get => GetShiftMask(ADDRESS_BIT_SIZE, ADDRESS_BIT_OFFSET);
        private set => SetShiftMask(ADDRESS_BIT_SIZE, ADDRESS_BIT_OFFSET, value);
    }

    /// <summary>
    /// Casts a <see cref="uint"/> to a <see cref="Instruction"/>.
    /// </summary>
    public static unsafe explicit operator Instruction(uint value) => *(Instruction*)&value;

    /// <summary>
    /// Casts a <see cref="uint"/> to a <see cref="Instruction"/>.
    /// </summary>
    public static unsafe explicit operator uint(Instruction value) => *(uint*)&value;

    private readonly uint GetShiftMask(int size, int offset)
    {
        // Generate the mask by taking 2^(size) and subtracting one
        uint mask = (uint)(1 << size) - 1;

        // Shift right by the offset then mask off the size
        return mask & (_inst >> offset);
    }

    private void SetShiftMask(int size, int offset, uint value)
    {
        // Generate the value mask by taking 2^(size),
        // subtracting one, and shifting.
        uint vMask = (uint)(1 << size) - 1;
        vMask <<= offset;

        // Then make the instruction mask and inverting
        // the value mask
        uint iMask = ~(vMask << offset);

        // Mask the instruction and the value
        uint vMasked = value & vMask;
        uint iMasked = _inst & iMask;

        // Combine the instruction and the value
        // post masking
        _inst = iMasked | vMasked;
    }
}
