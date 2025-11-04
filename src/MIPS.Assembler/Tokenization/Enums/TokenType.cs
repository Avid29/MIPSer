// Adam Dernis 2024

namespace MIPS.Assembler.Tokenization.Enums;

/// <summary>
/// An enum disignating a parse token's type.
/// </summary>
public enum TokenType
{
#pragma warning disable CS1591
    Instruction,
    Register,
    Immediate,
    Directive,
    Operator,
    String,

    LabelDeclaration,
    // TODO: Consider making the label splitter (':') a separate token type
    Reference,          // This could be either a label or a macro

    OpenParenthesis,
    CloseParenthesis,
    OpenBracket,
    CloseBracket,
    Comma,

    MacroDefinition,
    Assign,

    Comment,
    Whitespace,     // Used to maintain spacing in behavior mode

#pragma warning restore CS1591
}
