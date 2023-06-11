// Adam Dernis 2023

using MIPS.Models.Addressing;

namespace MIPS.Assembler.Models.Construction;

public partial class ObjectModuleConstructor
{
    /// <summary>
    /// Adds a symbol to the symbol table.
    /// </summary>
    /// <param name="name">The name of the symbol.</param>
    /// <param name="value">The value of the symbol.</param>
    /// <returns><see langword="false"/>if the symbol already exists. <see langword="true"/> otherwise.</returns>
    public bool TryDefineSymbol(string name, SegmentAddress value)
    {
        // Check if table already contains symbol
        if (_symbols.ContainsKey(name))
        {
            return false;
        }

        _symbols.Add(name, value);
        return true;
    }
}
