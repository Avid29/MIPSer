// Adam Dernis 2024

using Zarem.Models.Addressing;

namespace Zarem.Assembler;

/// <summary>
/// An assembler.
/// </summary>
public abstract class Assembler
{
    /// <summary>
    /// Gets the current address.
    /// </summary>
    public abstract Address CurrentAddress { get; }
}
