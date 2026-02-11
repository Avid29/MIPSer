// Adam Dernis 2024

using Zarem.MIPS.Models.Modules.Tables;

namespace Zarem.Assembler.MIPS.Models.Modules.Interfaces.Tables;

/// <summary>
/// An interface for a symbol entry implementation
/// </summary>
public interface ISymbolEntry<TSelf>
    where TSelf : ISymbolEntry<TSelf>
{
    /// <summary>
    /// Converts a <see cref="SymbolEntry"/> into the formatted <typeparamref name="TSelf"/>.
    /// </summary>
    /// <param name="entry">The original entry class.</param>
    /// <returns>A converted relation entry as <typeparamref name="TSelf"/>.</returns>
    public static abstract TSelf Convert(SymbolEntry entry);

    /// <summary>
    /// Converts a <typeparamref name="TSelf"/> into a <see cref="SymbolEntry"/>.
    /// </summary>
    /// <returns>A converted relation entry as <see cref="SymbolEntry"/>.</returns>
    public SymbolEntry Convert(string name);
}
