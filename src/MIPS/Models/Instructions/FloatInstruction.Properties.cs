// Adam Dernis 2025

using MIPS.Helpers;
using MIPS.Models.Instructions.Enums;
using MIPS.Models.Instructions.Enums.Operations;
using MIPS.Models.Instructions.Enums.Registers;
using MIPS.Models.Instructions.Enums.SpecialFunctions.FloatProc;

namespace MIPS.Models.Instructions;

public partial struct FloatInstruction
{
    private const int OPCODE_BIT_SIZE = 6;
    private const int FORMAT_BIT_SIZE = 5;
    private const int REGISTER_BIT_SIZE = 5;
    private const int FUNCTION_BIT_SIZE = 6;

    private const int FUNCTION_BIT_OFFSET = 0;
    private const int FD_BIT_OFFSET = FUNCTION_BIT_SIZE;
    private const int FS_BIT_OFFSET = FD_BIT_OFFSET + REGISTER_BIT_SIZE;
    private const int FT_BIT_OFFSET = FS_BIT_OFFSET + REGISTER_BIT_SIZE;
    private const int FORMAT_BIT_OFFSET = FT_BIT_OFFSET + REGISTER_BIT_SIZE;
    private const int OPCODE_BIT_OFFSET = FORMAT_BIT_OFFSET + FORMAT_BIT_SIZE;
    
    /// <summary>
    /// Gets the instruction's operation code.
    /// </summary>
    public OperationCode OpCode
    { 
        readonly get => (OperationCode)UintMasking.GetShiftMask(_inst, OPCODE_BIT_SIZE, OPCODE_BIT_OFFSET);
        private set => UintMasking.SetShiftMask(ref _inst, OPCODE_BIT_SIZE, OPCODE_BIT_OFFSET, (uint)value);
    }

    /// <summary>
    /// Gets the instruction's float function code.
    /// </summary>
    public FloatFuncCode FloatFuncCode
    {
        readonly get => (FloatFuncCode)UintMasking.GetShiftMask(_inst, FUNCTION_BIT_SIZE, FUNCTION_BIT_OFFSET);
        private set => UintMasking.SetShiftMask(ref _inst, FUNCTION_BIT_SIZE, FUNCTION_BIT_OFFSET, (uint)value);
    }

    /// <summary>
    /// Gets the instruction's format.
    /// </summary>
    public FloatFormat Format
    {
        readonly get => (FloatFormat)UintMasking.GetShiftMask(_inst, FORMAT_BIT_SIZE, FORMAT_BIT_OFFSET);
        private set => UintMasking.SetShiftMask(ref _inst, FORMAT_BIT_SIZE, FORMAT_BIT_OFFSET, (uint)value);
    }

    /// <summary>
    /// Gets the instruction's FT Register.
    /// </summary>
    public FloatRegister FT
    {
        readonly get => (FloatRegister)UintMasking.GetShiftMask(_inst, REGISTER_BIT_SIZE, FT_BIT_OFFSET);
        private set => UintMasking.SetShiftMask(ref _inst, REGISTER_BIT_SIZE, FT_BIT_OFFSET, (uint)value);
    }

    /// <summary>
    /// Gets the instruction's FS Register.
    /// </summary>
    public FloatRegister FS
    {
        readonly get => (FloatRegister)UintMasking.GetShiftMask(_inst, REGISTER_BIT_SIZE, FS_BIT_OFFSET);
        private set => UintMasking.SetShiftMask(ref _inst, REGISTER_BIT_SIZE, FS_BIT_OFFSET, (uint)value);
    }

    /// <summary>
    /// Gets the instruction's FS Register.
    /// </summary>
    public FloatRegister FD
    {
        readonly get => (FloatRegister)UintMasking.GetShiftMask(_inst, REGISTER_BIT_SIZE, FD_BIT_OFFSET);
        private set => UintMasking.SetShiftMask(ref _inst, REGISTER_BIT_SIZE, FD_BIT_OFFSET, (uint)value);
    }
}
