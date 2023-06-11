// Adam Dernis 2023

namespace MIPS.Assembler.Parsers.Expressions.Evaluator;

/// <summary>
/// An <see cref="IEvaluator{T}"/> for integer expressions.
/// </summary>
public class IntegerEvaluator : IEvaluator<long>
{
    /// <inheritdoc/>
    public long Add(long left, long right) => left + right;
    
    /// <inheritdoc/>
    public long Subtract(long left, long right) => left - right;
    
    /// <inheritdoc/>
    public long Multiply(long left, long right) => left * right;
    
    /// <inheritdoc/>
    public long Divide(long left, long right) => left / right;
    
    /// <inheritdoc/>
    public long Mod(long left, long right) => left % right;

    /// <inheritdoc/>
    public long And(long left, long right) => left & right;
    
    /// <inheritdoc/>
    public long Or(long left, long right) => left | right;
}
