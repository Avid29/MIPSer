// Adam Dernis 2023

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
    /// <returns>The sum of <paramref name="left"/> and <paramref name="right"/>.</returns>
    T Add(T left, T right);

    /// <summary>
    /// Subtract <paramref name="right"/> from <paramref name="left"/>.
    /// </summary>
    /// <param name="left">The left-hand child.</param>
    /// <param name="right">The right-hand child.</param>
    /// <returns>The difference between <paramref name="left"/> and <paramref name="right"/></returns>
    T Subtract(T left, T right);

    /// <summary>
    /// Multiply <paramref name="left"/> and <paramref name="right"/>.
    /// </summary>
    /// <param name="left">The left-hand child.</param>
    /// <param name="right">The right-hand child.</param>
    /// <returns>The product of <paramref name="left"/> and <paramref name="right"/>.</returns>
    T Multiply(T left, T right);

    /// <summary>
    /// Divide <paramref name="left"/> by <paramref name="right"/>.
    /// </summary>
    /// <param name="left">The left-hand child.</param>
    /// <param name="right">The right-hand child.</param>
    /// <returns>The quotient of <paramref name="left"/> divided by <paramref name="right"/>.</returns>
    T Divide(T left, T right);

    /// <summary>
    /// Modulus of <paramref name="left"/> divided by <paramref name="right"/>.
    /// </summary>
    /// <param name="left">The left-hand child.</param>
    /// <param name="right">The right-hand child.</param>
    /// <returns>The remainder of <paramref name="left"/> divided by <paramref name="right"/>.</returns>
    T Mod(T left, T right);

    /// <summary>
    /// Logical AND of <paramref name="left"/> and <paramref name="right"/>.
    /// </summary>
    /// <param name="left">The left-hand child.</param>
    /// <param name="right">The right-hand child.</param>
    /// <returns>Logical AND of <paramref name="left"/> and <paramref name="right"/>.</returns>
    T And(T left, T right);

    /// <summary>
    /// Logical OR of <paramref name="left"/> and <paramref name="right"/>.
    /// </summary>
    /// <param name="left">The left-hand child.</param>
    /// <param name="right">The right-hand child.</param>
    /// <returns>Logical OR of <paramref name="left"/> and <paramref name="right"/>.</returns>
    T Or(T left, T right);

    /// <summary>
    /// Logical XOR of <paramref name="left"/> and <paramref name="right"/>.
    /// </summary>
    /// <param name="left">The left-hand child.</param>
    /// <param name="right">The right-hand child.</param>
    /// <returns>Logical XOR of <paramref name="left"/> and <paramref name="right"/>.</returns>
    T Xor(T left, T right);
}
