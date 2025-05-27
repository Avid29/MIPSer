// Adam Dernis 2025

using MIPS.Models.Instructions.Enums.Operations;
using MIPS.Models.Instructions.Enums.Registers;
using MIPS.Models.Instructions.Enums.SpecialFunctions.CoProc0;
using System.Runtime.CompilerServices;

namespace MIPS.Models.Instructions;

/// <summary>
/// A struct representing an instruction utilizing coprocessor0.
/// </summary>
public partial struct CoProc0Instruction
{
    private Instruction _inst;

    /// <summary>
    /// Creates a new <see cref="OperationCode.Coprocessor0"/> instruction.
    /// </summary>
    public static CoProc0Instruction Create(CoProc0RSCode code, Register rt, Register rd)
    {
        CoProc0Instruction value = default;
        value.OpCode = OperationCode.Coprocessor0;
        value.CoProc0RSCode = code;
        value.RT = rt;
        value.RD = rd;

        return value;
    }
    
    /// <summary>
    /// Creates a new <see cref="CoProc0RSCode.C0"/> instruction.
    /// </summary>
    public static CoProc0Instruction Create(Co0FuncCode code)
    {
        CoProc0Instruction value = default;
        value.OpCode = OperationCode.Coprocessor0;
        value.Co0FuncCode = code;
        value.CoProc0RSCode = CoProc0RSCode.C0;

        return value;
    }
    
    /// <summary>
    /// Creates a new <see cref="CoProc0RSCode.MFMC0"/> instruction.
    /// </summary>
    public static CoProc0Instruction Create(MFMC0FuncCode code, Register rt = Register.Zero, byte? rd = null)
    {
        CoProc0Instruction value = default;
        value.OpCode = OperationCode.Coprocessor0;
        value.MFMC0FuncCode = code;
        value.CoProc0RSCode = CoProc0RSCode.MFMC0;
        value.RT = rt;

        // Conditionally assign
        value.RD = rd.HasValue ? (Register)rd.Value : value.RD;

        return value;
    }

    /// <summary>
    /// Casts a <see cref="uint"/> to a <see cref="CoProc0Instruction"/>.
    /// </summary>
    public static unsafe explicit operator CoProc0Instruction(uint value) => Unsafe.As<uint, CoProc0Instruction>(ref value);

    /// <summary>
    /// Casts a <see cref="CoProc0Instruction"/> to a <see cref="uint"/>.
    /// </summary>
    public static unsafe explicit operator uint(CoProc0Instruction value) => Unsafe.As<CoProc0Instruction, uint>(ref value);

    /// <summary>
    /// Casts an <see cref="Instruction"/> to a <see cref="CoProc0Instruction"/>.
    /// </summary>
    public static unsafe implicit operator CoProc0Instruction(Instruction value) => Unsafe.As<Instruction, CoProc0Instruction>(ref value);

    /// <summary>
    /// Casts a <see cref="CoProc0Instruction"/> to a <see cref="Instruction"/>.
    /// </summary>
    public static unsafe implicit operator Instruction(CoProc0Instruction value) => Unsafe.As<CoProc0Instruction, Instruction>(ref value);
}
