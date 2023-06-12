// Adam Dernis 2023

namespace MIPS.Assembler.Parsers.Enums;

/// <summary>
/// An enum for tracking the state of the expression parser.
/// </summary>
public enum ExpressionParserState
{
    #pragma warning disable CS1591

    Start,
    Integer,
    Operator,

    /// <remarks>
    /// Marco or symbol.
    /// </remarks>
    Macro,
    String,
    
    #pragma warning restore CS1591
}
