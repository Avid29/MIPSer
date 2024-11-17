// Adam Dernis 2024

using MIPS.Models.Modules.Tables;

namespace MIPS.Assembler.Models.Modules.Interfaces.Tables;

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
}
