// Adam Dernis 2024

using System.IO;
using System.Runtime.CompilerServices;
using Zarem.Extensions.System.IO;
using Zarem.RASM.Tables;

namespace Zarem.RASM;

/// <summary>
/// A struct containing the module header info for the RASM format.
/// </summary>
public struct Header
{
    /// <summary>
    /// The size of the header in the module.
    /// </summary>
    public const int HEADER_SIZE = (sizeof(uint) * (10 + 2)) + (sizeof(ushort) * 2);

    private ushort _magic;
    private ushort _version;
    private uint _flags;
    private uint _entryPoint;
    private uint[] _sizes;

    /// <summary>
    /// Initializes a new instance of the <see cref="Header"/> struct.
    /// </summary>
    public Header(ushort magic, ushort version, params uint[] sizes)
    {
        _magic = magic;
        _version = version;
        _sizes = sizes;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Header"/> struct.
    /// </summary>
    public Header(ushort magic, ushort version, uint flags, uint entry, params uint[] sizes)
    {
        _magic = magic;
        _version = version;
        _flags = flags;
        _entryPoint = entry;
        _sizes = sizes;
    }

    /// <summary>
    /// Gets the module magic identifier.
    /// </summary>
    public ushort Magic
    {
        readonly get => _magic;
        internal set => _magic = value;
    }

    /// <summary>
    /// Gets the module format version number.
    /// </summary>
    public ushort Version
    {
        readonly get => _version;
        internal set => _version = value;
    }

    /// <summary>
    /// Gets the module flags.
    /// </summary>
    public uint Flags
    {
        readonly get => _flags;
        internal set => _flags = value;
    }

    /// <summary>
    /// Gets the module entry point.
    /// </summary>
    public uint EntryPoint
    {
        readonly get => _entryPoint;
        internal set => _entryPoint = value;
    }

    /// <summary>
    /// Gets the size of the module's text section.
    /// </summary>
    public readonly uint TextSize
    {
        get => _sizes[0];
        internal set => _sizes[0] = value;
    }

    /// <summary>
    /// Gets the size of the module's rdata section.
    /// </summary>
    public readonly uint ReadOnlyDataSize
    {
        get => _sizes[1];
        internal set => _sizes[1] = value;
    }

    /// <summary>
    /// Gets the size of the module's data section.
    /// </summary>
    public readonly uint DataSize
    {
        get => _sizes[2];
        internal set => _sizes[2] = value;
    }

    /// <summary>
    /// Gets the size of the module's small initialized data section.
    /// </summary>
    public readonly uint SmallDataSize
    {
        get => _sizes[3];
        internal set => _sizes[3] = value;
    }

    /// <summary>
    /// Gets the size of the module's small initialized data section.
    /// </summary>
    public readonly uint SmallUninitializedDataSize
    {
        get => _sizes[4];
        internal set => _sizes[4] = value;
    }

    /// <summary>
    /// Gets the size of the module's small initialized data section.
    /// </summary>
    public readonly uint UninitializedDataSize
    {
        get => _sizes[5];
        internal set => _sizes[5] = value;
    }

    /// <summary>
    /// Gets the number of entries in the module's relocation table.
    /// </summary>
    public readonly uint RelocationTableCount
    {
        get => _sizes[6];
        internal set => _sizes[6] = value;
    }

    /// <summary>
    /// Gets the number of entries the module's reference table.
    /// </summary>
    public readonly uint ReferenceTableCount
    {
        get => _sizes[7];
        internal set => _sizes[7] = value;
    }

    /// <summary>
    /// Gets the number of entries in the defintions table.
    /// </summary>
    public readonly uint DefinitionsTableCount
    {
        get => _sizes[8];
        internal set => _sizes[8] = value;
    }

    /// <summary>
    /// Gets the size of the string table.
    /// </summary>
    public readonly uint StringTableSize
    {
        get => _sizes[9];
        internal set => _sizes[9] = value;
    }

    /// <summary>
    /// Gets the expected module size.
    /// </summary>
    public readonly unsafe long ExpectedModuleSize
    {
        get
        {
            // Begin with the size of the header
            long sum = HEADER_SIZE;
            
            // Add sizes of each section
            sum += TextSize + ReadOnlyDataSize;
            sum += DataSize + SmallDataSize;
            sum += SmallUninitializedDataSize + UninitializedDataSize;
            sum += StringTableSize;

            // Add table counts * entry size
            sum += RelocationTableCount * sizeof(RelocationEntry);
            sum += ReferenceTableCount * sizeof(ReferenceEntry);
            sum += DefinitionsTableCount * sizeof(SymbolEntry);
            return sum;
        }
    }

    /// <summary>
    /// Attempts to read a header from a <see cref="Stream"/>.
    /// </summary>
    /// <param name="stream">The stream of the module.</param>
    /// <param name="header">The header of the module.</param>
    /// <returns><see cref="true"/> if header was successfully read. <see cref="false"/> otherwise.</returns>
    public static bool TryRead(Stream stream, out Header header)
    {
        header = default;

        // Pre-declare explicit header components
        Unsafe.SkipInit(out ushort magic);
        Unsafe.SkipInit(out ushort version);
        Unsafe.SkipInit(out uint flags);
        Unsafe.SkipInit(out uint entryPoint);

        // Try reading explict header components
        bool success = stream.TryRead(out magic) && 
                       stream.TryRead(out version) &&
                       stream.TryRead(out flags) &&
                       stream.TryRead(out entryPoint);
        
        // Return if component reading failed
        if (!success)
            return false;

        // Assign explicit header components
        header.Magic = magic;
        header.Version = version;
        header.Flags = flags;
        header.EntryPoint = entryPoint;
        header._sizes = new uint[10];
        
        // Try loading size components
        for (int i = 0; i < header._sizes.Length; i++)
        {
            if(!stream.TryRead<uint>(out var x))
                return false;

            header._sizes[i] = x;
        }

        return true;
    }

    /// <summary>
    /// Writes the header to a stream.
    /// </summary>
    /// <param name="stream">The stream to write the header on.</param>
    /// <returns><see cref="true"/> if header was successfully written. <see cref="false"/> otherwise.</returns>
    public readonly bool TryWrite(Stream stream)
    {
        // Try write explicit header components
        bool success = stream.TryWrite(Magic) &&
                       stream.TryWrite(Version) &&
                       stream.TryWrite(Flags) && 
                       stream.TryWrite(EntryPoint);

        // Return if component writing failed
        if (!success)
            return false;

        for (int i = 0; i < 10; i++)
        {
            if(!stream.TryWrite(_sizes[i]))
                return false;
        }

        stream.Flush();
        return true;
    }
}
