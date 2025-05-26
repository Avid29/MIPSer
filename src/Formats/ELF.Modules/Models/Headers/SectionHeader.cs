// Adam Dernis 2025

using ELF.Modules.Models.Headers.Enums;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace ELF.Modules.Models.Headers;

/// <summary>
/// A struct containing the section header info for the ELF format.
/// </summary>
[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct SectionHeader<TAddress, TFlags>
    where TAddress : unmanaged, IBinaryInteger<TAddress>, IUnsignedNumber<TAddress>
    where TFlags : unmanaged, IBinaryInteger<TFlags>, IUnsignedNumber<TFlags>
{
    private uint _name;
    private SectionType _type;
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
    public SectionType Type
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
    public TAddress VirtualAddress
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

    /// <summary>
    /// Attempts to read a section header from a <see cref="Stream"/>.
    /// </summary>
    /// <param name="stream">The stream of the module.</param>
    /// <param name="header">The header of the module.</param>
    /// <param name="littleEndian">If the module is expected to be little endian.</param>
    /// <returns><see cref="true"/> if header was successfully read. <see cref="false"/> otherwise.</returns>
    public static bool TryRead(Stream stream, out SectionHeader<TAddress, TFlags> header, bool littleEndian = false)
    {
        header = default;

        // Pre-declare explicit header components
        Unsafe.SkipInit(out uint name);
        Unsafe.SkipInit(out uint type);
        Unsafe.SkipInit(out TFlags flags);
        Unsafe.SkipInit(out TAddress vAddress);
        Unsafe.SkipInit(out TAddress offset);
        Unsafe.SkipInit(out TAddress size);
        Unsafe.SkipInit(out uint link);
        Unsafe.SkipInit(out uint info);
        Unsafe.SkipInit(out TAddress alignment);
        Unsafe.SkipInit(out TAddress entrySize);

        // Try reading explict header components
        bool success = stream.TryRead(out name, littleEndian) &&
                       stream.TryRead(out type, littleEndian) &&
                       stream.TryRead(out flags, littleEndian) &&
                       stream.TryRead(out vAddress, littleEndian) &&
                       stream.TryRead(out offset, littleEndian) &&
                       stream.TryRead(out size, littleEndian) &&
                       stream.TryRead(out link, littleEndian) &&
                       stream.TryRead(out info, littleEndian) &&
                       stream.TryRead(out alignment, littleEndian) &&
                       stream.TryRead(out entrySize, littleEndian);
        
        // Return if component reading failed
        if (!success)
            return false;

        // Assign explicit header components
        header.Name = name;
        header.Type = (SectionType)type;
        header.Flags = flags;
        header.VirtualAddress = vAddress;
        header.Offset = offset;
        header.Size = size;
        header.Link= link;
        header.Info = info;
        header.Alignment = alignment;
        header.EntrySize = entrySize;

        return true;
    }

    /// <summary>
    /// Attempts to write the section header to a stream.
    /// </summary>
    /// <param name="stream">The stream to write the header on.</param>
    /// <param name="littleEndian">If the module is to be written in little endian.</param>
    /// <returns><see cref="true"/> if header was successfully written. <see cref="false"/> otherwise.</returns>
    public readonly bool TryWrite(Stream stream, bool littleEndian = false)
    {
        // Try write explicit header components
        bool success = stream.TryWrite(Name, littleEndian) &&
                       stream.TryWrite((uint)Type, littleEndian) &&
                       stream.TryWrite(Flags, littleEndian) &&
                       stream.TryWrite(VirtualAddress, littleEndian) &&
                       stream.TryWrite(Offset, littleEndian) &&
                       stream.TryWrite(Size, littleEndian) &&
                       stream.TryWrite(Link, littleEndian) &&
                       stream.TryWrite(Info, littleEndian) &&
                       stream.TryWrite(Alignment, littleEndian) &&
                       stream.TryWrite(EntrySize, littleEndian);

        // Return if component writing failed
        if (!success)
            return false;

        stream.Flush();
        return true;
    }
}
