// Adam Dernis 2023

using MIPS.Models.Addressing;

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
    /// Attempts to get a symbol's realized value, including segment offset.
    /// </summary>
    /// <param name="name">The name of the symbol.</param>
    /// <param name="value">The realized value of the symbol.</param>
    /// <returns><see cref="true"/> if the symbol exists, <see cref="false"/> otherwise.</returns>
    public bool TryGetSymbol(string name, out Address value)
    {
        if (!_definitions.ContainsKey(name))
        {
            value = default;
            return false;
        }

        value = _definitions[name];
        return true;
    }

    /// <summary>
    /// Attempts to make a reference to a symbol.
    /// </summary>
    /// <param name="location">The location the symbol is referenced.</param>
    /// <param name="symbol">The symbol referenced.</param>
    /// <returns>Whether or not a symbol reference was made.</returns>
    public bool TryMakeReference(Address location, string symbol)
    {
        // TODO:
        return false;
    }
}
