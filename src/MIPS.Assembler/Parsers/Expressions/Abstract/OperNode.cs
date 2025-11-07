// Avishai Dernis 2025

using CommunityToolkit.Diagnostics;
using MIPS.Assembler.Parsers.Expressions.Enums;
using MIPS.Assembler.Parsers.Expressions.Evaluator;
using MIPS.Assembler.Tokenization.Models;
using MIPS.Models.Addressing;

namespace MIPS.Assembler.Parsers.Expressions.Abstract;

/// <summary>
/// A class an operator in an expression tree.
/// </summary>
public abstract class OperNode : ExpNode
{
    /// <summary>
    /// Initializes a new instance of the <see cref="OperNode"/> class.
    /// </summary>
    public OperNode(Token token, Operation operation) : base(token)
    {
        Operation = operation;
    }

    /// <summary>
    /// Gets or sets the operation of the <see cref="BinaryOperNode"/>.
    /// </summary>
    public Operation Operation { get; }

    /// <summary>
    /// Gets the priority of the operation by order of operations.
    /// </summary>
    /// <remarks>
    /// Lower is lower on the tree and therefore executed earlier.
    /// </remarks>
    public int Priority => Operation switch
    {
        Operation.Addition => 4,
        Operation.Subtraction => 4,
        Operation.Multiplication => 3,
        Operation.Division => 3,
        Operation.Modulus => 3,
        Operation.And => 2,
        Operation.Or => 1,
        Operation.Xor => 1,

        Operation.Negation => 1,
        Operation.UnaryPlus => 1,
        Operation.Not => 1,
        _ => ThrowHelper.ThrowArgumentException<int>("Cannot assess priority of invalid operation.")
    };

    /// <summary>
    /// Attempts to add a child node to this operator.
    /// </summary>
    /// <returns>Whether or not a child could be added to the opertator node.</returns>
    public abstract bool TryAddChild(ExpNode node);

    /// <summary>
    /// Attempts to insert a <see cref="BinaryOperNode"/> above this node in the tree.
    /// </summary>
    /// <param name="node">The node to insert.</param>
    /// <returns></returns>
    public abstract bool TryInsertNode(BinaryOperNode node);

    /// <summary>
    /// Gets or sets a child node, updating its parent accordingly.
    /// </summary>
    protected void SetChild(ref ExpNode? child, ExpNode? value)
    {
        // Clear current child's parent
        if (child is not null)
            child.Parent = null;

        // Assign new child's parent
        if (value is not null)
            value.Parent = this;

        // Assign new child
        child = value;
    }
}
