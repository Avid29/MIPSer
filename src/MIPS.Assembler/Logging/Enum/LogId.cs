// Adam Dernis 2023

namespace MIPS.Assembler.Logging.Enum;

/// <summary>
/// An id for types of logs.
/// </summary>
public enum LogId
{
#pragma warning disable CS1591

    // General errors
    IllegalSymbolName,
    DuplicateSymbolDefinition,
    TokenizerError,

    // Macro errors
    MacroMissingValue,
    MacroCannotBeRelocatable,

    // Instruction parser errors
    InvalidInstructionName,
    InvalidInstructionArgCount,
    InvalidRegisterArgument,
    InvalidAddressOffsetArgument,

    // Instruction parser warnings
    IntegerTruncated,
    RelocatableReferenceInShift,

    // Instruction parser messages
    ZeroRegWriteBack,

    // Expression parser errors
    UnparsableExpression,
    InvalidExpressionOperation,

    // Directive parser errors
    InvalidDirectiveDataArg,
    InvalidDirectiveArg,
    InvalidDirectiveArgCount,

    // String parsing errors
    NotAString,
    MultiLineString,
    UnrecognizedEscapeSequence,
    UnescapedQuoteInString,

#pragma warning restore CS1591
}
