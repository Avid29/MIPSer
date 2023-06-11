// Adam Dernis 2023

using MIPS.Models.Addressing.Enums;

namespace MIPS.Models.Addressing;

/// <summary>
/// A struct containing an address and the segment it belongs to.
/// </summary>
public struct SegmentAddress
{
    /// <summary>
    /// Initializes a new instance of the <see cref="SegmentAddress"/> struct.
    /// </summary>
    public SegmentAddress(long address, Segment segment)
    {
        Address = address;
        Segment = segment;
    }

    /// <summary>
    /// The address within the segment.
    /// </summary>
    public long Address { get; set; }

    /// <summary>
    /// The segment the address belongs to.
    /// </summary>
    public Segment Segment { get; set; }
}
