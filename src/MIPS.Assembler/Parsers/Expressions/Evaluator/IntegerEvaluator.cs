// Adam Dernis 2024

namespace MIPS.Assembler.Parsers.Expressions.Evaluator;

/// <summary>
/// An <see cref="IEvaluator{T}"/> for integer expressions.
/// </summary>
public readonly struct IntegerEvaluator : IEvaluator<long>
{
    /// <inheritdoc/>
    public bool TryAdd(long left, long right, out long result)
    {
        result = left + right;
        return true;
    }

    /// <inheritdoc/>
    public bool TrySubtract(long left, long right, out long result)
    {
        result = left - right;
        return true;
    }

    /// <inheritdoc/>
    public bool TryMultiply(long left, long right, out long result)
    {
        result = left * right;
        return true;
    }

    /// <inheritdoc/>
    public bool TryDivide(long left, long right, out long result)
    {
        result = left / right;
        return true;
    }

    /// <inheritdoc/>
    public bool TryMod(long left, long right, out long result)
    {
        result = left % right;
        return true;
    }
    
    /// <inheritdoc/>
    public bool TryUnaryPlus(long value, out long result)
    {
        result = value;
        return true;
    }
    
    /// <inheritdoc/>
    public bool TryNegate(long value, out long result)
    {
        result = -value;
        return true;
    }

    /// <inheritdoc/>
    public bool TryAnd(long left, long right, out long result)
    {
        result = left & right;
        return true;
    }

    /// <inheritdoc/>
    public bool TryOr(long left, long right, out long result)
    {
        result = left | right;
        return true;
    }

    /// <inheritdoc/>
    public bool TryXor(long left, long right, out long result)
    {
        result = left ^ right;
        return true;
    }
    
    /// <inheritdoc/>
    public bool TryNot(long value, out long result)
    {
        result = ~value;
        return true;
    }
}
