// Adam Dernis 2025

using ELF.Modules.Models.Program.Enums;
using System.Runtime.InteropServices;

using Type = ELF.Modules.Models.Program.Enums.Type;

namespace ELF.Modules.Models.Program;

/// <summary>
/// A struct containing the program header info for the ELF format in 32-bit.
/// </summary>
[StructLayout(LayoutKind.Sequential)]
public struct ProgramHeader32 : IProgramHeader<uint>
{
    private Type _type;
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
    public Type Type
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
}
