// Adam Dernis 2023

using MIPS.Assembler.Models.Markers.Abstract;

namespace MIPS.Assembler.Models.Markers;

/// <summary>
/// A <see cref="Marker"/> for data allocations.
/// </summary>
public class DataMarker : Marker
{
    /// <summary>
    /// Initializes a new instance of the <see cref="DataMarker"/> class.
    /// </summary>
    public DataMarker(byte[] data)
    {
        Data = data;
    }

    /// <summary>
    /// Gets the data allocated by the marker.
    /// </summary>
    public byte[] Data { get; }
}
