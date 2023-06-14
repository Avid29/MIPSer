// Adam Dernis 2023

using MIPS.Models.Addressing.Enums;

namespace MIPS.Models.Addressing;

/// <summary>
/// A struct containing an address and the segment it belongs to.
/// </summary>
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
    /// Gets whether or not the address is relative to a section.
    /// </summary>
    public bool IsRelative => Section is not Section.None;

    /// <inheritdoc/>
    public override string ToString() => $"{Value}";
}
