// Adam Dernis 2023

using MIPS.Models.Instructions;
using MIPS.Models.Instructions.Enums;

namespace MIPS.Extensions.MIPS.Models.Instructions;

/// <summary>
/// A static class containing instruction extensions.
/// </summary>
public static class InstructionExtensions
{
    /// <summary>
    /// Checks if an instructions writes back to the $zero register.
    /// </summary>
    /// <param name="instruction">The instruction to check.</param>
    /// <returns>Whether or not the instruction writes back to the $zero register.</returns>
    public static bool WritesBackToZero(this Instruction instruction)
    {
        return instruction.Type switch
        {
            // Check for R type instructions writing back to $zero
            InstructionType.R when instruction.RD is Register.Zero => instruction.FuncCode switch
            {
                // All these functions write back to $rd.
                FunctionCode.ShiftLeftLogical or FunctionCode.ShiftRightLogical or FunctionCode.ShiftRightArithmetic or                         // Shifts
                FunctionCode.ShiftLeftLogicalVariable or FunctionCode.ShiftRightLogicalVariable or FunctionCode.ShiftRightArithmeticVariable or // Variable Shifts
                FunctionCode.MoveFromHigh or FunctionCode.MoveFromLow or                                                                        // Move From
                FunctionCode.Add or FunctionCode.AddUnsigned or FunctionCode.Subtract or FunctionCode.SubtractUnsigned or                       // Arithmetic
                FunctionCode.And or FunctionCode.Or or FunctionCode.ExclusiveOr or FunctionCode.Nor or                                          // Logical
                FunctionCode.SetLessThan or FunctionCode.SetLessThanUnsigned => true,                                                           // Sets
                _ => false,
            },

            // Check for I type instructions writing back to $zero
            InstructionType.I when instruction.RT is Register.Zero => instruction.OpCode switch
            {
                OperationCode.AddImmediate or OperationCode.AddImmediateUnsigned or                                                             // Arithmetic
                OperationCode.SetLessThanImmediate or OperationCode.SetLessThanImmediateUnsigned or                                             // Sets
                OperationCode.AndImmediate or OperationCode.OrImmediate or OperationCode.ExclusiveOrImmediate or                                // Logical
                OperationCode.LoadByte or OperationCode.LoadHalfWord or OperationCode.LoadWordLeft or OperationCode.LoadWord or                 // Loads
                OperationCode.LoadByteUnsigned or OperationCode.LoadHalfWordUnsigned or OperationCode.LoadWordRight => true,                    // Loads (continued)
                    _ => false
            },
            _ => false
        };
    }
}
