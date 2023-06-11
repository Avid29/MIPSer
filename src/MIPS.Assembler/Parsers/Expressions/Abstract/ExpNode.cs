// Adam Dernis 2023

using MIPS.Assembler.Parsers.Expressions.Enums;

namespace MIPS.Assembler.Parsers.Expressions.Abstract;

/// <summary>
/// A class for nodes in an expression tree.
/// </summary>
public abstract class ExpNode
{
    /// <summary>
    /// Gets the node's parent in the expression tree.
    /// </summary>
    public OperNode? Parent { get; set; }

    /// <summary>
    /// Gets the type of the expression node.
    /// </summary>
    public abstract ExpressionType Type { get; }
}
