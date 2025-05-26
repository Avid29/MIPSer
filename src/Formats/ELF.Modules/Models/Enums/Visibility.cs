// Adam Dernis 2025

namespace ELF.Modules.Models.Enums;

/// <summary>
/// An enum for the visibility field of the ELF symbol entry format.
/// </summary>
public enum Visibility : byte
{
    #pragma warning disable CS1591

    Default,
    Protected,
    Hidden,
    Internal,

    #pragma warning restore CS1591
}
