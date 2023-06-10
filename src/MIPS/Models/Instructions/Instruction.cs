// Adam Dernis 2023

using MIPS.Models.Instructions.Enums;

namespace MIPS.Models.Instructions;

/// <summary>
/// A struct representing an instruction.
/// </summary>
public struct Instruction
{
    // Universal
    private const int OPCODE_SIZE = 6;
    private const int REGISTER_ADDRESS_SIZE = 5;
    private const int OPCODE_OFFSET = ADDRESS_SIZE;

    // R Type
    private const int SHIFT_AMOUNT_SIZE = 5;
    private const int FUNCTION_SIZE = 6;

    private const int RS_OFFSET = REGISTER_ADDRESS_SIZE + RT_OFFSET;
    private const int RT_OFFSET = (REGISTER_ADDRESS_SIZE + RD_OFFSET);
    private const int RD_OFFSET = (SHIFT_AMOUNT_SIZE + SHIFT_AMOUNT_OFFSET);

    private const int SHIFT_AMOUNT_OFFSET = (FUNCTION_SIZE + FUNCTION_OFFSET);
    private const int FUNCTION_OFFSET = 0;

    // I Type
    private const int IMMEDIATE_SIZE = 16;
    private const int IMMEDIATE_OFFSET = 0;

    // J Type
    private const int ADDRESS_SIZE = 26;
    private const int ADDRESS_OFFSET = 0;

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
    public static Instruction Create(OperationCode opCode, Register rs, Register rt, ushort immediate)
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
    public static Instruction Create(OperationCode opCode, ushort address)
    {
        Instruction value = default;
        value.OpCode = opCode;
        value.Address = address;
        return value;
    }

    /// <summary>
    /// Gets the instruction type.
    /// </summary>
    public InstructionType Type
    {
        get
        {
            return OpCode switch
            {
                OperationCode.RType => InstructionType.R,

                OperationCode.Jump or 
                OperationCode.JumpAndLink => InstructionType.J,

                _ => InstructionType.I,
            };
        }
    }

    /// <summary>
    /// Gets the instruction's operation code.
    /// </summary>
    public OperationCode OpCode
    {
        get => (OperationCode)GetShiftMask(OPCODE_SIZE, OPCODE_OFFSET);
        private set => SetShiftMask(OPCODE_SIZE, OPCODE_OFFSET, (uint)value);
    }

    /// <summary>
    /// Gets the instruction's RS Register 
    /// </summary>
    public Register RS
    {
        get => (Register)GetShiftMask(REGISTER_ADDRESS_SIZE, RS_OFFSET);
        private set => SetShiftMask(REGISTER_ADDRESS_SIZE, RS_OFFSET, (uint)value);
    }

    /// <summary>
    /// Gets the instruction's RS Register 
    /// </summary>
    public Register RT
    {
        get => (Register)GetShiftMask(REGISTER_ADDRESS_SIZE, RT_OFFSET);
        private set => SetShiftMask(REGISTER_ADDRESS_SIZE, RT_OFFSET, (uint)value);
    }

    /// <summary>
    /// Gets the instruction's RS Register 
    /// </summary>
    public Register RD
    {
        get => (Register)GetShiftMask(REGISTER_ADDRESS_SIZE, RD_OFFSET);
        private set => SetShiftMask(REGISTER_ADDRESS_SIZE, RD_OFFSET, (uint)value);
    }

    /// <summary>
    /// Gets the instruction's RS Register 
    /// </summary>
    public byte ShiftAmount
    {
        get => (byte)GetShiftMask(SHIFT_AMOUNT_SIZE, SHIFT_AMOUNT_OFFSET);
        private set => SetShiftMask(SHIFT_AMOUNT_SIZE, SHIFT_AMOUNT_OFFSET, (uint)value);
    }
    
    /// <summary>
    /// Gets the instruction's function code.
    /// </summary>
    /// <remarks>
    /// Instruction may or may not have function code.
    /// </remarks>
    public FunctionCode FuncCode
    {
        get => (FunctionCode)GetShiftMask(FUNCTION_SIZE, FUNCTION_OFFSET);
        private set => SetShiftMask(FUNCTION_SIZE, FUNCTION_OFFSET, (uint)value);
    }

    /// <summary>
    /// Gets the instruction's immediate value.
    /// </summary>
    public ushort ImmediateValue
    {
        get => (ushort)GetShiftMask(IMMEDIATE_SIZE, IMMEDIATE_OFFSET);
        private set => SetShiftMask(IMMEDIATE_SIZE, IMMEDIATE_OFFSET, value);
    }

    /// <summary>
    /// Gets the instruction's immediate value.
    /// </summary>
    public uint Address
    {
        get => GetShiftMask(ADDRESS_SIZE, ADDRESS_OFFSET);
        private set => SetShiftMask(ADDRESS_SIZE, ADDRESS_OFFSET, value);
    }

    private uint GetShiftMask(int size, int offset)
    {
        // Generate the mask by taking 2^(size) and subtracting one
        uint mask = (uint)(1 << size) - 1;

        // Shift right by the offset then mask off the size
        return mask & (_inst >> offset);
    }

    private void SetShiftMask(int size, int offset, uint value)
    {
        // Generate the mask by taking 2^(size) and subtracting one
        // Then shifting and inverting
        uint mask = (uint)(1 << size) - 1;
        mask = ~(mask << offset);

        // Clear masked section
        uint masked = _inst & mask;

        // Shift value and assign to masked section
        _inst = masked | (value << offset);
    }
}
