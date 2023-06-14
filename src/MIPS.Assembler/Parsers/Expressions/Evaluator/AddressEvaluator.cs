// Adam Dernis 2023

using MIPS.Assembler.Logging;
using MIPS.Models.Addressing;
using MIPS.Models.Addressing.Enums;

namespace MIPS.Assembler.Parsers.Expressions.Evaluator;

/// <summary>
/// An <see cref="IEvaluator{T}"/> for address expressions.
/// </summary>
public struct AddressEvaluator : IEvaluator<Address>
{
    /// <summary>
    /// Gets or sets the address evaluator.
    /// </summary>
    public ILogger Logger { get; set; }

    /// <inheritdoc/>
    public bool TryAdd(Address left, Address right, out Address result)
    {
        result = default;

        // If both address have a section they cannot be added
        if (left.IsRelative && right.IsRelative)
            return false;

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
        
        // TODO: Handle subtraction relativity
        if (left.IsRelative || right.IsRelative)
            return false;
        
        result = new Address(left.Value - right.Value, Section.None);
        return true;
    }
    
    /// <inheritdoc/>
    public bool TryMultiply(Address left, Address right, out Address result)
    {
        result = default;

        // Cannot multiply relative addressing 
        if (left.IsRelative || right.IsRelative)
            return false;

        result = new Address(left.Value * right.Value, Section.None);
        return true;
    }
    
    /// <inheritdoc/>
    public bool TryDivide(Address left, Address right, out Address result)
    {
        // Cannot divide relative addressing
        if (left.IsRelative || right.IsRelative)
        {
            result = default;
            return false;
        }

        result = new Address(left.Value / right.Value, Section.None);
        return true;
    }
    
    /// <inheritdoc/>
    public bool TryMod(Address left, Address right, out Address result)
    {
        // Cannot mod relative addressing
        if (left.IsRelative || right.IsRelative)
        {
            result = default;
            return false;
        }

        result = new Address(left.Value % right.Value, Section.None);
        return true;
    }
    
    /// <inheritdoc/>
    public bool TryAnd(Address left, Address right, out Address result)
    {
        // Cannot AND relative addressing
        if (left.IsRelative || right.IsRelative)
        {
            result = default;
            return false;
        }

        result = new Address(left.Value & right.Value, Section.None);
        return true;
    }
    
    /// <inheritdoc/>
    public bool TryOr(Address left, Address right, out Address result)
    {
        // Cannot OR relative addressing
        if (left.IsRelative || right.IsRelative)
        {
            result = default;
            return false;
        }

        result = new Address(left.Value | right.Value, Section.None);
        return true;
    }
    
    /// <inheritdoc/>
    public bool TryXor(Address left, Address right, out Address result)
    {
        // Cannot XOR relative addressing
        if (left.IsRelative || right.IsRelative)
        {
            result = default;
            return false;
        }

        result = new Address(left.Value ^ right.Value, Section.None);
        return true;
    }
}
