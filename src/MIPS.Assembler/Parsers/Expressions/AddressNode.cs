// Adam Dernis 2023

using MIPS.Assembler.Parsers.Expressions.Abstract;
using MIPS.Assembler.Parsers.Expressions.Enums;
using MIPS.Assembler.Parsers.Expressions.Evaluator;
using MIPS.Models.Addressing;
using MIPS.Models.Addressing.Enums;

namespace MIPS.Assembler.Parsers.Expressions;

/// <summary>
/// A class for an integer node on an expression tree.
/// </summary>
public class AddressNode : ValueNode<Address>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="AddressNode"/> class.
    /// </summary>
    public AddressNode(long value, Section section = Section.None) : this(new Address(value, section))
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="AddressNode"/> class.
    /// </summary>
    public AddressNode(Address value) : base(value)
    {
    }

    /// <inheritdoc/>
    public override ExpressionType Type => ExpressionType.Integer;

    /// <inheritdoc/>
    public override bool TryEvaluate(IEvaluator<Address> evaluator, out Address result)
    {
        result = Value;
        return true;
    }
}
