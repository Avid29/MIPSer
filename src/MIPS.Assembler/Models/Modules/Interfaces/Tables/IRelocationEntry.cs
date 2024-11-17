// Adam Dernis 2024

using MIPS.Models.Modules.Tables;

namespace MIPS.Assembler.Models.Modules.Interfaces.Tables;

/// <summary>
/// An interface for a relocation entry implementation
/// </summary>
public interface IRelocationEntry<TSelf>
    where TSelf : IRelocationEntry<TSelf>
{
    /// <summary>
    /// Converts a <see cref="RelocationEntry"/> into the formatted <typeparamref name="TSelf"/>.
    /// </summary>
    /// <param name="entry">The original entry class.</param>
    /// <returns>A converted relation entry as <typeparamref name="TSelf"/>.</returns>
    public static abstract TSelf Convert(RelocationEntry entry);
}
