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
    /// Gets whether or not the address is relocating.
    /// </summary>
    public readonly bool IsRelocatable => Section is not Section.None;
    
    /// <inheritdoc/>
    public static Address operator +(Address address, long offset) => new(address.Value + offset, address.Section);
    
    /// <inheritdoc/>
    public static Address operator -(Address address, long offset) => new(address.Value - offset, address.Section);
}
