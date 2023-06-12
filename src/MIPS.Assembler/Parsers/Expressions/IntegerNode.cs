// Adam Dernis 2023

using MIPS.Assembler.Parsers.Expressions.Abstract;
using MIPS.Assembler.Parsers.Expressions.Enums;
using MIPS.Assembler.Parsers.Expressions.Evaluator;

namespace MIPS.Assembler.Parsers.Expressions;

/// <summary>
/// A class for an integer node on an expression tree.
/// </summary>
public class IntegerNode : ValueNode
{
    /// <summary>
    /// Initializes a new instance of the <see cref="IntegerNode"/> class.
    /// </summary>
    public IntegerNode(long value) : base(value)
    {
    }

    /// <inheritdoc/>
    public override ExpressionType Type => ExpressionType.Integer;
    
    /// <inheritdoc/>
    public override bool TryEvaluate(IEvaluator<long> evaluator, out long result)
    {
        result = Value;
        return true;
    }
}
