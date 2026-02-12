// Adam Dernis 2024

using Zarem.Models.Addressing;

namespace Zarem.Assembler;

/// <summary>
/// An assembler.
/// </summary>
public interface IAssembler
{
    /// <summary>
    /// Gets the current address.
    /// </summary>
    public Address CurrentAddress { get; }
}
