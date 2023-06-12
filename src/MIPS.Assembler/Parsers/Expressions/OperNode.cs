// Adam Dernis 2023

using MIPS.Assembler.Parsers.Expressions.Abstract;
using MIPS.Assembler.Parsers.Expressions.Enums;
using MIPS.Assembler.Parsers.Expressions.Evaluator;
using System.Diagnostics.CodeAnalysis;

namespace MIPS.Assembler.Parsers.Expressions;

/// <summary>
/// A class for an operator in an expression tree.
/// </summary>
public class OperNode<T> : ExpNode<T>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="OperNode{T}"/> class.
    /// </summary>
    public OperNode(Operation operation)
    {
        Operation = operation;
    }

    /// <summary>
    /// Gets or sets the operation of the <see cref="OperNode{T}"/>.
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
    public override bool TryEvaluate(IEvaluator<T> evaluator, out T? result)
    {
        result = default;
        
        T? left = default;
        T? right = default;

        if ((LeftChild?.TryEvaluate(evaluator, out left) ?? false) || left is null)
        {
            // TODO: Log error
            return false;
        }

        if ((RightChild?.TryEvaluate(evaluator, out right) ?? false) || right is null)
        {
            // TODO: Log error
            return false;
        }

        result =  Operation switch
        {
            // Arithmetic
            Operation.Addition => evaluator.Add(left, right),
            Operation.Subtraction => evaluator.Subtract(left, right),
            Operation.Multiplication => evaluator.Multiply(left, right),
            Operation.Division => evaluator.Divide(left, right),
            Operation.Modulus => evaluator.Mod(left, right),

            // Logical
            Operation.And => evaluator.And(left, right),
            Operation.Or => evaluator.Or(left, right),
            Operation.Xor => evaluator.Xor(left, right),
            _ => default,
        };

        return true;
    }

    /// <summary>
    /// Gets or sets the left hand child of the <see cref="OperNode{T}"/>.
    /// </summary>
    public ExpNode<T>? LeftChild { get; set; }
    
    /// <summary>
    /// Gets or sets the right hand child of the <see cref="OperNode{T}"/>.
    /// </summary>
    public ExpNode<T>? RightChild { get; set; }

    
}
