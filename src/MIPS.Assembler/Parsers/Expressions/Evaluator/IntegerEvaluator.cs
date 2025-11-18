// Adam Dernis 2024

namespace MIPS.Assembler.Parsers.Expressions.Evaluator;

/// <summary>
/// An <see cref="IEvaluator{T}"/> for integer expressions.
/// </summary>
public readonly struct IntegerEvaluator : IEvaluator<long>
{
    /// <inheritdoc/>
    public bool TryAdd(BinaryOperNode node, long left, long right, out long result)
    {
        result = left + right;
        return true;
    }

    /// <inheritdoc/>
    public bool TrySubtract(BinaryOperNode node, long left, long right, out long result)
    {
        result = left - right;
        return true;
    }

    /// <inheritdoc/>
    public bool TryMultiply(BinaryOperNode node, long left, long right, out long result)
    {
        result = left * right;
        return true;
    }

    /// <inheritdoc/>
    public bool TryDivide(BinaryOperNode node, long left, long right, out long result)
    {
        result = left / right;
        return true;
    }

    /// <inheritdoc/>
    public bool TryMod(BinaryOperNode node, long left, long right, out long result)
    {
        result = left % right;
        return true;
    }
    
    /// <inheritdoc/>
    public bool TryUnaryPlus(UnaryOperNode node, long value, out long result)
    {
        result = value;
        return true;
    }
    
    /// <inheritdoc/>
    public bool TryNegate(UnaryOperNode node, long value, out long result)
    {
        result = -value;
        return true;
    }

    /// <inheritdoc/>
    public bool TryAnd(BinaryOperNode node, long left, long right, out long result)
    {
        result = left & right;
        return true;
    }

    /// <inheritdoc/>
    public bool TryOr(BinaryOperNode node, long left, long right, out long result)
    {
        result = left | right;
        return true;
    }

    /// <inheritdoc/>
    public bool TryXor(BinaryOperNode node, long left, long right, out long result)
    {
        result = left ^ right;
        return true;
    }
    
    /// <inheritdoc/>
    public bool TryNot(UnaryOperNode node, long value, out long result)
    {
        result = ~value;
        return true;
    }
}
