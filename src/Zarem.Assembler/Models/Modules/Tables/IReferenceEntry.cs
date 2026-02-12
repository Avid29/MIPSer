// Adam Dernis 2024

using Zarem.Models.Modules.Tables;

namespace Zarem.Assembler.Models.Modules.Tables;

/// <summary>
/// An interface for a reference entry implementation
/// </summary>
public interface IReferenceEntry<TSelf>
    where TSelf : IReferenceEntry<TSelf>
{
    /// <summary>
    /// Converts a <see cref="ReferenceEntry"/> into the formatted <typeparamref name="TSelf"/>.
    /// </summary>
    /// <param name="entry">The original entry class.</param>
    /// <returns>A converted relation entry as <typeparamref name="TSelf"/>.</returns>
    public static abstract TSelf Convert(ReferenceEntry entry);

    /// <summary>
    /// Converts a <typeparamref name="TSelf"/> into a <see cref="ReferenceEntry"/>.
    /// </summary>
    /// <returns>A converted relation entry as <see cref="ReferenceEntry"/>.</returns>
    public ReferenceEntry Convert(string name);
}
