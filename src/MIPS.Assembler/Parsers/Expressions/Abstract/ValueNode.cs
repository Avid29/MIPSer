// Adam Dernis 2023

namespace MIPS.Assembler.Parsers.Expressions.Abstract;

/// <summary>
/// A class for a value in an expression tree.
/// </summary>
public abstract class ValueNode : ExpNode
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ValueNode"/> class.
    /// </summary>
    protected ValueNode(long value)
    {
        Value = value;
    }

    /// <summary>
    /// Gets the value in the <see cref="ValueNode"/>.
    /// </summary>
    public long Value { get; }
}
