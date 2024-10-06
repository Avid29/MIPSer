// Adam Dernis 2023

namespace MIPS.Assembler.Tokenization.Enums;

/// <summary>
/// An enum to track the evaulation state of the tokenizer.
/// </summary>
public enum TokenizerState
{
#pragma warning disable CS1591
    LineBegin,
    Begin,
    NewLineText,
    NewLineTextWait,    // Waiting for a ':', '=', or other to mark a label, macro, or instruction
    Register,
    Immediate,
    SpecialImmediate,   // This is after the first 0 because it could be followed by an x, o, or b etc making it hex, oct, or binary etc
    Character,
    String,
    Directive,
    Reference,
    Comment,

#pragma warning restore CS1591
}
