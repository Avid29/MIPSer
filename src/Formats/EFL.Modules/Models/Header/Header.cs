// Adam Dernis 2025

using System.Numerics;
using System.Runtime.InteropServices;

using Machine = EFL.Modules.Models.Header.Enums.Machine;
using Type = EFL.Modules.Models.Header.Enums.Type;

namespace EFL.Modules.Models.Header;

/// <summary>
/// A struct containing the module header info for the ELF format.
/// </summary>
/// <remarks>
/// Abstracts the type for addresses. uint if 32bit, ulong if 64bit. 
/// </remarks>
/// <typeparam name="TAddress">The type to use for addresses.</typeparam>
[StructLayout(LayoutKind.Sequential)]
public struct Header<TAddress>
    where TAddress : unmanaged, IBinaryInteger<TAddress>, IUnsignedNumber<TAddress>
{
    private HeaderIdentity _identity;
    private Type _type;
    private Machine _machine;
    private uint _version; // Should be 1
    private TAddress _entry;
    private TAddress _programHeaderOffset;
    private TAddress _sectionHeaderOffset;
    private uint _flags;
    private short _headerSize;
    private short _programHeaderEntrySize;
    private short _programHeaderCount;
    private short _sectionHeaderEntrySize;
    private short _sectionHeaderCount;
    private short _nameSectionIndex;
    
    /// <summary>
    /// Gets the header's identity info.
    /// </summary>
    public HeaderIdentity Identity
    {
        readonly get => _identity;
        internal set => _identity = value;
    }
    
    /// <summary>
    /// Gets the header's identity info.
    /// </summary>
    public Type Type
    {
        readonly get => _type;
        internal set => _type = value;
    }
    
    /// <summary>
    /// Gets the machine type.
    /// </summary>
    public Machine Machine
    {
        readonly get => _machine;
        internal set => _machine = value;
    }
    
    /// <summary>
    /// Gets the version, which should always be 1.
    /// </summary>
    public uint Version
    {
        readonly get => _version;
        internal set => _version = value;
    }
    
    /// <summary>
    /// Gets the entry point address.
    /// </summary>
    public TAddress Entry
    {
        readonly get => _entry;
        internal set => _entry = value;
    }
    
    /// <summary>
    /// Gets the offset from the start of the file to the start of the program header table.
    /// </summary>
    public TAddress ProgramHeaderTableOffset
    {
        readonly get => _programHeaderOffset;
        internal set => _programHeaderOffset = value;
    }
    
    /// <summary>
    /// Gets the offset from the start of the file to the start of the section header table.
    /// </summary>
    public TAddress SectionHeaderTableOffset
    {
        readonly get => _sectionHeaderOffset;
        internal set => _sectionHeaderOffset = value;
    }
    
    /// <summary>
    /// Gets the flags associated with the header.
    /// </summary>
    public uint Flags
    {
        readonly get => _flags;
        internal set => _flags = value;
    }
    
    /// <summary>
    /// Gets the header size.
    /// </summary>
    public short HeaderSize
    {
        readonly get => _headerSize;
        internal set => _headerSize = value;
    }
    
    /// <summary>
    /// Gets the size of each entry in the program header table.
    /// </summary>
    public short ProgramHeaderEntrySize
    {
        readonly get => _programHeaderEntrySize;
        internal set => _programHeaderEntrySize = value;
    }
    
    /// <summary>
    /// Gets the number of program header entries.
    /// </summary>
    public short ProgramHeaderCount
    {
        readonly get => _programHeaderCount;
        internal set => _programHeaderCount = value;
    }
    
    /// <summary>
    /// Gets the size of each entry in the section header table.
    /// </summary>
    public short SectionHeaderEntrySize
    {
        readonly get => _sectionHeaderEntrySize;
        internal set => _sectionHeaderEntrySize = value;
    }
    
    /// <summary>
    /// Gets the number of section header entries.
    /// </summary>
    public short SectionHeaderCount
    {
        readonly get => _sectionHeaderCount;
        internal set => _sectionHeaderCount = value;
    }
    
    /// <summary>
    /// Gets the index of the section in the section header table containing section names.
    /// </summary>
    public short NameSectionIndex
    {
        readonly get => _nameSectionIndex;
        internal set => _nameSectionIndex = value;
    }
}
