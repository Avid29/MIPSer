// Adam Dernis 2025

using EFL.Modules.Models.Header.Enums;
using System.Runtime.InteropServices;

namespace EFL.Modules.Models.Header;

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
}
