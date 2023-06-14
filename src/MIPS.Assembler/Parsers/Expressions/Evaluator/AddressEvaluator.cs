// Adam Dernis 2023

using MIPS.Assembler.Logging;
using MIPS.Assembler.Logging.Enum;
using MIPS.Models.Addressing;
using MIPS.Models.Addressing.Enums;

namespace MIPS.Assembler.Parsers.Expressions.Evaluator;

/// <summary>
/// An <see cref="IEvaluator{T}"/> for address expressions.
/// </summary>
public struct AddressEvaluator : IEvaluator<Address>
{
    private ILogger? _logger;

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
            _logger?.Log(Severity.Error, LogId.InvalidExpressionOperation, "Cannot sum two relative symbols.");
            return false;
        }

        // Determine the resulting section
        var section = left.Section;
        if (section is Section.None)
            section = right.Section;

        result = new Address(left.Value + right.Value, section);
        return true;
    }
    
    /// <inheritdoc/>
    public bool TrySubtract(Address left, Address right, out Address result)
    {
        result = default;
        
        // TODO: Handle subtraction relocation
        if (left.IsRelocatable || right.IsRelocatable)
        {
            _logger?.Log(Severity.Error, LogId.InvalidExpressionOperation, "Cannot subtract with relative symbols.");
            return false;
        }
        
        result = new Address(left.Value - right.Value, Section.None);
        return true;
    }
    
    /// <inheritdoc/>
    public bool TryMultiply(Address left, Address right, out Address result)
    {
        result = default;

        // Cannot multiply relocatable addressing 
        if (left.IsRelocatable || right.IsRelocatable)
        {
            _logger?.Log(Severity.Error, LogId.InvalidExpressionOperation, "Cannot multiply with relative symbols.");
            return false;
        }

        result = new Address(left.Value * right.Value, Section.None);
        return true;
    }
    
    /// <inheritdoc/>
    public bool TryDivide(Address left, Address right, out Address result)
    {
        result = default;

        // Cannot divide relocatable addressing
        if (left.IsRelocatable || right.IsRelocatable)
        {
            _logger?.Log(Severity.Error, LogId.InvalidExpressionOperation, "Cannot divide with relative symbols.");
            return false;
        }

        result = new Address(left.Value / right.Value, Section.None);
        return true;
    }
    
    /// <inheritdoc/>
    public bool TryMod(Address left, Address right, out Address result)
    {
        result = default;

        // Cannot mod relocatable addressing
        if (left.IsRelocatable || right.IsRelocatable)
        {
            _logger?.Log(Severity.Error, LogId.InvalidExpressionOperation, "Cannot take modulus with relative symbols.");
            return false;
        }

        result = new Address(left.Value % right.Value, Section.None);
        return true;
    }
    
    /// <inheritdoc/>
    public bool TryAnd(Address left, Address right, out Address result)
    {
        result = default;

        // Cannot AND relocatable addressing
        if (left.IsRelocatable || right.IsRelocatable)
        {
            _logger?.Log(Severity.Error, LogId.InvalidExpressionOperation, "Cannot perform logical AND with relative symbols.");
            return false;
        }

        result = new Address(left.Value & right.Value, Section.None);
        return true;
    }
    
    /// <inheritdoc/>
    public bool TryOr(Address left, Address right, out Address result)
    {
        result = default;

        // Cannot OR relocatable addressing
        if (left.IsRelocatable || right.IsRelocatable)
        {
            _logger?.Log(Severity.Error, LogId.InvalidExpressionOperation, "Cannot perform logical OR with relative symbols.");
            return false;
        }

        result = new Address(left.Value | right.Value, Section.None);
        return true;
    }
    
    /// <inheritdoc/>
    public bool TryXor(Address left, Address right, out Address result)
    {
        result = default;

        // Cannot XOR relocatable addressing
        if (left.IsRelocatable || right.IsRelocatable)
        {
            _logger?.Log(Severity.Error, LogId.InvalidExpressionOperation, "Cannot perform logical XOR with relative symbols.");
            return false;
        }

        result = new Address(left.Value ^ right.Value, Section.None);
        return true;
    }
}
