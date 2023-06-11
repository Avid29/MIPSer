// Adam Dernis 2023

using MIPS.Assembler.Parsers.Expressions.Abstract;
using MIPS.Assembler.Parsers.Expressions.Enums;
using MIPS.Assembler.Parsers.Expressions.Evaluator;

namespace MIPS.Assembler.Parsers.Expressions;

/// <summary>
/// A class for an operator in an expression tree.
/// </summary>
public class OperNode : ExpNode
{
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
    
    /// <summary>
    /// Gets or sets the left hand child of the <see cref="OperNode"/>.
    /// </summary>
    public ExpNode? LeftChild { get; set; }
    
    /// <summary>
    /// Gets or sets the right hand child of the <see cref="OperNode"/>.
    /// </summary>
    public ExpNode? RightChild { get; set; }

    /// <summary>
    /// Evaluate the expression tree as a type.
    /// </summary>
    /// <typeparam name="T">The type of the result.</typeparam>
    /// <param name="evaluator"></param>
    /// <param name="result">The evaluated expression tree.</param>
    /// <returns></returns>
    public bool TryEvaluate<T>(IEvaluator<T> evaluator, out T? result)
    {
        result = default;

        if (LeftChild is not ValueNode<T> leftValue)
        {
            // TODO: Log error
            return false;
        }

        if (RightChild is not ValueNode<T> rightValue)
        {
            // TODO: Log error
            return false;
        }

        T left = leftValue.Value;
        T right = rightValue.Value;

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
}
