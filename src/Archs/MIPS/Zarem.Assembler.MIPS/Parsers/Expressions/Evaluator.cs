// Adam Dernis 2024

using CommunityToolkit.Diagnostics;
using Zarem.Assembler.MIPS.Logging;
using Zarem.Assembler.MIPS.Logging.Enum;
using Zarem.Assembler.MIPS.Models;
using Zarem.Assembler.MIPS.Parsers.Expressions.Abstract;
using Zarem.MIPS.Models.Addressing;
using Zarem.MIPS.Models.Modules.Tables;
using Zarem.MIPS.Models.Modules.Tables.Enums;

namespace Zarem.Assembler.MIPS.Parsers.Expressions;

/// <summary>
/// A struct for applying operations.
/// </summary>
public struct Evaluator
{
    private readonly ILogger? _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="Evaluator"/> struct.
    /// </summary>
    public Evaluator(AssemblerContext? context, ILogger? logger)
    {
        _logger = logger;
        Context = context;
    }

    /// <summary>
    /// Gets the assembler content to use by the evaluator.
    /// </summary>
    public AssemblerContext? Context { get; }

    /// <summary>
    /// Add <paramref name="left"/> and <paramref name="right"/>.
    /// </summary>
    /// <param name="node">The expression node being evaluated.</param>
    /// <param name="left">The left-hand child.</param>
    /// <param name="right">The right-hand child.</param>
    /// <param name="result">The sum of <paramref name="left"/> and <paramref name="right"/>.</param>
    /// <returns>Whether or not the sum of the items could be taken.</returns>
    public bool TryAdd(BinaryOperNode node, ExpressionResult left, ExpressionResult right, out ExpressionResult result)
    {
        result = default;

        // If both address are relocatable
        if (!Address.TryAdd(left.Value, right.Value, out var value))
        {
            _logger?.Log(Severity.Error, LogCode.InvalidExpressionOperation, node.ExpressionToken, "CantAddRelocatables");
            return false;
        }

        ReferenceEntry? reference = null;
        if (value.IsRelocatable)
        {
            var symbol = left.Reference?.Symbol ?? right.Reference?.Symbol;
            reference = new ReferenceEntry(symbol, Context?.CurrentAddress ?? default, MipsReferenceType.None, value.Value);
        }

        result = new ExpressionResult(value, reference);
        return true;
    }

    /// <summary>
    /// Subtract <paramref name="right"/> from <paramref name="left"/>.
    /// </summary>
    /// <param name="node">The expression node being evaluated.</param>
    /// <param name="left">The left-hand child.</param>
    /// <param name="right">The right-hand child.</param>
    /// <param name="result">The difference between <paramref name="left"/> and <paramref name="right"/></param>
    /// <returns>Whether or not the difference of the items could be taken.</returns>
    public bool TrySubtract(BinaryOperNode node, ExpressionResult left, ExpressionResult right, out ExpressionResult result)
    {
        result = default;

        // If both address are relocatable
        if (!Address.TrySubtract(left.Value, right.Value, out var value))
        {
            _logger?.Log(Severity.Error, LogCode.InvalidExpressionOperation, node.ExpressionToken, "CantSubtractRelocatables");
            return false;
        }

        ReferenceEntry? reference = null;
        if (value.IsRelocatable)
        {
            var symbol = left.Reference?.Symbol ?? right.Reference?.Symbol;
            reference = new ReferenceEntry(symbol, Context?.CurrentAddress ?? default, MipsReferenceType.None, value.Value);
        }

        result = new(value, reference);
        return true;
    }

    /// <summary>
    /// Multiply <paramref name="left"/> and <paramref name="right"/>.
    /// </summary>
    /// <param name="node">The expression node being evaluated.</param>
    /// <param name="left">The left-hand child.</param>
    /// <param name="right">The right-hand child.</param>
    /// <param name="result">The product of <paramref name="left"/> and <paramref name="right"/>.</param>
    /// <returns>Whether or not the product of the items could be taken.</returns>
    public bool TryMultiply(BinaryOperNode node, ExpressionResult left, ExpressionResult right, out ExpressionResult result)
    {
        result = default;

        // Cannot multiply relocatable addressing 
        if (CheckRelocatable(node, left, right, "Multiply"))
            return false;

        result = new(new Address(left.Value.Value * right.Value.Value));
        return true;
    }

    /// <summary>
    /// Divide <paramref name="left"/> by <paramref name="right"/>.
    /// </summary>
    /// <param name="node">The expression node being evaluated.</param>
    /// <param name="left">The left-hand child.</param>
    /// <param name="right">The right-hand child.</param>
    /// <param name="result">The quotient of <paramref name="left"/> divided by <paramref name="right"/>.</param>
    /// <returns>Whether or not the quotient of the items could be taken.</returns>
    public bool TryDivide(BinaryOperNode node, ExpressionResult left, ExpressionResult right, out ExpressionResult result)
    {
        result = default;

        // Cannot divide relocatable addressing
        if (CheckRelocatable(node, left, right, "Divide"))
            return false;

        result = new(new Address(left.Value.Value / right.Value.Value));
        return true;
    }

    /// <summary>
    /// Modulus of <paramref name="left"/> divided by <paramref name="right"/>.
    /// </summary>
    /// <param name="node">The expression node being evaluated.</param>
    /// <param name="left">The left-hand child.</param>
    /// <param name="right">The right-hand child.</param>
    /// <param name="result">The remainder of <paramref name="left"/> divided by <paramref name="right"/>.</param>
    /// <returns>Whether or not the remainder of dividing the items could be taken.</returns>
    public bool TryMod(BinaryOperNode node, ExpressionResult left, ExpressionResult right, out ExpressionResult result)
    {
        result = default;

        // Cannot mod relocatable addressing
        if (CheckRelocatable(node, left, right, "Modulus"))
            return false;

        result = new(new Address(left.Value.Value % right.Value.Value));
        return true;
    }

    /// <summary>
    /// Apply a unary plus to <paramref name="value"/>.
    /// </summary>
    /// <param name="node">The expression node being evaluated.</param>
    /// <param name="value">The child.</param>
    /// <param name="result">The result of a unary plus on <paramref name="value"/>.</param>
    /// <returns>Whether or not a unary plus of the child could be taken </returns>
    public bool TryUnaryPlus(UnaryOperNode node, ExpressionResult value, out ExpressionResult result)
    {
        result = value;
        return true;
    }

    /// <summary>
    /// Negate <paramref name="value"/>.
    /// </summary>
    /// <param name="node">The expression node being evaluated.</param>
    /// <param name="value">The child.</param>
    /// <param name="result">Negation of <paramref name="value"/>.</param>
    /// <returns>Whether or not the negation of the child could be taken.</returns>
    public bool TryNegate(UnaryOperNode node, ExpressionResult value, out ExpressionResult result)
    {
        result = default;

        // Cannot negate relocatable addressing
        if (CheckRelocatable(node, value, "Negate"))
            return false;

        result = new(new Address(-value.Value.Value));
        return true;
    }

    /// <summary>
    /// Logical AND of <paramref name="left"/> and <paramref name="right"/>.
    /// </summary>
    /// <param name="node">The expression node being evaluated.</param>
    /// <param name="left">The left-hand child.</param>
    /// <param name="right">The right-hand child.</param>
    /// <param name="result">Logical AND of <paramref name="left"/> and <paramref name="right"/>.</param>
    /// <returns>Whether or not the Logical AND of the items could be taken.</returns>
    public bool TryAnd(BinaryOperNode node, ExpressionResult left, ExpressionResult right, out ExpressionResult result)
    {
        result = default;

        // Cannot AND relocatable addressing
        if (CheckRelocatable(node, left, right, "AND"))
            return false;

        result = new(new Address(left.Value.Value & right.Value.Value));
        return true;
    }

    /// <summary>
    /// Logical OR of <paramref name="left"/> and <paramref name="right"/>.
    /// </summary>
    /// <param name="node">The expression node being evaluated.</param>
    /// <param name="left">The left-hand child.</param>
    /// <param name="right">The right-hand child.</param>
    /// <param name="result">Logical OR of <paramref name="left"/> and <paramref name="right"/>.</param>
    /// <returns>Whether or not the Logical OR of the items could be taken.</returns>
    public bool TryOr(BinaryOperNode node, ExpressionResult left, ExpressionResult right, out ExpressionResult result)
    {
        result = default;

        // Cannot OR relocatable addressing
        if (CheckRelocatable(node, left, right, "OR"))
            return false;

        result = new(new Address(left.Value.Value | right.Value.Value));
        return true;
    }

    /// <summary>
    /// Logical XOR of <paramref name="left"/> and <paramref name="right"/>.
    /// </summary>
    /// <param name="node">The expression node being evaluated.</param>
    /// <param name="left">The left-hand child.</param>
    /// <param name="right">The right-hand child.</param>
    /// <param name="result">Logical XOR of <paramref name="left"/> and <paramref name="right"/>.</param>
    /// <returns>Whether or not the Logical XOR of the items could be taken.</returns>
    public bool TryXor(BinaryOperNode node, ExpressionResult left, ExpressionResult right, out ExpressionResult result)
    {
        result = default;

        // Cannot XOR relocatable addressing
        if (CheckRelocatable(node, left, right, "XOR"))
            return false;

        result = new(new Address(left.Value.Value ^ right.Value.Value));
        return true;
    }

    /// <summary>
    /// Logical NOT of <paramref name="value"/>.
    /// </summary>
    /// <param name="node">The expression node being evaluated.</param>
    /// <param name="value">The child.</param>
    /// <param name="result">Logical NOT of <paramref name="value"/>.</param>
    /// <returns>Whether or not the logical NOT of the child could be taken.</returns>
    public bool TryNot(UnaryOperNode node, ExpressionResult value, out ExpressionResult result)
    {
        result = default;

        // Cannot NOT relocatable addressing
        if (CheckRelocatable(node, value, "NOT"))
            return false;

        result = new(new Address(~value.Value.Value));
        return true;
    }

    private readonly bool CheckRelocatable(ExpNode? node, ExpressionResult value, string operation)
    {
        Guard.IsNotNull(node);

        if (value.IsRelocatable)
        {
            _logger?.Log(Severity.Error, LogCode.InvalidExpressionOperation, node.ExpressionToken, $"Cant{operation}Relocatable");
            return true;
        }
        return false;
    }

    private readonly bool CheckRelocatable(BinaryOperNode node, ExpressionResult left, ExpressionResult right, string operation)
    {
        return CheckRelocatable(node.LeftChild, left, operation) || CheckRelocatable(node.RightChild, right, operation);
    }
}
