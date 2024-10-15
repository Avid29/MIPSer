// Adam Dernis 2024

using MIPS.Helpers.Instructions;
using MIPS.Helpers;
using MIPS.Models.Instructions.Enums;
using MIPS.Models.Instructions.Enums.SpecialFunctions;
using MIPS.Models.Instructions.Enums.Registers;

namespace MIPS.Models.Instructions;

public partial struct Instruction
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

    /// <summary>
    /// Gets a no operation instruction.
    /// </summary>
    public static Instruction NOP => (Instruction)0;

    /// <summary>
    /// Gets the instruction type.
    /// </summary>
    public InstructionType Type => InstructionTypeHelper.GetInstructionType(OpCode, RTFuncCode);

    /// <summary>
    /// Gets the instruction's operation code.
    /// </summary>
    public OperationCode OpCode
    {
        get => (OperationCode)UintMasking.GetShiftMask(_inst, OPCODE_BIT_SIZE, OPCODE_BIT_OFFSET);
        private set => UintMasking.SetShiftMask(ref _inst, OPCODE_BIT_SIZE, OPCODE_BIT_OFFSET, (uint)value);
    }

    /// <summary>
    /// Gets the instruction's RS Register 
    /// </summary>
    public Register RS
    {
        get => (Register)UintMasking.GetShiftMask(_inst, REGISTER_ADDRESS_BIT_SIZE, RS_BIT_OFFSET);
        private set => UintMasking.SetShiftMask(ref _inst, REGISTER_ADDRESS_BIT_SIZE, RS_BIT_OFFSET, (uint)value);
    }

    /// <summary>
    /// Gets the instruction's RT Register 
    /// </summary>
    public Register RT
    {
        get => (Register)UintMasking.GetShiftMask(_inst, REGISTER_ADDRESS_BIT_SIZE, RT_BIT_OFFSET);
        private set => UintMasking.SetShiftMask(ref _inst, REGISTER_ADDRESS_BIT_SIZE, RT_BIT_OFFSET, (uint)value);
    }

    /// <summary>
    /// Gets the instruction's RT Func code.
    /// </summary>
    /// <remarks>
    /// This is stored in the RT register space for an instruction where
    /// the <see cref="OpCode"/> is <see cref="OperationCode.RegisterImmediate"/>.
    /// </remarks>
    public RTFuncCode RTFuncCode
    {
        get => (RTFuncCode)RT;
        set => RT = (Register)value;
    }

    /// <summary>
    /// Gets the instruction's RD Register 
    /// </summary>
    public Register RD
    {
        get => (Register)UintMasking.GetShiftMask(_inst, REGISTER_ADDRESS_BIT_SIZE, RD_BIT_OFFSET);
        private set => UintMasking.SetShiftMask(ref _inst, REGISTER_ADDRESS_BIT_SIZE, RD_BIT_OFFSET, (uint)value);
    }

    /// <summary>
    /// Gets the instruction's shift amount 
    /// </summary>
    public byte ShiftAmount
    {
        get => (byte)UintMasking.GetShiftMask(_inst, SHIFT_AMOUNT_BIT_SIZE, SHIFT_AMOUNT_BIT_OFFSET);
        private set => UintMasking.SetShiftMask(ref _inst, SHIFT_AMOUNT_BIT_SIZE, SHIFT_AMOUNT_BIT_OFFSET, (uint)value);
    }

    /// <summary>
    /// Gets the instruction's function code.
    /// </summary>
    /// <remarks>
    /// Instruction may or may not have function code.
    /// </remarks>
    public FunctionCode FuncCode
    {
        get => (FunctionCode)UintMasking.GetShiftMask(_inst, FUNCTION_BIT_SIZE, FUNCTION_BIT_OFFSET);
        private set => UintMasking.SetShiftMask(ref _inst, FUNCTION_BIT_SIZE, FUNCTION_BIT_OFFSET, (uint)value);
    }

    /// <summary>
    /// Gets the instruction's function2 code.
    /// </summary>
    /// <remarks>
    /// Instruction may or may not have function code.
    /// </remarks>
    public Func2Code Func2Code
    {
        get => (Func2Code)FuncCode;
        private set => FuncCode = (FunctionCode)value;
    }

    /// <summary>
    /// Gets the instruction's immediate value.
    /// </summary>
    public short ImmediateValue
    {
        get => (short)UintMasking.GetShiftMask(_inst, IMMEDIATE_BIT_SIZE, IMMEDIATE_BIT_OFFSET);
        private set => UintMasking.SetShiftMask(ref _inst, IMMEDIATE_BIT_SIZE, IMMEDIATE_BIT_OFFSET, (ushort)value);
    }

    /// <summary>
    /// Gets the instructions offset value.
    /// </summary>
    public int Offset
    {
        get => ImmediateValue << 2;
        private set => ImmediateValue = (short)(value >> 2);
    }

    /// <summary>
    /// Gets the instruction's immediate value.
    /// </summary>
    public uint Address
    {
        get => UintMasking.GetShiftMask(_inst, ADDRESS_BIT_SIZE, ADDRESS_BIT_OFFSET) << 2;
        private set => UintMasking.SetShiftMask(ref _inst, ADDRESS_BIT_SIZE, ADDRESS_BIT_OFFSET, value >> 2);
    }
}
