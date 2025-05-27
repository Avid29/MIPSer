// Adam Dernis 2024

using MIPS.Models.Instructions.Enums;
using MIPS.Models.Instructions.Enums.Operations;
using MIPS.Models.Instructions.Enums.Registers;
using MIPS.Models.Instructions.Enums.SpecialFunctions.FloatProc;
using System.Runtime.CompilerServices;

namespace MIPS.Models.Instructions;

/// <summary>
/// A struct representing an instruction utilizing the floating-point coprocessor.
/// </summary>
public partial struct FloatInstruction
{
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
