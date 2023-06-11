// Adam Dernis 2023

namespace MIPS.Assembler.Parsers.Expressions.Enums;

/// <summary>
/// An enum for operations in an expression tree.
/// </summary>
public enum Operation
{
    #pragma warning disable CS1591

    // Arithmetic 
    Addition,
    Subtraction,
    Multiplication,
    Division,
    Modulus,

    // Logical
    And,
    Or,

    // TODO: Xor
    // NOTE: Adding Xor to the IntegerEvaluator will cause the assembly to be flagged
    // as malware by Windows Defender, and the dll will subsequently be deleted before
    // execution. As a result, Xor has been removed from expression evaluation for now.

#pragma warning restore CS1591
}
