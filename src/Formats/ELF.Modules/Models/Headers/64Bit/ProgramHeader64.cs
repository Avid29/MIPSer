// Adam Dernis 2025

using ELF.Modules.Models.Headers.Enums;
using ELF.Modules.Models.Headers.Interfaces;
using System.Runtime.InteropServices;

using ProgramType = ELF.Modules.Models.Headers.Enums.ProgramType;

namespace ELF.Modules.Models.Headers;

/// <summary>
/// A struct containing the program header info for the ELF format in 64-bit.
/// </summary>
[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct ProgramHeader64 : IProgramHeader<ulong>
{
    private ProgramType _type;
    private Permissions _permissions;     // 64-bit perms/flags position
    private ulong _offset;
    private ulong _vAddress;
    private ulong _pAddress;
    private ulong _sizeInFile;
    private ulong _sizeInMemory;
    private ulong _alignment;

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
    public ulong Offset
    {
        readonly get => _offset;
        set => _offset = value;
    }

    /// <summary>
    /// Gets the virtual address where the segment is loaded in memory.
    /// </summary>
    public ulong VirtualAddress
    {
        readonly get => _vAddress;
        set => _vAddress = value;
    }
    
    /// <summary>
    /// Gets the physical address of the segment (if applicable).
    /// </summary>
    public ulong PhysicalAddress
    {
        readonly get => _pAddress;
        set => _pAddress = value;
    }
    
    /// <summary>
    /// Gets the size of the segment in the file.
    /// </summary>
    public ulong SizeInFile
    {
        readonly get => _sizeInFile;
        set => _sizeInFile = value;
    }
    
    /// <summary>
    /// Gets the size of the segment in memory.
    /// </summary>
    public ulong SizeInMemory
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
    public ulong Alignment
    {
        readonly get => _alignment;
        set => _alignment = value;
    }
}
