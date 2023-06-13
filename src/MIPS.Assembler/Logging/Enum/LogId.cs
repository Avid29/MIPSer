// Adam Dernis 2023

namespace MIPS.Assembler.Logging.Enum;

/// <summary>
/// An id for types of logs.
/// </summary>
public enum LogId
{
    #pragma warning disable CS1591

    // Instruction parser
    InvalidInstructionName,
    InvalidInstructionArgCount,
    InstructionIncorrectParsingMethod,
    InvalidRegisterArgument,

    // Expression parser
    InvalidImmediate

    #pragma warning restore CS1591
}
