// Adam Dernis 2024

using MIPS.Models.Addressing;
using MIPS.Models.Modules.Tables;

namespace MIPS.Assembler.Models.Modules;

public partial class ModuleConstruction
{
    /// <summary>
    /// Adds a symbol to the symbol table.
    /// </summary>
    /// <param name="name">The name of the symbol.</param>
    /// <param name="value">The value of the symbol.</param>
    /// <returns><see langword="false"/>if the symbol already exists. <see langword="true"/> otherwise.</returns>
    public bool TryDefineSymbol(string name, Address value)
    {
        // Check if table already contains symbol
        if (_definitions.ContainsKey(name))
            return false;

        _definitions.Add(name, value);
        return true;
    }

    /// <summary>
    /// Attempts to get a symbol's value.
    /// </summary>
    /// <param name="name">The name of the symbol.</param>
    /// <param name="value">The realized value of the symbol.</param>
    /// <returns><see cref="true"/> if the symbol exists, <see cref="false"/> otherwise.</returns>
    public bool TryGetSymbol(string name, out Address value) => _definitions.TryGetValue(name, out value);

    /// <summary>
    /// Attempts to make a reference to a symbol.
    /// </summary>
    /// <param name="location">The location the symbol is referenced.</param>
    /// <param name="symbol">The symbol referenced.</param>
    /// <returns>Whether or not a symbol reference was made.</returns>
    public bool TryMakeReference(Address location, string symbol)
    {
        // TODO: Track upper/lower
        _references.Add(location, symbol);
        return true;
    }

    /// <summary>
    /// Attempts to track a relocation entry.
    /// </summary>
    /// <param name="entry">The relocation information.</param>
    /// <returns>Whether or not a relocation entry was made.</returns>
    public bool TryTrackRelocation(RelocationEntry entry)
    {
        _relocations.Add(entry);
        return true;
    }
}
