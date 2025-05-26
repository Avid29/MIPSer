// Adam Dernis 2025

using ELF.Modules.Models.Headers.Enums;
using ELF.Modules.Models.Headers.Interfaces;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace ELF.Modules.Models.Headers;

/// <summary>
/// A struct containing the program header info for the ELF format in 32-bit.
/// </summary>
[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct ProgramHeader32 : IProgramHeader<uint>
{
    private ProgramType _type;
    private uint _offset;
    private uint _vAddress;
    private uint _pAddress;
    private uint _sizeInFile;
    private uint _sizeInMemory;
    private Permissions _permissions;    // 32-bit perms/flags position
    private uint _alignment;

    /// <summary>
    /// Gets the type of the program header.
    /// </summary>
    public ProgramType Type
    {
        readonly get => _type;
        set => _type = value;
    }

    /// <summary>
    /// Gets the offset of the segment in the file image.
    /// </summary>
    public uint Offset
    {
        readonly get => _offset;
        set => _offset = value;
    }

    /// <summary>
    /// Gets the virtual address where the segment is loaded in memory.
    /// </summary>
    public uint VirtualAddress
    {
        readonly get => _vAddress;
        set => _vAddress = value;
    }
    
    /// <summary>
    /// Gets the physical address of the segment (if applicable).
    /// </summary>
    public uint PhysicalAddress
    {
        readonly get => _pAddress;
        set => _pAddress = value;
    }
    
    /// <summary>
    /// Gets the size of the segment in the file.
    /// </summary>
    public uint SizeInFile
    {
        readonly get => _sizeInFile;
        set => _sizeInFile = value;
    }
    
    /// <summary>
    /// Gets the size of the segment in memory.
    /// </summary>
    public uint SizeInMemory
    {
        readonly get => _sizeInMemory;
        set => _sizeInMemory = value;
    }
    
    /// <summary>
    /// Gets the permissions for the section.
    /// </summary>
    /// <remarks>
    /// Position in struct is for 32-bit only.
    /// </remarks>
    public Permissions Permissions
    {
        readonly get => _permissions;
        set => _permissions = value;
    }
    
    /// <summary>
    /// Gets the alignment of the segment.
    /// </summary>
    public uint Alignment
    {
        readonly get => _alignment;
        set => _alignment = value;
    }

    /// <summary>
    /// Attempts to read a section header from a <see cref="Stream"/>.
    /// </summary>
    /// <param name="stream">The stream of the module.</param>
    /// <param name="header">The header of the module.</param>
    /// <param name="littleEndian">If the module is expected to be little endian.</param>
    /// <returns><see cref="true"/> if header was successfully read. <see cref="false"/> otherwise.</returns>
    public static bool TryRead(Stream stream, out ProgramHeader32 header, bool littleEndian = false)
    {
        header = default;

        // Pre-declare explicit header components
        Unsafe.SkipInit(out uint type);
        Unsafe.SkipInit(out uint offset);
        Unsafe.SkipInit(out uint vAddress);
        Unsafe.SkipInit(out uint pAddress);
        Unsafe.SkipInit(out uint sizeInFile);
        Unsafe.SkipInit(out uint sizeInMemory);
        Unsafe.SkipInit(out uint permissions);
        Unsafe.SkipInit(out uint alignment);

        // Try reading explict header components
        bool success = stream.TryRead(out type, littleEndian) &&
                       stream.TryRead(out offset, littleEndian) &&
                       stream.TryRead(out vAddress, littleEndian) &&
                       stream.TryRead(out pAddress, littleEndian) &&
                       stream.TryRead(out sizeInFile, littleEndian) &&
                       stream.TryRead(out sizeInMemory, littleEndian) &&
                       stream.TryRead(out permissions, littleEndian) &&
                       stream.TryRead(out alignment, littleEndian);
        
        // Return if component reading failed
        if (!success)
            return false;

        // Assign explicit header components
        header.Type = (ProgramType)type;
        header.Offset = offset;
        header.VirtualAddress = vAddress;
        header.PhysicalAddress = pAddress;
        header.SizeInFile = sizeInFile;
        header.SizeInMemory = sizeInMemory;
        header.Permissions = (Permissions)permissions;
        header.Alignment = alignment;

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
        bool success = stream.TryWrite((uint)Type, littleEndian) &&
                       stream.TryWrite(Offset, littleEndian) &&
                       stream.TryWrite(VirtualAddress, littleEndian) &&
                       stream.TryWrite(PhysicalAddress, littleEndian) &&
                       stream.TryWrite(SizeInFile, littleEndian) &&
                       stream.TryWrite(SizeInMemory, littleEndian) &&
                       stream.TryWrite((uint)Permissions, littleEndian) &&
                       stream.TryWrite(Alignment, littleEndian);
        

        // Return if component writing failed
        if (!success)
            return false;

        stream.Flush();
        return true;
    }
}
