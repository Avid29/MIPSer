// Adam Dernis 2024

using MIPS.Assembler.Parsers.Expressions.Abstract;
using MIPS.Assembler.Parsers.Expressions.Enums;
using MIPS.Assembler.Tokenization.Models;
using MIPS.Models.Addressing;

namespace MIPS.Assembler.Parsers.Expressions;

/// <summary>
/// A class for an integer node on an expression tree.
/// </summary>
public class AddressNode : ValueNode<Address>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="AddressNode"/> class.
    /// </summary>
    public AddressNode(Token token, long value, string? section = null) : this(token, new Address(value, section))
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="AddressNode"/> class.
    /// </summary>
    public AddressNode(Token token, Address value) : base(token, value)
    {
    }

    /// <inheritdoc/>
    public override ExpressionType Type => ExpressionType.Integer;

    /// <inheritdoc/>
    public override bool TryEvaluate(Evaluator evaluator, out ExpressionResult result)
    {
        result = new ExpressionResult(Value);
        return true;
    }
}
