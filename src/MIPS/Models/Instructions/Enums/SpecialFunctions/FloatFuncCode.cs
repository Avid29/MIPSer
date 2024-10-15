// Adam Dernis 2024

namespace MIPS.Models.Instructions.Enums.SpecialFunctions;

/// <summary>
/// An enum for <see cref="OperationCode.Coprocessor1"/> instruction function codes.
/// </summary>
public enum FloatFuncCode
{
#pragma warning disable CS1591

    Add = 0x0,
    Subtract = 0x1,
    Multiply = 0x2,
    Divide = 0x3,
    SquareRoot = 0x4,
    AbsoluteValue = 0x5,
    Move = 0x6,
    Negate = 0x7,
    Round = 0xc,
    Ceiling = 0xe,
    Floor = 0xf,
    ConvertToSingle = 0x20,
    ConvertToDouble = 0x21,
    
#pragma warning restore CS1591
}
