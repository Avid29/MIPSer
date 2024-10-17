// Adam Dernis 2024

namespace MIPS.Models.Modules.Tables.Enums;

/// <summary>
/// An enum for the symbol flags
/// </summary>
[Flags]
public enum SymbolFlags : uint
{
    #pragma warning disable CS1591

    Forward = 0x10,
    Defined = 0x20,

    /// <summary>
    /// ?
    /// </summary>
    Def_Equate = 0x40,
    Def_Label = 0x80,

    Register = 0x100,
    PreDefined = 0x200,
    Base = 0x400,
    
    /// <summary>
    /// ?
    /// </summary>
    ExtValue = 0x800,

    /// <summary>
    /// ?
    /// </summary>
    MulDef = 0x1000,

    /// <summary>
    /// ?
    /// </summary>
    Repeat = 0x2000,

    Global = 0x4000,
    Small = 0x8000,
    
    /// <summary>
    /// ?
    /// </summary>
    Adjustable = 0x10000,

    /// <summary>
    /// ?
    /// </summary>
    Discarded = 0x20000,

    /// <summary>
    /// ?
    /// </summary>
    Literal = 0x40000,

    #pragma warning restore CS1591
}
