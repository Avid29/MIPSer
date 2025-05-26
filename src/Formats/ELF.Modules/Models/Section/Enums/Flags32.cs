// Adam Dernis 2025

namespace ELF.Modules.Models.Section.Enums;

/// <summary>
/// An enum for the flag field of the ELF section header format in 32bit.
/// </summary>
[Flags]
public enum Flags32 : uint
{
    #pragma warning disable CS1591

    Writable = 0x001,

    /// <summary>
    /// Occupies memory during execution.
    /// </summary>
    Allocate = 0x002,
    Executable = 0x004,

    /// <summary>
    /// Might be merged.
    /// </summary>
    Merged = 0x010,
    
    /// <summary>
    /// Contained null-terminated strings
    /// </summary>
    Strings = 0x020,

    ContainsInfoLink = 0x040,
    PreserveOrder = 0x080,
    Grouped = 0x200,
    ThreadLocalData = 0x400,

    #pragma warning restore CS1591
}
