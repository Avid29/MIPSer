// Adam Dernis 2023

using CommunityToolkit.Diagnostics;
using System.Diagnostics.CodeAnalysis;

namespace MIPS.Models;

/// <summary>
/// A struct containing the module header info.
/// </summary>
public unsafe struct Header
{
    private ushort _magic;
    private ushort _version;
    private uint _flags;
    private uint _entryPoint;
    
    [SuppressMessage("Usage", "CS0649", Justification = "Written to unsafely with  indexing.")]
    private fixed uint _sizes[10];

    /// <summary>
    /// Initializes a new instance of the <see cref="Header"/> struct.
    /// </summary>
    public Header(ushort magic, ushort version, uint flags, uint entry, params uint[] sizes)
    {
        _magic = magic;
        _version = version;
        _flags = flags;
        _entryPoint = entry;

        if (sizes.Length != 10)
        {
            ThrowHelper.ThrowArgumentException($"{nameof(sizes)} must have a length of exactly 10.");
        }

        for (int i = 0; i < sizes.Length; i++)
        {
            _sizes[i] = sizes[i];
        }
    }

    /// <summary>
    /// Gets the module magic identifier.
    /// </summary>
    public ushort Magic => _magic;

    /// <summary>
    /// Gets the module format version number.
    /// </summary>
    public ushort Version => _version;

    /// <summary>
    /// Gets the module flags.
    /// </summary>
    public uint Flags => _flags;

    /// <summary>
    /// Gets the module entry point.
    /// </summary>
    public uint EntryPoint => _entryPoint;

    /// <summary>
    /// Gets the size of the module's text section.
    /// </summary>
    public uint TextSize => _sizes[0];

    /// <summary>
    /// Gets the size of the module's rdata section.
    /// </summary>
    public uint ReadOnlyDataSize => _sizes[1];

    /// <summary>
    /// Gets the size of the module's data section.
    /// </summary>
    public uint DataSize => _sizes[2];

    /// <summary>
    /// Gets the size of the module's small initialized data section.
    /// </summary>
    public uint SmallDataSize => _sizes[3];

    /// <summary>
    /// Gets the size of the module's small initialized data section.
    /// </summary>
    public uint SmallUninitializedDataSize => _sizes[4];

    /// <summary>
    /// Gets the size of the module's small initialized data section.
    /// </summary>
    public uint UninitializedDataSize => _sizes[5];

    /// <summary>
    /// Gets the size of the module's tables.
    /// </summary>
    public uint TablesSize => _sizes[6];

    /// <summary>
    /// Gets the number of entries in the module's relative table.
    /// </summary>
    public uint RelativeTableCount => _sizes[7];

    /// <summary>
    /// Gets the number of entries the module's reference table.
    /// </summary>
    public uint ReferenceTableCount => _sizes[8];

    /// <summary>
    /// Gets the number of entries the module's symbol table.
    /// </summary>
    public uint SymbolTableCount => _sizes[9];

    /// <summary>
    /// Attempts to load a header from a <see cref="Stream"/>.
    /// </summary>
    /// <param name="stream">The stream of the module.</param>
    /// <param name="header">The header of the module.</param>
    /// <returns><see cref="true"/> if header was successfully read. <see cref="false"/> otherwise.</returns>
    public static bool TryLoadHeader(Stream stream, out Header header)
    {
        header._magic = stream.Read<ushort>();
        header._version = stream.Read<ushort>();
        header._flags = stream.Read<uint>();
        header._entryPoint = stream.Read<uint>();

        for (int i = 0; i < 10; i++)
        {
            // Read sizes
            header._sizes[i] = stream.Read<uint>();
        }

        return true;
    }

    /// <summary>
    /// Writes the header to a stream.
    /// </summary>
    /// <param name="stream">The stream to write the header on.</param>
    public void WriteHeader(Stream stream)
    {
        stream.Write(_magic);
        stream.Write(_version);
        stream.Write(_flags);
        stream.Write(_entryPoint);

        for (int i = 0; i < 10; i++)
            stream.Write(_sizes[i]);

        stream.Flush();
    }
}
