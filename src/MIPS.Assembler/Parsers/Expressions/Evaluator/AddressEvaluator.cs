// Adam Dernis 2024

using MIPS.Assembler.Logging;
using MIPS.Assembler.Logging.Enum;
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
    public bool TryAdd(Address left, Address right, out Address result)
    {
        result = default;

        // If both address are relocatable
        if (left.IsRelocatable && right.IsRelocatable)
        {
            _logger?.Log(Severity.Error, LogId.InvalidExpressionOperation, "CantAddRelocatables");
            return false;
        }

        result = left + right.Value;
        return true;
    }

    /// <inheritdoc/>
    public bool TrySubtract(Address left, Address right, out Address result)
    {
        result = default;

        if (CheckRelocatable(right, "Subtract"))
            return false;

        result = left - right.Value;
        return true;
    }

    /// <inheritdoc/>
    public bool TryMultiply(Address left, Address right, out Address result)
    {
        result = default;

        // Cannot multiply relocatable addressing 
        if (CheckRelocatable(left, right, "Multiply"))
            return false;

        result = new Address(left.Value * right.Value, Section.None);
        return true;
    }

    /// <inheritdoc/>
    public bool TryDivide(Address left, Address right, out Address result)
    {
        result = default;

        // Cannot divide relocatable addressing
        if (CheckRelocatable(left, right, "Divide"))
            return false;

        result = new Address(left.Value / right.Value, Section.None);
        return true;
    }

    /// <inheritdoc/>
    public bool TryMod(Address left, Address right, out Address result)
    {
        result = default;

        // Cannot mod relocatable addressing
        if (CheckRelocatable(left, right, "Modulus"))
            return false;

        result = new Address(left.Value % right.Value, Section.None);
        return true;
    }
    
    /// <inheritdoc/>
    public readonly bool TryUnaryPlus(Address value, out Address result)
    {
        result = value;
        return true;
    }
    
    /// <inheritdoc/>
    public readonly bool TryNegate(Address value, out Address result)
    {
        result = default;

        // Cannot negate relocatable addressing
        if (CheckRelocatable(value, "Negate"))
            return false;

        result = new Address(-value.Value, Section.None);
        return true;
    }

    /// <inheritdoc/>
    public bool TryAnd(Address left, Address right, out Address result)
    {
        result = default;
        
        // Cannot AND relocatable addressing
        if (CheckRelocatable(left, right, "AND"))
            return false;

        result = new Address(left.Value & right.Value, Section.None);
        return true;
    }

    /// <inheritdoc/>
    public bool TryOr(Address left, Address right, out Address result)
    {
        result = default;
        
        // Cannot OR relocatable addressing
        if (CheckRelocatable(left, right, "OR"))
            return false;

        result = new Address(left.Value | right.Value, Section.None);
        return true;
    }

    /// <inheritdoc/>
    public bool TryXor(Address left, Address right, out Address result)
    {
        result = default;

        // Cannot XOR relocatable addressing
        if (CheckRelocatable(left, right, "XOR"))
            return false;

        result = new Address(left.Value ^ right.Value, Section.None);
        return true;
    }
    
    /// <inheritdoc/>
    public bool TryNot(Address value, out Address result)
    {
        result = default;

        // Cannot NOT relocatable addressing
        if (CheckRelocatable(value, "NOT"))
            return false;

        result = new Address(~value.Value, Section.None);
        return true;
    }

    private bool CheckRelocatable(Address value, string operation)
    {
        if (value.IsRelocatable)
        {
            _logger?.Log(Severity.Error, LogId.InvalidExpressionOperation, $"Cant{operation}Relocatable");
            return true;
        }
        return false;
    }

    private bool CheckRelocatable(Address left, Address right, string operation)
    {
        return CheckRelocatable(left, operation) || CheckRelocatable(right, operation);
    }
}
