// Adam Dernis 2023

namespace MIPS.Assembler.Parsers.Expressions.Evaluator;

/// <summary>
/// An <see cref="IEvaluator{T}"/> for integer expressions.
/// </summary>
public struct IntegerEvaluator : IEvaluator<long>
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
}
