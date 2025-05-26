// Adam Dernis 2025

namespace EFL.Modules.Models.Header.Enums;

/// <summary>
/// An enum for the machine field of the ELF header format.
/// It architecture.
/// </summary>
public enum Machine : ushort
{
    #pragma warning disable CS1591

    Intel_x86 = 0x03,
    Intel_MCU = 0x06,
    Intel_80860 = 0x07,
    MIPS = 0x08,
    MIPS_RS3k_LittleEndian = 0x0A,
    PowerPC = 0x14,
    PowerPC64 = 0x15,
    Arm = 0x28,
    IA64 = 0x32,
    AMD_x86_64 = 0x3E,
    VAX = 0x4B,
    Arm64 = 0xB7,
    RISV_V = 0xF3,

    #pragma warning restore
}
