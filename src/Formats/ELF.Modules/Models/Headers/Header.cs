// Adam Dernis 2025

using ELF.Modules.Models.Headers.Enums;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Machine = ELF.Modules.Models.Headers.Enums.Machine;
using Type = ELF.Modules.Models.Headers.Enums.Type;

namespace ELF.Modules.Models.Headers;

/// <summary>
/// A struct containing the module header info for the ELF format.
/// </summary>
/// <remarks>
/// Abstracts the type for addresses. uint if 32bit, ulong if 64bit. 
/// </remarks>
/// <typeparam name="TAddress">The type to use for addresses.</typeparam>
[StructLayout(LayoutKind.Sequential, Pack = 1)]
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
    private ushort _headerSize;
    private ushort _programHeaderEntrySize;
    private ushort _programHeaderCount;
    private ushort _sectionHeaderEntrySize;
    private ushort _sectionHeaderCount;
    private ushort _nameSectionIndex;
    
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
    public ushort HeaderSize
    {
        readonly get => _headerSize;
        internal set => _headerSize = value;
    }
    
    /// <summary>
    /// Gets the size of each entry in the program header table.
    /// </summary>
    public ushort ProgramHeaderEntrySize
    {
        readonly get => _programHeaderEntrySize;
        internal set => _programHeaderEntrySize = value;
    }
    
    /// <summary>
    /// Gets the number of program header entries.
    /// </summary>
    public ushort ProgramHeaderCount
    {
        readonly get => _programHeaderCount;
        internal set => _programHeaderCount = value;
    }
    
    /// <summary>
    /// Gets the size of each entry in the section header table.
    /// </summary>
    public ushort SectionHeaderEntrySize
    {
        readonly get => _sectionHeaderEntrySize;
        internal set => _sectionHeaderEntrySize = value;
    }
    
    /// <summary>
    /// Gets the number of section header entries.
    /// </summary>
    public ushort SectionHeaderCount
    {
        readonly get => _sectionHeaderCount;
        internal set => _sectionHeaderCount = value;
    }
    
    /// <summary>
    /// Gets the index of the section in the section header table containing section names.
    /// </summary>
    public ushort NameSectionIndex
    {
        readonly get => _nameSectionIndex;
        internal set => _nameSectionIndex = value;
    }
    
    /// <summary>
    /// Attempts to read a header from a <see cref="Stream"/>.
    /// </summary>
    /// <param name="stream">The stream of the module.</param>
    /// <param name="header">The header of the module.</param>
    /// <param name="littleEndian">If the module is expected to be little endian.</param>
    /// <returns><see cref="true"/> if header was successfully read. <see cref="false"/> otherwise.</returns>
    public static bool TryRead(Stream stream, out Header<TAddress> header, bool littleEndian = false)
    {
        header = default;

        // Pre-declare explicit header components
        Unsafe.SkipInit(out HeaderIdentity identity);
        Unsafe.SkipInit(out ushort type);
        Unsafe.SkipInit(out ushort machine);
        Unsafe.SkipInit(out uint version);
        Unsafe.SkipInit(out TAddress entry);
        Unsafe.SkipInit(out TAddress progHeadOffset);
        Unsafe.SkipInit(out TAddress sectHeadOffset);
        Unsafe.SkipInit(out uint flags);
        Unsafe.SkipInit(out ushort headerSize);
        Unsafe.SkipInit(out ushort progHeadEntrySize);
        Unsafe.SkipInit(out ushort progHeadCount);
        Unsafe.SkipInit(out ushort sectHeadEntrySize);
        Unsafe.SkipInit(out ushort sectHeadCount);
        Unsafe.SkipInit(out ushort nameSectionIndex);

        // Try reading explict header components
        bool success = HeaderIdentity.TryRead(stream, out identity, littleEndian) && 
                       stream.TryRead(out type, littleEndian) &&
                       stream.TryRead(out machine, littleEndian) &&
                       stream.TryRead(out version, littleEndian) &&
                       stream.TryRead(out entry, littleEndian) &&
                       stream.TryRead(out progHeadOffset, littleEndian) &&
                       stream.TryRead(out sectHeadOffset, littleEndian) &&
                       stream.TryRead(out flags, littleEndian) &&
                       stream.TryRead(out headerSize, littleEndian) &&
                       stream.TryRead(out progHeadEntrySize, littleEndian) &&
                       stream.TryRead(out progHeadCount, littleEndian) &&
                       stream.TryRead(out sectHeadEntrySize, littleEndian) &&
                       stream.TryRead(out sectHeadCount, littleEndian) &&
                       stream.TryRead(out nameSectionIndex, littleEndian);
        
        // Return if component reading failed
        if (!success)
            return false;

        // Assign explicit header components
        header.Identity = identity;
        header.Type = (Type)type;
        header.Machine = (Machine)machine;
        header.Version = version;
        header.Entry = entry;
        header.ProgramHeaderTableOffset = progHeadOffset;
        header.SectionHeaderTableOffset = sectHeadOffset;
        header.Flags = flags;
        header.HeaderSize = headerSize;
        header.ProgramHeaderEntrySize = progHeadEntrySize;
        header.ProgramHeaderCount = progHeadCount;
        header.SectionHeaderEntrySize = sectHeadEntrySize;
        header.SectionHeaderCount = sectHeadCount;
        header.NameSectionIndex = nameSectionIndex;

        return true;
    }

    /// <summary>
    /// Writes the header to a stream.
    /// </summary>
    /// <param name="stream">The stream to write the header on.</param>
    /// <returns><see cref="true"/> if header was successfully written. <see cref="false"/> otherwise.</returns>
    public readonly bool TryWrite(Stream stream)
    {
        bool littleEndian = Identity.Data is Data.LittleEndian;

        // Try write explicit header components
        bool success = Identity.TryWrite(stream) &&
                       stream.TryWrite((ushort)Type, littleEndian) &&
                       stream.TryWrite((ushort)Machine, littleEndian) && 
                       stream.TryWrite(Version, littleEndian) && 
                       stream.TryWrite(Entry, littleEndian) && 
                       stream.TryWrite(ProgramHeaderTableOffset, littleEndian) && 
                       stream.TryWrite(SectionHeaderTableOffset, littleEndian) && 
                       stream.TryWrite(Flags, littleEndian) && 
                       stream.TryWrite(HeaderSize, littleEndian) && 
                       stream.TryWrite(ProgramHeaderEntrySize, littleEndian) && 
                       stream.TryWrite(ProgramHeaderCount, littleEndian) && 
                       stream.TryWrite(SectionHeaderEntrySize, littleEndian) && 
                       stream.TryWrite(SectionHeaderCount, littleEndian) && 
                       stream.TryWrite(NameSectionIndex, littleEndian);

        // Return if component writing failed
        if (!success)
            return false;

        stream.Flush();
        return true;
    }
}
