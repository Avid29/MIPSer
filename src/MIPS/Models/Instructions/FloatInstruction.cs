// Adam Dernis 2024

using MIPS.Models.Instructions.Enums.Registers;
using MIPS.Models.Instructions.Enums.SpecialFunctions;
using MIPS.Models.Instructions.Enums;
using MIPS.Helpers;
using System.Runtime.CompilerServices;

namespace MIPS.Models.Instructions;

/// <summary>
/// A struct representing an instruction utilizing the floating-point coprocessor..
/// </summary>
public struct FloatInstruction
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

    private uint _inst;
    
    /// <summary>
    /// Creates a new floating-point coprocessor instruction.
    /// </summary>
    public static FloatInstruction Create(FloatFuncCode funcCode, FloatFormat format, FloatRegister ft, FloatRegister fs, FloatRegister fd)
    {
        FloatInstruction value = default;
        value.OpCode = OperationCode.Coprocessor1;
        value.FloatFuncCode = funcCode;
        value.Format = format;
        value.FT = ft;
        value.FS = fs;
        value.FD = fd;
        return value;
    }

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

    /// <summary>
    /// Casts a <see cref="FloatInstruction"/> to a <see cref="Instruction"/>.
    /// </summary>
    public static unsafe implicit operator Instruction(FloatInstruction value) => Unsafe.As<FloatInstruction, Instruction>(ref value);
}
