// Adam Dernis 2023

using MIPS.Models.Instructions.Enums;

namespace MIPS.Helpers.Instructions;

/// <summary>
/// A static class containing helper methods related to instruction types.
/// </summary>
public static class InstructionTypeHelper
{
    /// <summary>
    /// Gets the <see cref="InstructionType"/> of an <see cref="OperationCode"/>.
    /// </summary>
    /// <param name="opCode">The instruction op-code</param>
    /// <returns>The <see cref="InstructionType"/> associated to an <see cref="OperationCode"/>.</returns>
    public static InstructionType GetInstructionType(OperationCode opCode)
    {
        return opCode switch
        {
            OperationCode.RType or
                OperationCode.MoveFromCoprocessor => InstructionType.R,

            OperationCode.Jump or 
                OperationCode.JumpAndLink => InstructionType.J,

            _ => InstructionType.I,
        };
    }
}
