// Adam Dernis 2023

using CommunityToolkit.Diagnostics;

namespace MIPS.Assembler.Parsers.Expressions.Evaluator;

/// <summary>
/// An <see cref="IEvaluator{T}"/> for string expressions.
/// </summary>
public class StringEvaluator : IEvaluator<string>
{
    /// <inheritdoc/>
    public string Add(string left, string right) => left + right;
    
    /// <inheritdoc/>
    public string And(string left, string right) => ThrowHelper.ThrowNotSupportedException<string>();
    
    /// <inheritdoc/>
    public string Divide(string left, string right) => ThrowHelper.ThrowNotSupportedException<string>();
    
    /// <inheritdoc/>
    public string Mod(string left, string right) => ThrowHelper.ThrowNotSupportedException<string>();
    
    /// <inheritdoc/>
    public string Multiply(string left, string right) => ThrowHelper.ThrowNotSupportedException<string>();
    
    /// <inheritdoc/>
    public string Or(string left, string right) => ThrowHelper.ThrowNotSupportedException<string>();
    
    /// <inheritdoc/>
    public string Subtract(string left, string right) => ThrowHelper.ThrowNotSupportedException<string>();
    
    /// <inheritdoc/>
    public string Xor(string left, string right) => ThrowHelper.ThrowNotSupportedException<string>();
}
