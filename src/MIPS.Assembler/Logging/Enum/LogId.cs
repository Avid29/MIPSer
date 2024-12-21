// Adam Dernis 2024

namespace MIPS.Assembler.Logging.Enum;

/// <summary>
/// An id for types of logs.
/// </summary>
public enum LogId
{
#pragma warning disable CS1591

    // General
    IllegalSymbolName,
    DuplicateSymbolDefinition,
    TokenizerError,
    DisabledFeatureInUse,
    NotInVersion,

    // Macros
    MacroMissingValue,
    MacroCannotBeRelocatable,

    // Instruction parser 
    InvalidInstructionName,
    InvalidInstructionArgCount,
    InvalidRegisterArgument,
    InvalidAddressOffsetArgument,
    BranchBetweenSections,
    IntegerTruncated,
    RelocatableReferenceInShift,
    ZeroRegWriteBack,

    // Expression parser
    UnparsableExpression,
    InvalidExpressionOperation,
    InvalidOperationOnRelocatable,

    // Directive parser
    InvalidDirectiveDataArg,
    InvalidDirectiveArg,
    InvalidDirectiveArgCount,
    LargeAlignment,
    LargeSpacing,

    // Char/String parsing
    NotAString,
    MultiLineString,
    InvalidCharLiteral,
    UnrecognizedEscapeSequence,
    UnescapedQuoteInString,

    // Linker Errors
    FailedToLoadModule,

#pragma warning restore CS1591
}
