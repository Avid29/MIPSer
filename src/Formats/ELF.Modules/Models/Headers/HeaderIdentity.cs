// Adam Dernis 2025

using ELF.Modules.Models.Headers.Enums;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace ELF.Modules.Models.Headers;

/// <summary>
/// A struct containing the module header identity info for the ELF format.
/// </summary>
[StructLayout(LayoutKind.Explicit, Size = 16)]
public struct HeaderIdentity
{
    [FieldOffset(0x0)]
    private uint _magic;

    [FieldOffset(0x4)]
    private Class _class;

    [FieldOffset(0x5)]
    private Data _data;
    
    [FieldOffset(0x6)]
    private byte _version; // Should be 1

    [FieldOffset(0x7)]
    private OSABI _osabi;

    [FieldOffset(0x8)]
    private byte _abiVersion;
    
    /// <summary>
    /// Gets the module's magic number.
    /// </summary>
    public uint Magic
    {
        readonly get => _magic;
        internal set => _magic = value;
    }
    
    /// <summary>
    /// Gets the module's bit count.
    /// </summary>
    public Class Class
    {
        readonly get => _class;
        internal set => _class = value;
    }
    
    /// <summary>
    /// Gets the module's endianness.
    /// </summary>
    public Data Data
    {
        readonly get => _data;
        internal set => _data = value;
    }
    
    /// <summary>
    /// Gets the module's elf version.
    /// </summary>
    public byte Version
    {
        readonly get => _version;
        internal set => _version = value;
    }
    
    /// <summary>
    /// Gets the module's operating system application binary interface.
    /// </summary>
    public byte OperatingSystemABI
    {
        readonly get => _version;
        internal set => _version = value;
    }
    
    /// <summary>
    /// Gets the module's operating system application binary interface.
    /// </summary>
    public byte OperatingSystemABIVersion
    {
        readonly get => _abiVersion;
        internal set => _abiVersion = value;
    }

    
    /// <summary>
    /// Attempts to read a header from a <see cref="Stream"/>.
    /// </summary>
    /// <param name="stream">The stream of the module.</param>
    /// <param name="header">The header of the module.</param>
    /// <param name="littleEndian">If the module is expected to be little endian.</param>
    /// <returns><see cref="true"/> if header was successfully read. <see cref="false"/> otherwise.</returns>
    public static bool TryRead(Stream stream, out HeaderIdentity header, bool littleEndian = false)
    {
        header = default;

        // Pre-declare explicit header components
        Unsafe.SkipInit(out uint magic);
        Unsafe.SkipInit(out byte @class);
        Unsafe.SkipInit(out byte data);
        Unsafe.SkipInit(out byte version);

        // Try reading explict header components
        bool success = stream.TryRead(out magic, littleEndian) && 
                       stream.TryRead(out @class, littleEndian) &&
                       stream.TryRead(out data, littleEndian) &&
                       stream.TryRead(out version, littleEndian);
        
        // Return if component reading failed
        if (!success)
            return false;

        // Assign explicit header components
        header.Magic = magic;
        header.Class = (Class)@class;
        header.Data = (Data)data;
        header.Version = version;

        return true;
    }

    /// <summary>
    /// Writes the header to a stream.
    /// </summary>
    /// <param name="stream">The stream to write the header on.</param>
    /// <returns><see cref="true"/> if header was successfully written. <see cref="false"/> otherwise.</returns>
    public readonly bool TryWrite(Stream stream)
    {
        bool littleEndian = Data is Data.LittleEndian;

        // Try write explicit header components
        bool success = stream.TryWrite(Magic, littleEndian) &&
                       stream.TryWrite((byte)Class, littleEndian) &&
                       stream.TryWrite((byte)Data, littleEndian) && 
                       stream.TryWrite(Version, littleEndian);

        // Return if component writing failed
        if (!success)
            return false;

        stream.Flush();
        return true;
    }
}
