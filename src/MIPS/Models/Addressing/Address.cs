// Adam Dernis 2024

using System.Diagnostics;

namespace MIPS.Models.Addressing;

/// <summary>
/// A struct containing an address and the section it belongs to.
/// </summary>
[DebuggerDisplay("{Section}: {Value}")]
public struct Address
{
    /// <summary>
    /// Initializes a new instance of the <see cref="Address"/> struct.
    /// </summary>
    public Address(long value, string? section = null)
    {
        Value = value;
        Section = section;
    }

    /// <summary>
    /// Gets the value of the address within the section.
    /// </summary>
    public long Value { get; set; }

    /// <summary>
    /// Gets the section the address belongs to.
    /// </summary>
    public string? Section { get; set; }

    /// <summary>
    /// Gets whether or not the address is locatable.
    /// </summary>
    public readonly bool IsRelocatable => Section is not null;

    /// <summary>
    /// Attempts to add two <see cref="Address"/> structs.
    /// </summary>
    /// <param name="left">The left operand.</param>
    /// <param name="right">The right operand.</param>
    /// <param name="result">The resulting address.</param>
    /// <returns>Whether or not the addresses could be added.</returns>
    public static bool TryAdd(Address left, Address right, out Address result)
    {
        result = default;

        if (left.IsRelocatable && right.IsRelocatable)
            return false;

        var section = left.Section ?? right.Section;
        result = new Address(left.Value + right.Value, section);
        return true;
    }

    /// <summary>
    /// Attempts to subtract an <see cref="Address"/> from another.
    /// </summary>
    /// <param name="left">The left operand.</param>
    /// <param name="right">The right operand.</param>
    /// <param name="result">The resulting address.</param>
    /// <returns>Whether or not the addresses could be added.</returns>
    public static bool TrySubtract(Address left, Address right, out Address result)
    {
        result = default;

        if (left.Section == right.Section)
        {
            var value = left.Value - right.Value;
            result = new Address(value, null);
            return true;
        }

        if (right.IsRelocatable)
            return false;


        result = new Address(left.Value - right.Value, left.Section);
        return true;
    }

    /// <inheritdoc/>
    public static Address operator +(Address address, long offset) => new(address.Value + offset, address.Section);
    
    /// <inheritdoc/>
    public static Address operator -(Address address, long offset) => new(address.Value - offset, address.Section);
}
