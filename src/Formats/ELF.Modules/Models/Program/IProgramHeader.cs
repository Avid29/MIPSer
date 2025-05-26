// Adam Dernis 2025

using ELF.Modules.Models.Program.Enums;
using System.Numerics;

using Type = ELF.Modules.Models.Program.Enums.Type;

namespace ELF.Modules.Models.Program;

/// <summary>
/// An interface for a program header in the ELF format.
/// </summary>
/// <remarks>
/// Abstracts the type for addresses. uint if 32bit, ulong if 64bit. 
/// </remarks>
/// <typeparam name="TAddress">The type to use for addresses.</typeparam>
public interface IProgramHeader<TAddress>
    where TAddress : unmanaged, IBinaryInteger<TAddress>, IUnsignedNumber<TAddress>
{
    /// <summary>
    /// Gets the type of the program header.
    /// </summary>
    public Type Type { get; internal set; }
    
    /// <summary>
    /// Gets the offset of the segment in the file image.
    /// </summary>
    public TAddress Offset { get; internal set; }

    /// <summary>
    /// Gets the virtual address where the segment is loaded in memory.
    /// </summary>
    public TAddress VirtualAddress { get; internal set; }
    
    /// <summary>
    /// Gets the physical address of the segment (if applicable).
    /// </summary>
    public TAddress PhysicalAddress { get; internal set; }
    
    /// <summary>
    /// Gets the size of the segment in the file.
    /// </summary>
    public TAddress SizeInFile { get; internal set; }
    
    /// <summary>
    /// Gets the size of the segment in memory.
    /// </summary>
    public TAddress SizeInMemory { get; internal set; }
    
    /// <summary>
    /// Gets the permissions for the segment.
    /// </summary>
    public Permissions Permissions { get; internal set; }
    
    /// <summary>
    /// Gets the alignment of the segment.
    /// </summary>
    public TAddress Alignment { get; internal set; }
}
