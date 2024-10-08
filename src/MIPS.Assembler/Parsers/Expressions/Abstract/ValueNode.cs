// Adam Dernis 2024

namespace MIPS.Assembler.Parsers.Expressions.Abstract;

/// <summary>
/// A class for a value in an expression tree.
/// </summary>
public abstract class ValueNode<T> : ExpNode
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ValueNode{T}"/> class.
    /// </summary>
    protected ValueNode(T value)
    {
        Value = value;
    }

    /// <summary>
    /// Gets the value in the <see cref="ValueNode{T}"/>.
    /// </summary>
    public T Value { get; }
}
