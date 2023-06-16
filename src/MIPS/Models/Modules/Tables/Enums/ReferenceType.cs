// Adam Dernis 2023

namespace MIPS.Models.Modules.Tables.Enums;

/// <summary>
/// Gets how to preform the reference.
/// </summary>
public enum ReferenceType : byte
{
    /// <remarks>
    /// Use lower 16 bits to update 16 bit immediate field.
    /// </remarks>
    SimpleImmediate = 0x01,
    
    /// <remarks>
    /// How is this different from 
    /// </remarks>
    HalfWord = 0x02,

    /// <remarks>
    /// Split across two 16 bit immediate fields.
    /// </remarks>
    SplitImmediate = 0x03,

    /// <remarks>
    /// All 32 bits.
    /// </remarks>
    FullWord = 0x04,

    /// <remarks>
    /// 26-bit jump target.
    /// </remarks>
    Address = 0x05,

    /// <remarks>
    /// "like [Split Immediate], but split across first and last words of a three-word series", whatever that means.
    /// </remarks>
    Immediate3 = 0x06,

    // Method flags

    #pragma warning disable CS1591

    Add = 0x00,
    Replace = 0x10,
    Subtract = 0x20,
    
    // Explicit Combinations

    AddSimpleImmediate = ReferenceType.Add | ReferenceType.SimpleImmediate,
    ReplaceSimpleImmediate = ReferenceType.Replace | ReferenceType.SimpleImmediate,
    SubtractSimpleImmediate = ReferenceType.Subtract | ReferenceType.SimpleImmediate,

    #pragma warning restore CS1591
}
