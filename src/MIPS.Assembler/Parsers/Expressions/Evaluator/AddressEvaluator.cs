// Adam Dernis 2024

using CommunityToolkit.Diagnostics;
using MIPS.Assembler.Logging;
using MIPS.Assembler.Logging.Enum;
using MIPS.Assembler.Parsers.Expressions.Abstract;
using MIPS.Models.Addressing;
using MIPS.Models.Addressing.Enums;

namespace MIPS.Assembler.Parsers.Expressions.Evaluator;

/// <summary>
/// An <see cref="IEvaluator{T}"/> for address expressions.
/// </summary>
public readonly struct AddressEvaluator : IEvaluator<Address>
{
    private readonly ILogger? _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="AddressEvaluator"/> struct.
    /// </summary>
    public AddressEvaluator(ILogger? logger)
    {
        _logger = logger;
    }

    /// <inheritdoc/>
    public bool TryAdd(BinaryOperNode node, Address left, Address right, out Address result)
    {
        result = default;

        // If both address are relocatable
        if (left.IsRelocatable && right.IsRelocatable)
        {
            _logger?.Log(Severity.Error, LogId.InvalidExpressionOperation, node.ExpressionToken, "CantAddRelocatables");
            return false;
        }

        result = left + right.Value;
        return true;
    }

    /// <inheritdoc/>
    public bool TrySubtract(BinaryOperNode node, Address left, Address right, out Address result)
    {
        result = default;

        if (CheckRelocatable(node.RightChild, right, "Subtract"))
            return false;

        result = left - right.Value;
        return true;
    }

    /// <inheritdoc/>
    public bool TryMultiply(BinaryOperNode node, Address left, Address right, out Address result)
    {
        result = default;

        // Cannot multiply relocatable addressing 
        if (CheckRelocatable(node, left, right, "Multiply"))
            return false;

        result = new Address(left.Value * right.Value, Section.None);
        return true;
    }

    /// <inheritdoc/>
    public bool TryDivide(BinaryOperNode node, Address left, Address right, out Address result)
    {
        result = default;

        // Cannot divide relocatable addressing
        if (CheckRelocatable(node, left, right, "Divide"))
            return false;

        result = new Address(left.Value / right.Value, Section.None);
        return true;
    }

    /// <inheritdoc/>
    public bool TryMod(BinaryOperNode node, Address left, Address right, out Address result)
    {
        result = default;

        // Cannot mod relocatable addressing
        if (CheckRelocatable(node, left, right, "Modulus"))
            return false;

        result = new Address(left.Value % right.Value, Section.None);
        return true;
    }
    
    /// <inheritdoc/>
    public readonly bool TryUnaryPlus(UnaryOperNode node, Address value, out Address result)
    {
        result = value;
        return true;
    }
    
    /// <inheritdoc/>
    public readonly bool TryNegate(UnaryOperNode node, Address value, out Address result)
    {
        result = default;

        // Cannot negate relocatable addressing
        if (CheckRelocatable(node, value, "Negate"))
            return false;

        result = new Address(-value.Value, Section.None);
        return true;
    }

    /// <inheritdoc/>
    public bool TryAnd(BinaryOperNode node, Address left, Address right, out Address result)
    {
        result = default;
        
        // Cannot AND relocatable addressing
        if (CheckRelocatable(node, left, right, "AND"))
            return false;

        result = new Address(left.Value & right.Value, Section.None);
        return true;
    }

    /// <inheritdoc/>
    public bool TryOr(BinaryOperNode node, Address left, Address right, out Address result)
    {
        result = default;
        
        // Cannot OR relocatable addressing
        if (CheckRelocatable(node, left, right, "OR"))
            return false;

        result = new Address(left.Value | right.Value, Section.None);
        return true;
    }

    /// <inheritdoc/>
    public bool TryXor(BinaryOperNode node, Address left, Address right, out Address result)
    {
        result = default;

        // Cannot XOR relocatable addressing
        if (CheckRelocatable(node, left, right, "XOR"))
            return false;

        result = new Address(left.Value ^ right.Value, Section.None);
        return true;
    }
    
    /// <inheritdoc/>
    public bool TryNot(UnaryOperNode node, Address value, out Address result)
    {
        result = default;

        // Cannot NOT relocatable addressing
        if (CheckRelocatable(node, value, "NOT"))
            return false;

        result = new Address(~value.Value, Section.None);
        return true;
    }

    private bool CheckRelocatable(ExpNode? node, Address value, string operation)
    {
        Guard.IsNotNull(node);

        if (value.IsRelocatable)
        {
            _logger?.Log(Severity.Error, LogId.InvalidExpressionOperation, node.ExpressionToken, $"Cant{operation}Relocatable");
            return true;
        }
        return false;
    }

    private bool CheckRelocatable(BinaryOperNode node, Address left, Address right, string operation)
    {
        return CheckRelocatable(node.LeftChild, left, operation) || CheckRelocatable(node.RightChild, right, operation);
    }
}
