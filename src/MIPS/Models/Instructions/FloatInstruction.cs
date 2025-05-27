// Adam Dernis 2024

using MIPS.Models.Instructions.Enums;
using MIPS.Models.Instructions.Enums.Operations;
using MIPS.Models.Instructions.Enums.Registers;
using MIPS.Models.Instructions.Enums.SpecialFunctions;
using MIPS.Models.Instructions.Enums.SpecialFunctions.FloatProc;
using System.Runtime.CompilerServices;

namespace MIPS.Models.Instructions;

/// <summary>
/// A struct representing an instruction utilizing the floating-point coprocessor.
/// </summary>
public struct FloatInstruction
{
    private Instruction _inst;
    
    /// <summary>
    /// Creates a new floating-point coprocessor instruction.
    /// </summary>
    public static FloatInstruction Create(FloatFuncCode funcCode, FloatFormat format, FloatRegister fs, FloatRegister fd, FloatRegister ft = FloatRegister.F0)
    {
        FloatInstruction value = default;
        value.OpCode = OperationCode.Coprocessor1;
        value.FloatFuncCode = funcCode;
        value.Format = format;
        value.FS = fs;
        value.FD = fd;
        value.FT = ft;
        return value;
    }
    /// <summary>
    /// Gets the instruction's operation code.
    /// </summary>
    public OperationCode OpCode
    { 
        readonly get => _inst.OpCode;
        private set => _inst.OpCode = value;
    }

    /// <summary>
    /// Gets the instruction's float function code.
    /// </summary>
    public FloatFuncCode FloatFuncCode
    {
        readonly get => (FloatFuncCode)_inst.FuncCode;
        private set => _inst.FuncCode = (FunctionCode)value;
    }

    /// <summary>
    /// Gets the instruction's format.
    /// </summary>
    public FloatFormat Format
    {
        readonly get => (FloatFormat)_inst.RS;
        private set => _inst.RS = (Register)value;
    }

    /// <summary>
    /// Gets the instruction's FT Register.
    /// </summary>
    public FloatRegister FT
    {
        readonly get => (FloatRegister)_inst.RT;
        private set => _inst.RT = (Register)value;
    }

    /// <summary>
    /// Gets the instruction's FS Register.
    /// </summary>
    public FloatRegister FS
    {
        readonly get => (FloatRegister)_inst.RT;
        private set => _inst.RT = (Register)value;
    }

    /// <summary>
    /// Gets the instruction's FD Register.
    /// </summary>
    public FloatRegister FD
    {
        readonly get => (FloatRegister)_inst.ShiftAmount;
        private set => _inst.ShiftAmount = (byte)value;
    }
    
    /// <summary>
    /// Casts a <see cref="uint"/> to a <see cref="FloatInstruction"/>.
    /// </summary>
    public static unsafe explicit operator FloatInstruction(uint value) => Unsafe.As<uint, FloatInstruction>(ref value);

    /// <summary>
    /// Casts a <see cref="FloatInstruction"/> to a <see cref="uint"/>.
    /// </summary>
    public static unsafe explicit operator uint(FloatInstruction value) => Unsafe.As<FloatInstruction, uint>(ref value);

    /// <summary>
    /// Casts an <see cref="Instruction"/> to a <see cref="FloatInstruction"/>.
    /// </summary>
    public static unsafe implicit operator FloatInstruction(Instruction value) => Unsafe.As<Instruction, FloatInstruction>(ref value);

    /// <summary>
    /// Casts a <see cref="FloatInstruction"/> to a <see cref="Instruction"/>.
    /// </summary>
    public static unsafe implicit operator Instruction(FloatInstruction value) => Unsafe.As<FloatInstruction, Instruction>(ref value);
}
