// Adam Dernis 2025

using System.Numerics;
using System.Runtime.InteropServices;
using Type = ELF.Modules.Models.Section.Enums.Type;

namespace ELF.Modules.Models.Section;

/// <summary>
/// A struct containing the section header info for the ELF format.
/// </summary>
[StructLayout(LayoutKind.Sequential)]
public struct SectionHeader<TAddress, TFlags>
    where TAddress : unmanaged, IBinaryInteger<TAddress>, IUnsignedNumber<TAddress>
    where TFlags : Enum
{
    private uint _name;
    private Type _type;
    private TFlags _flags;
    private TAddress _vAddress;
    private TAddress _offset;
    private TAddress _size;
    private uint _link;
    private uint _info;
    private TAddress _alignment;
    private TAddress _entrySize;

    /// <summary>
    /// Gets the name of the section (as an index into the string table).
    /// </summary>
    public uint Name
    {
        readonly get => _name;
        internal set => _name = value;
    }
    
    /// <summary>
    /// Gets the type of the section.
    /// </summary>
    public Type Type
    {
        readonly get => _type;
        internal set => _type = value;
    }
    
    /// <summary>
    /// Gets the flags associated with the section.
    /// </summary>
    public TFlags Flags
    {
        readonly get => _flags;
        internal set => _flags = value;
    }
    
    /// <summary>
    /// Gets the virtual address of the section in memory for sections that are loaded.
    /// </summary>
    public TAddress VAddress
    {
        readonly get => _vAddress;
        internal set => _vAddress = value;
    }
    
    /// <summary>
    /// Gets the offset of the section in the file.
    /// </summary>
    public TAddress Offset
    {
        readonly get => _offset;
        internal set => _offset = value;
    }
    
    /// <summary>
    /// Gets the size of the section in the file.
    /// </summary>
    public TAddress Size
    {
        readonly get => _size;
        internal set => _size = value;
    }
    
    /// <summary>
    /// Gets the index of a related section (e.g., a string table or symbol table).
    /// </summary>
    public uint Link
    {
        readonly get => _link;
        internal set => _link = value;
    }
    
    /// <summary>
    /// Gets extra information about the section.
    /// </summary>
    public uint Info
    {
        readonly get => _info;
        internal set => _info = value;
    }
    
    /// <summary>
    /// Gets the alignment of the section.
    /// </summary>
    public TAddress Alignment
    {
        readonly get => _alignment;
        internal set => _alignment = value;
    }
    
    /// <summary>
    /// Gets the size of each entry in the section, if it contains fixed-size entries.
    /// 0 otherwise.
    /// </summary>
    public TAddress EntrySize
    {
        readonly get => _entrySize;
        internal set => _entrySize = value;
    }
}
