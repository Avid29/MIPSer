// Adam Dernis 2024

namespace MIPS.Models.Modules.Tables.Enums;

/// <summary>
/// An enum for symbol flags.
/// </summary>
[Flags]
public enum SymbolFlags
{
    #pragma warning disable CS1591

    Global = 0x01,
    ForwardDefined = 0x02,
    

    #pragma warning restore CS1591
}
