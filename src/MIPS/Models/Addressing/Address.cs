// Adam Dernis 2024

using MIPS.Models.Addressing.Enums;
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
    public Address(long value, Section section)
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
    public Section Section { get; set; }

    /// <summary>
    /// Get whether or not the address is fixed and not subject to relocation.
    /// </summary>
    public readonly bool IsFixed => Section is Section.None or Section.Absolute;

    /// <summary>
    /// Gets whether or not the address is relocating.
    /// </summary>
    /// <remarks>
    /// External addresses are not counted as relocatable.
    /// </remarks>
    public readonly bool IsRelocatable => !(IsFixed || IsExternal);
    
    /// <summary>
    /// Gets whether or not the address is external.
    /// </summary>
    public readonly bool IsExternal => Section is Section.External;

    /// <summary>
    /// Gets the default external address.
    /// </summary>
    public static Address External => new(0, Section.External);

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

        var section = left.IsFixed ? right.Section : left.Section;
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
            result = new Address(value, Section.Absolute);
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
