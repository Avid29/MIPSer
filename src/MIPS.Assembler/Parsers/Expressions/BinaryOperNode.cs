// Adam Dernis 2024

using CommunityToolkit.Diagnostics;
using MIPS.Assembler.Parsers.Expressions.Abstract;
using MIPS.Assembler.Parsers.Expressions.Enums;
using MIPS.Assembler.Parsers.Expressions.Evaluator;
using MIPS.Assembler.Tokenization;
using MIPS.Models.Addressing;

namespace MIPS.Assembler.Parsers.Expressions;

/// <summary>
/// A class for a binary operator in an expression tree.
/// </summary>
public class BinaryOperNode : OperNode
{
    private ExpNode? _left;
    private ExpNode? _right;

    /// <summary>
    /// Initializes a new instance of the <see cref="BinaryOperNode"/> class.
    /// </summary>
    public BinaryOperNode(Token token, Operation operation) : base(token, operation)
    {
    }

    /// <summary>
    /// Gets or sets the left hand child of the <see cref="BinaryOperNode"/>.
    /// </summary>
    public ExpNode? LeftChild
    {
        get => _left;
        set => SetChild(ref _left, value);
    }

    /// <summary>
    /// Gets or sets the right hand child of the <see cref="BinaryOperNode"/>.
    /// </summary>
    public ExpNode? RightChild
    {
        get => _right;
        set => SetChild(ref _right, value);
    }

    /// <inheritdoc/>
    /// <remarks>
    /// Determined by its children. Invalid if they don't match.
    /// </remarks>
    public override ExpressionType Type
    {
        get
        {
            // Check if types match and children aren't null
            if (LeftChild?.Type == RightChild?.Type)
            {
                // Return matched type, or invalid if match was null.
                return LeftChild?.Type ?? ExpressionType.Invalid;
            }

            // Types don't match, it's an invalid expression
            return ExpressionType.Invalid;
        }
    }
    
    /// <inheritdoc/>
    public override bool TryAddChild(ExpNode node)
    {
        // We can only add to the right child, left is always filled first
        if (RightChild is not null)
            return false;

        RightChild = node;
        return true;
    }
    
    /// <inheritdoc/>
    public override bool TryInsertNode(BinaryOperNode node)
    {
        // Insert the binary operator between this operator and its right child
        node.LeftChild = RightChild;
        RightChild = node;
        return true;
    }

    /// <inheritdoc/>
    public override bool TryEvaluate(IEvaluator<Address> evaluator, out Address result)
    {
        result = default;

        // Evaluate children, and return false if either fails.
        // They log their own errors, so no need to log another.
        if ((!(LeftChild?.TryEvaluate(evaluator, out Address left) ?? false)) ||
            (!(RightChild?.TryEvaluate(evaluator, out Address right) ?? false)))
            return false;

        return Operation switch
        {
            // Arithmetic
            Operation.Addition => evaluator.TryAdd(this, left, right, out result),
            Operation.Subtraction => evaluator.TrySubtract(this, left, right, out result),
            Operation.Multiplication => evaluator.TryMultiply(this, left, right, out result),
            Operation.Division => evaluator.TryDivide(this, left, right, out result),
            Operation.Modulus => evaluator.TryMod(this, left, right, out result),

            // Logical
            Operation.And => evaluator.TryAnd(this, left, right, out result),
            Operation.Or => evaluator.TryOr(this, left, right, out result),
            Operation.Xor => evaluator.TryXor(this, left, right, out result),


            _ => ThrowHelper.ThrowArgumentException<bool>(),
        };
    }
}
