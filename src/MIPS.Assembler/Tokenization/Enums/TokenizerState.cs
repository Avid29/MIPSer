// Adam Dernis 2024

namespace MIPS.Assembler.Tokenization.Enums;

/// <summary>
/// An enum to track the evaulation state of the tokenizer.
/// </summary>
public enum TokenizerState
{
#pragma warning disable CS1591

    LineBegin,
    ArgBegin,
    NewLineText,
    NewLineTextWait,    // Waiting for a ':', '=', or other to mark a label, macro, or instruction
    Register,
    Immediate,
    SpecialImmediate,   // This is after the first 0 because it could be followed by an x, o, or b etc making it hex, oct, or binary etc
    BinaryImmediate,    // After 0b
    OctImmediate,       // After 0o
    HexImmediate,       // After 0x
    BadImmediate,       // After an invalid character for the base
    Character,
    String,
    Directive,
    Reference,
    Comment,
    Complete,           // The last token is complete, but creation should be deferred in order to get the column right
    NewLineWhitespace,  // Whitespace at the start of a new line
    Whitespace,

#pragma warning restore CS1591
}
