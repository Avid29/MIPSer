// Adam Dernis 2023

namespace MIPS.Assembler.Parsers.Expressions.Evaluator;

/// <summary>
/// An <see cref="IEvaluator{T}"/> for integer expressions.
/// </summary>
public struct IntegerEvaluator : IEvaluator<long>
{
    /// <inheritdoc/>
    public readonly bool TryAdd(long left, long right, out long result)
    {
        result = left + right;
        return true;
    }

    /// <inheritdoc/>
    public readonly bool TrySubtract(long left, long right, out long result)
    {
        result = left - right;
        return true;
    }

    /// <inheritdoc/>
    public readonly bool TryMultiply(long left, long right, out long result)
    {
        result = left * right;
        return true;
    }

    /// <inheritdoc/>
    public readonly bool TryDivide(long left, long right, out long result)
    {
        result = left / right;
        return true;
    }

    /// <inheritdoc/>
    public readonly bool TryMod(long left, long right, out long result)
    {
        result = left % right;
        return true;
    }

    /// <inheritdoc/>
    public readonly bool TryAnd(long left, long right, out long result)
    {
        result = left & right;
        return true;
    }

    /// <inheritdoc/>
    public readonly bool TryOr(long left, long right, out long result)
    {
        result = left | right;
        return true;
    }

    /// <inheritdoc/>
    public readonly bool TryXor(long left, long right, out long result)
    {
        result = left ^ right;
        return true;
    }
}
