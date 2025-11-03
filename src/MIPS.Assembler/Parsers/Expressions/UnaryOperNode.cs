// Avishai Dernis 2025

using CommunityToolkit.Diagnostics;
using MIPS.Assembler.Parsers.Expressions.Abstract;
using MIPS.Assembler.Parsers.Expressions.Enums;
using MIPS.Assembler.Parsers.Expressions.Evaluator;
using MIPS.Assembler.Tokenization;
using MIPS.Models.Addressing;

namespace MIPS.Assembler.Parsers.Expressions;

/// <summary>
/// A class for a unary operator in an expression tree.
/// </summary>
public class UnaryOperNode : OperNode
{
    private ExpNode? _child;
    /// <summary>
    /// Initializes a new instance of the <see cref="UnaryOperNode"/> class.
    /// </summary>
    public UnaryOperNode(Token token, Operation operation) : base(token, operation)
    {
    }

    /// <summary>
    /// Gets or sets the child of the <see cref="UnaryOperNode"/>.
    /// </summary>
    public ExpNode? Child
    {
        get => _child;
        set => SetChild(ref _child, value);
    }

    /// <inheritdoc/>
    /// <remarks>
    /// Determined by its child.
    /// </remarks>
    public override ExpressionType Type => Child?.Type ?? ExpressionType.Invalid;
    
    /// <inheritdoc/>
    public override bool TryAddChild(ExpNode node)
    {
        if (Child is not null)
            return false;

        Child = node;
        return true;
    }
    
    /// <inheritdoc/>
    public override bool TryInsertNode(BinaryOperNode node)
    {
        // Insert the binary operator between the unary operator and its child
        node.LeftChild = Child;
        Child = node;
        return true;
    }

    /// <inheritdoc/>
    public override bool TryEvaluate(IEvaluator<Address> evaluator, out Address result)
    {
        result = default;

        // Evaluate child first
        if (!(Child?.TryEvaluate(evaluator, out Address child) ?? false))
            return false;

        return Operation switch
        {
            Operation.UnaryPlus => evaluator.TryUnaryPlus(this, child, out result),
            Operation.Negation => evaluator.TryNegate(this, child, out result),
            Operation.Not => evaluator.TryNot(this, child, out result),
            _ => ThrowHelper.ThrowArgumentException<bool>(),
        };
    }
}
