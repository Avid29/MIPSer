// Adam Dernis 2024

using MIPS.Models.Addressing;
using MIPS.Models.Modules.Tables;
using MIPS.Models.Modules.Tables.Enums;

namespace MIPS.Assembler.Models.Modules;

public partial class Module
{
    /// <summary>
    /// Adds a symbol to the symbol table.
    /// </summary>
    /// <param name="name">The name of the symbol.</param>
    /// <param name="type">The symbol's type.</param>
    /// <param name="value">The value of the symbol.</param>
    /// <returns><see langword="false"/> if the symbol already exists. <see langword="true"/> otherwise.</returns>
    public bool TryDefineSymbol(string name, SymbolType type, Address? value = null)
    {
        // Check if table already contains symbol
        if (_definitions.ContainsKey(name))
            return false;

        DefineSymbol(name, type, value);
        return true;
    }

    /// <summary>
    /// Adds or updates a symbol in the symbol table.
    /// </summary>
    /// <returns><see cref="false"/> if the symbol already has a value, and a new value is being defined.</returns>
    public bool TryDefineOrUpdateSymbol(string name, SymbolType? type = null, Address? value = null, SymbolBinding binding = SymbolBinding.Local)
    {
        if (_definitions.ContainsKey(name))
            return UpdateSymbol(name, type, value, binding);
            
        DefineSymbol(name, type ?? SymbolType.Unknown, value, binding);
        return true;
    }

    /// <summary>
    /// Attempts to get a symbol's value.
    /// </summary>
    /// <param name="name">The name of the symbol.</param>
    /// <param name="value">The realized value of the symbol.</param>
    /// <returns><see cref="true"/> if the symbol exists, <see cref="false"/> otherwise.</returns>
    public bool TryGetSymbol(string name, out SymbolEntry? value) => _definitions.TryGetValue(name, out value);

    /// <summary>
    /// Attempts to track a reference to a symbol.
    /// </summary>
    /// <param name="entry">The reference information.</param>
    /// <returns>Whether or not a symbol reference was made.</returns>
    public bool TryTrackReference(ReferenceEntry entry)
    {
        _references.Add(entry);
        return true;
    }

    private void DefineSymbol(string name, SymbolType type, Address? value = null, SymbolBinding binding = SymbolBinding.Local)
    {
        // Create entry
        var entry = new SymbolEntry(name, value, type, binding);

        if (!value.HasValue)
        {
            entry.ForwardDeclared = true;
        }

        _definitions.Add(name, entry);
    }

    private bool UpdateSymbol(string name, SymbolType? type = null, Address? value = null, SymbolBinding? binding = null)
    {
        var entry = _definitions[name];

        // Update address
        if (value.HasValue)
        {
            // Cannot update the address on an already defined symbol.
            if (entry.IsDefined)
                return false;

            entry.Address = value.Value;
        }

        // Update type
        if (type.HasValue)
        {
            // Cannot update the type of an already typed symbol.
            if (entry.Type is not SymbolType.Unknown)
                return false;

            entry.Type = type.Value;
        }
        
        // Update bindings
        if (binding.HasValue)
        {
            // Cannot update the binding if it is already global/weak
            if (entry.Binding is not SymbolBinding.Local)
                return false;

            entry.Binding = binding.Value;
        }

        _definitions[name] = entry;
        return true;
    }
}
