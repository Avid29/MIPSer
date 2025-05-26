// Adam Dernis 2025

namespace ELF.Modules.Models.Headers.Enums;

/// <summary>
/// An enum for the permission flags of a segment in the program header.
/// </summary>
[Flags]
public enum Permissions : uint
{
    #pragma warning disable CS1591

    Executable = 0x1,
    Writeable = 0x2,
    Readable = 0x4,

    #pragma warning restore CS1591
}
