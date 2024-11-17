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
    public readonly bool IsFixed => Section is Section.None;

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
    public static Address External => new Address(0, Section.External);

    /// <inheritdoc/>
    public static Address operator +(Address address, long offset) => new(address.Value + offset, address.Section);
    
    /// <inheritdoc/>
    public static Address operator -(Address address, long offset) => new(address.Value - offset, address.Section);
}
