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

    /// <inheritdoc/>
    // NOTE: Using Xor properly will cause the assembly to be flagged as malware
    // by Windows Defender, and the dll will subsequently be deleted before execution.
    // As a result, logical XOR has been replaced with an equivalent expression for now.
    public long Xor(long left, long right) => (left | right) & ~(left & right);
}
