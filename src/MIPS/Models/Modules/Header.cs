// Adam Dernis 2023

using CommunityToolkit.Diagnostics;
using MIPS.Extensions.System.IO;

namespace MIPS.Models.Modules;

/// <summary>
/// A struct containing the module header info.
/// </summary>
public unsafe struct Header
{
    private ushort _magic;
    private ushort _version;
    private uint _flags;
    private uint _entryPoint;

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
    public uint TextSize
    {
        readonly get => _sizes[0];
        internal set => _sizes[0] = value;
    }

    /// <summary>
    /// Gets the size of the module's rdata section.
    /// </summary>
    public uint ReadOnlyDataSize
    {
        readonly get => _sizes[1];
        internal set => _sizes[1] = value;
    }

    /// <summary>
    /// Gets the size of the module's data section.
    /// </summary>
    public uint DataSize
    {
        readonly get => _sizes[2];
        internal set => _sizes[2] = value;
    }

    /// <summary>
    /// Gets the size of the module's small initialized data section.
    /// </summary>
    public uint SmallDataSize
    {
        readonly get => _sizes[3];
        internal set => _sizes[3] = value;
    }

    /// <summary>
    /// Gets the size of the module's small initialized data section.
    /// </summary>
    public uint SmallUninitializedDataSize
    {
        readonly get => _sizes[4];
        internal set => _sizes[4] = value;
    }

    /// <summary>
    /// Gets the size of the module's small initialized data section.
    /// </summary>
    public uint UninitializedDataSize
    {
        readonly get => _sizes[5];
        internal set => _sizes[5] = value;
    }

    /// <summary>
    /// Gets the size of the module's tables.
    /// </summary>
    public uint TablesSize
    {
        readonly get => _sizes[6];
        internal set => _sizes[6] = value;
    }

    /// <summary>
    /// Gets the number of entries in the module's relocation table.
    /// </summary>
    public uint RelocationTableCount
    {
        readonly get => _sizes[7];
        internal set => _sizes[7] = value;
    }

    /// <summary>
    /// Gets the number of entries the module's reference table.
    /// </summary>
    public uint ReferenceTableCount
    {
        readonly get => _sizes[8];
        internal set => _sizes[8] = value;
    }

    /// <summary>
    /// Gets the number of entries the module's symbol table.
    /// </summary>
    public uint SymbolTableCount
    {
        readonly get => _sizes[9];
        internal set => _sizes[9] = value;
    }

    /// <summary>
    /// Attempts to load a header from a <see cref="Stream"/>.
    /// </summary>
    /// <param name="stream">The stream of the module.</param>
    /// <param name="header">The header of the module.</param>
    /// <returns><see cref="true"/> if header was successfully read. <see cref="false"/> otherwise.</returns>
    public static bool TryLoadHeader(Stream stream, out Header header)
    {
        header = default;
        header.Magic = stream.Read<ushort>();
        header.Version = stream.Read<ushort>();
        header.Flags = stream.Read<uint>();
        header.EntryPoint = stream.Read<uint>();

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
        stream.Write(Magic);
        stream.Write(Version);
        stream.Write(Flags);
        stream.Write(EntryPoint);

        for (int i = 0; i < 10; i++)
            stream.Write(_sizes[i]);

        stream.Flush();
    }
}
