// Adam Dernis 2025

using ELF.Modules.Models.Enums;
using ELF.Modules.Models.Interfaces;

namespace ELF.Modules.Models;

/// <summary>
/// A struct containing the symbol entry info for the ELF format in 32-bit.
/// </summary>
public struct SymbolEntry32 : ISymbolEntry
{
    private uint _name;
    private uint _value;
    private uint _size;
    private char _info;
    private Visibility _visibility;
    private uint _sectionIndex;

    /// <summary>
    /// Gets the name of the symbol (as an index into the string table).
    /// </summary>
    public uint Name
    {
        readonly get => _name;
        internal set => _name = value;
    }
    
    /// <summary>
    /// Gets the value of the symbol.
    /// </summary>
    public uint Value
    {
        readonly get => _value;
        internal set => _value = value;
    }
    
    /// <summary>
    /// Gets the size of the symbol.
    /// </summary>
    public uint Size
    {
        readonly get => _size;
        internal set => _size = value;
    }
    
    /// <summary>
    /// Gets the symbol's type and binding information.
    /// </summary>
    public char Info
    {
        readonly get => _info;
        internal set => _info = value;
    }
    
    /// <summary>
    /// Gets the visibility of the symbol.
    /// </summary>
    public Visibility Visibility
    {
        readonly get => _visibility;
        internal set => _visibility = value;
    }
    
    /// <summary>
    /// Gets the index of the section associated with the symbol.
    /// </summary>
    public uint SectionIndex
    {
        readonly get => _sectionIndex;
        internal set => _sectionIndex = value;
    }
}
