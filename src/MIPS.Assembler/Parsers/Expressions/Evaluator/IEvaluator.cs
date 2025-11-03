// Adam Dernis 2024

using MIPS.Assembler.Parsers.Expressions.Abstract;

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
    /// <param name="node">The expression node being evaluated.</param>
    /// <param name="left">The left-hand child.</param>
    /// <param name="right">The right-hand child.</param>
    /// <param name="result">The sum of <paramref name="left"/> and <paramref name="right"/>.</param>
    /// <returns>Whether or not the sum of the items could be taken.</returns>
    bool TryAdd(BinaryOperNode node, T left, T right, out T result);

    /// <summary>
    /// Subtract <paramref name="right"/> from <paramref name="left"/>.
    /// </summary>
    /// <param name="node">The expression node being evaluated.</param>
    /// <param name="left">The left-hand child.</param>
    /// <param name="right">The right-hand child.</param>
    /// <param name="result">The difference between <paramref name="left"/> and <paramref name="right"/></param>
    /// <returns>Whether or not the difference of the items could be taken.</returns>
    bool TrySubtract(BinaryOperNode node, T left, T right, out T result);

    /// <summary>
    /// Multiply <paramref name="left"/> and <paramref name="right"/>.
    /// </summary>
    /// <param name="node">The expression node being evaluated.</param>
    /// <param name="left">The left-hand child.</param>
    /// <param name="right">The right-hand child.</param>
    /// <param name="result">The product of <paramref name="left"/> and <paramref name="right"/>.</param>
    /// <returns>Whether or not the product of the items could be taken.</returns>
    bool TryMultiply(BinaryOperNode node, T left, T right, out T result);

    /// <summary>
    /// Divide <paramref name="left"/> by <paramref name="right"/>.
    /// </summary>
    /// <param name="node">The expression node being evaluated.</param>
    /// <param name="left">The left-hand child.</param>
    /// <param name="right">The right-hand child.</param>
    /// <param name="result">The quotient of <paramref name="left"/> divided by <paramref name="right"/>.</param>
    /// <returns>Whether or not the quotient of the items could be taken.</returns>
    bool TryDivide(BinaryOperNode node, T left, T right, out T result);

    /// <summary>
    /// Modulus of <paramref name="left"/> divided by <paramref name="right"/>.
    /// </summary>
    /// <param name="node">The expression node being evaluated.</param>
    /// <param name="left">The left-hand child.</param>
    /// <param name="right">The right-hand child.</param>
    /// <param name="result">The remainder of <paramref name="left"/> divided by <paramref name="right"/>.</param>
    /// <returns>Whether or not the remainder of dividing the items could be taken.</returns>
    bool TryMod(BinaryOperNode node, T left, T right, out T result);

    /// <summary>
    /// Apply a unary plus to <paramref name="value"/>.
    /// </summary>
    /// <param name="node">The expression node being evaluated.</param>
    /// <param name="value">The child.</param>
    /// <param name="result">The result of a unary plus on <paramref name="value"/>.</param>
    /// <returns>Whether or not a unary plus of the child could be taken </returns>
    bool TryUnaryPlus(UnaryOperNode node, T value, out T result);

    /// <summary>
    /// Negate <paramref name="value"/>.
    /// </summary>
    /// <param name="node">The expression node being evaluated.</param>
    /// <param name="value">The child.</param>
    /// <param name="result">Negation of <paramref name="value"/>.</param>
    /// <returns>Whether or not the negation of the child could be taken.</returns>
    bool TryNegate(UnaryOperNode node, T value, out T result);

    /// <summary>
    /// Logical AND of <paramref name="left"/> and <paramref name="right"/>.
    /// </summary>
    /// <param name="node">The expression node being evaluated.</param>
    /// <param name="left">The left-hand child.</param>
    /// <param name="right">The right-hand child.</param>
    /// <param name="result">Logical AND of <paramref name="left"/> and <paramref name="right"/>.</param>
    /// <returns>Whether or not the Logical AND of the items could be taken.</returns>
    bool TryAnd(BinaryOperNode node, T left, T right, out T result);

    /// <summary>
    /// Logical OR of <paramref name="left"/> and <paramref name="right"/>.
    /// </summary>
    /// <param name="node">The expression node being evaluated.</param>
    /// <param name="left">The left-hand child.</param>
    /// <param name="right">The right-hand child.</param>
    /// <param name="result">Logical OR of <paramref name="left"/> and <paramref name="right"/>.</param>
    /// <returns>Whether or not the Logical OR of the items could be taken.</returns>
    bool TryOr(BinaryOperNode node, T left, T right, out T result);

    /// <summary>
    /// Logical XOR of <paramref name="left"/> and <paramref name="right"/>.
    /// </summary>
    /// <param name="node">The expression node being evaluated.</param>
    /// <param name="left">The left-hand child.</param>
    /// <param name="right">The right-hand child.</param>
    /// <param name="result">Logical XOR of <paramref name="left"/> and <paramref name="right"/>.</param>
    /// <returns>Whether or not the Logical XOR of the items could be taken.</returns>
    bool TryXor(BinaryOperNode node, T left, T right, out T result);

    /// <summary>
    /// Logical NOT of <paramref name="value"/>.
    /// </summary>
    /// <param name="node">The expression node being evaluated.</param>
    /// <param name="value">The child.</param>
    /// <param name="result">Logical NOT of <paramref name="value"/>.</param>
    /// <returns>Whether or not the logical NOT of the child could be taken.</returns>
    bool TryNot(UnaryOperNode node, T value, out T result);
}
