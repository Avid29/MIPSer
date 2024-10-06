// Adam Dernis 2023

using CommunityToolkit.Diagnostics;
using MIPS.Assembler.Parsers.Expressions.Abstract;
using MIPS.Assembler.Parsers.Expressions.Enums;
using MIPS.Assembler.Parsers.Expressions.Evaluator;
using MIPS.Models.Addressing;

namespace MIPS.Assembler.Parsers.Expressions;

/// <summary>
/// A class for an operator in an expression tree.
/// </summary>
public class OperNode : ExpNode
{
    private ExpNode? _left;
    private ExpNode? _right;

    /// <summary>
    /// Initializes a new instance of the <see cref="OperNode"/> class.
    /// </summary>
    public OperNode(Operation operation)
    {
        Operation = operation;
    }

    /// <summary>
    /// Gets or sets the operation of the <see cref="OperNode"/>.
    /// </summary>
    public Operation Operation { get; }

    /// <summary>
    /// Gets the type of the expression.
    /// </summary>
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

    /// <summary>
    /// Gets or sets the left hand child of the <see cref="OperNode"/>.
    /// </summary>
    public ExpNode? LeftChild
    {
        get => _left;
        set => SetChild(ref _left, value);
    }

    /// <summary>
    /// Gets or sets the right hand child of the <see cref="OperNode"/>.
    /// </summary>
    public ExpNode? RightChild
    {
        get => _right;
        set => SetChild(ref _right, value);
    }

    private void SetChild(ref ExpNode? child, ExpNode? value)
    {
        // Clear current child's parent
        if (child is not null)
        {
            child.Parent = null;
        }

        // Assign new child's parent
        if (value is not null)
        {
            value.Parent = this;
        }

        // Assign new child
        child = value;

    }

    /// <summary>
    /// Gets the priority of the operation by order of operations.
    /// </summary>
    /// <remarks>
    /// Lower is lower on the tree and therefore executed earlier.
    /// </remarks>
    public int Priority => Operation switch
    {
        Operation.Addition => 3,
        Operation.Subtraction => 3,
        Operation.Multiplication => 2,
        Operation.Division => 2,
        Operation.Modulus => 2,
        Operation.And => 1,
        Operation.Or => 1,
        Operation.Xor => 1,
        _ => ThrowHelper.ThrowArgumentException<int>("Cannot assess priority of invalid operation.")
    };
}
