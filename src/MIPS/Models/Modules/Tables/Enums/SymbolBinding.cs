// Adam Dernis 2024

namespace MIPS.Models.Modules.Tables.Enums;

/// <summary>
/// An enum for symbol flags.
/// </summary>
[Flags]
public enum SymbolBinding
{
    #pragma warning disable CS1591

    Local,
    Global,
    Weak,
    
    #pragma warning restore CS1591
}
