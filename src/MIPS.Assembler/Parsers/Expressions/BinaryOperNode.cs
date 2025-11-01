// Adam Dernis 2024

using CommunityToolkit.Diagnostics;
using MIPS.Assembler.Parsers.Expressions.Abstract;
using MIPS.Assembler.Parsers.Expressions.Enums;
using MIPS.Assembler.Parsers.Expressions.Evaluator;
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
    public BinaryOperNode(Operation operation) : base(operation)
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

        if (!(LeftChild?.TryEvaluate(evaluator, out Address left) ?? false))
        {
            // TODO: Log error
            return false;
        }

        if (!(RightChild?.TryEvaluate(evaluator, out Address right) ?? false))
        {
            // TODO: Log error
            return false;
        }

        return Operation switch
        {
            // Arithmetic
            Operation.Addition => evaluator.TryAdd(left, right, out result),
            Operation.Subtraction => evaluator.TrySubtract(left, right, out result),
            Operation.Multiplication => evaluator.TryMultiply(left, right, out result),
            Operation.Division => evaluator.TryDivide(left, right, out result),
            Operation.Modulus => evaluator.TryMod(left, right, out result),

            // Logical
            Operation.And => evaluator.TryAnd(left, right, out result),
            Operation.Or => evaluator.TryOr(left, right, out result),
            Operation.Xor => evaluator.TryXor(left, right, out result),


            _ => ThrowHelper.ThrowArgumentException<bool>(),
        };
    }
}
