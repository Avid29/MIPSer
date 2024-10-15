// Adam Dernis 2024

using MIPS.Models.Instructions.Enums;
using MIPS.Models.Instructions.Enums.Registers;
using MIPS.Models.Instructions.Enums.SpecialFunctions;

namespace MIPS.Models.Instructions;

public partial struct Instruction
{
    /// <summary>
    /// Gets the instruction's float function code.
    /// </summary>
    public FloatFuncCode FloatFuncCode
    {
        // The float func code register is aligned with the function code register
        get => (FloatFuncCode)FuncCode;
        set => FuncCode = (FunctionCode)value;
    }

    /// <summary>
    /// Gets the instruction's format.
    /// </summary>
    public FloatFormat Format
    {
        // The format register is aligned with the RS register
        get => (FloatFormat)RS;
        set => RS = (Register)value;
    }

    /// <summary>
    /// Gets the instruction's FT Register.
    /// </summary>
    public FloatRegister FT
    {
        get => (FloatRegister)RT;
        private set => RT = (Register)value;
    }

    /// <summary>
    /// Gets the instruction's FS Register.
    /// </summary>
    public FloatRegister FS
    {
        // Yes, the FS register is aligned with the RD register
        get => (FloatRegister)RD;
        private set => RD = (Register)value;
    }

    /// <summary>
    /// Gets the instruction's FS Register.
    /// </summary>
    public FloatRegister FD
    {
        // The FD register is aligned with the shift amoint
        get => (FloatRegister)ShiftAmount;
        private set => ShiftAmount = (byte)value;
    }
}
