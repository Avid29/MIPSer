// Adam Dernis 2024

namespace MIPS.Assembler.Parsers.Expressions.Evaluator;

/// <summary>
/// An interface for applying operations 
/// </summary>
/// <typeparam name="T">The type being simplified.</typeparam>
public interface IEvaluator<T>
{
    /// <summary>
    /// Add <paramref name="left"/> and <paramref name="right"/>.
    /// </summary>
    /// <param name="left">The left-hand child.</param>
    /// <param name="right">The right-hand child.</param>
    /// <param name="result">The sum of <paramref name="left"/> and <paramref name="right"/>.</param>
    /// <returns>Whether or not the sum of the items could be taken.</returns>
    bool TryAdd(T left, T right, out T result);

    /// <summary>
    /// Subtract <paramref name="right"/> from <paramref name="left"/>.
    /// </summary>
    /// <param name="left">The left-hand child.</param>
    /// <param name="right">The right-hand child.</param>
    /// <param name="result">The difference between <paramref name="left"/> and <paramref name="right"/></param>
    /// <returns>Whether or not the difference of the items could be taken.</returns>
    bool TrySubtract(T left, T right, out T result);

    /// <summary>
    /// Multiply <paramref name="left"/> and <paramref name="right"/>.
    /// </summary>
    /// <param name="left">The left-hand child.</param>
    /// <param name="right">The right-hand child.</param>
    /// <param name="result">The product of <paramref name="left"/> and <paramref name="right"/>.</param>
    /// <returns>Whether or not the product of the items could be taken.</returns>
    bool TryMultiply(T left, T right, out T result);

    /// <summary>
    /// Divide <paramref name="left"/> by <paramref name="right"/>.
    /// </summary>
    /// <param name="left">The left-hand child.</param>
    /// <param name="right">The right-hand child.</param>
    /// <param name="result">The quotient of <paramref name="left"/> divided by <paramref name="right"/>.</param>
    /// <returns>Whether or not the quotient of the items could be taken.</returns>
    bool TryDivide(T left, T right, out T result);

    /// <summary>
    /// Modulus of <paramref name="left"/> divided by <paramref name="right"/>.
    /// </summary>
    /// <param name="left">The left-hand child.</param>
    /// <param name="right">The right-hand child.</param>
    /// <param name="result">The remainder of <paramref name="left"/> divided by <paramref name="right"/>.</param>
    /// <returns>Whether or not the remainder of dividing the items could be taken.</returns>
    bool TryMod(T left, T right, out T result);

    /// <summary>
    /// Logical AND of <paramref name="left"/> and <paramref name="right"/>.
    /// </summary>
    /// <param name="left">The left-hand child.</param>
    /// <param name="right">The right-hand child.</param>
    /// <param name="result">Logical AND of <paramref name="left"/> and <paramref name="right"/>.</param>
    /// <returns>Whether or not the Logical AND of the items could be taken.</returns>
    bool TryAnd(T left, T right, out T result);

    /// <summary>
    /// Logical OR of <paramref name="left"/> and <paramref name="right"/>.
    /// </summary>
    /// <param name="left">The left-hand child.</param>
    /// <param name="right">The right-hand child.</param>
    /// <param name="result">Logical OR of <paramref name="left"/> and <paramref name="right"/>.</param>
    /// <returns>Whether or not the Logical OR of the items could be taken.</returns>
    bool TryOr(T left, T right, out T result);

    /// <summary>
    /// Logical XOR of <paramref name="left"/> and <paramref name="right"/>.
    /// </summary>
    /// <param name="left">The left-hand child.</param>
    /// <param name="right">The right-hand child.</param>
    /// <param name="result">Logical XOR of <paramref name="left"/> and <paramref name="right"/>.</param>
    /// <returns>Whether or not the Logical XOR of the items could be taken.</returns>
    bool TryXor(T left, T right, out T result);
}
