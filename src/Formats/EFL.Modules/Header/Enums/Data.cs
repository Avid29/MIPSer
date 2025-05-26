// Adam Dernis 2025

namespace EFL.Modules.Header.Enums;

/// <summary>
/// An enum for the data field of the ELF header format.
/// It signifies endianness.
/// </summary>
public enum Data : byte
{
    #pragma warning disable CS1591

    LittleEndian = 1,
    BigEndian = 2,
    
    #pragma warning restore
}
