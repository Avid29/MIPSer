// Adam Dernis 2024

namespace Zarem.RASM.Tables.Enums;

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
    /// How is this different from simple immediate?
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

#pragma warning disable CS1591

    // Method flags
    Add = 0x00,
    Replace = 0x10,
    Subtract = 0x20,

    // Explicit Combinations
    AddSimpleImmediate = Add | SimpleImmediate,
    ReplaceSimpleImmediate = Replace | SimpleImmediate,
    SubtractSimpleImmediate = Subtract | SimpleImmediate,

    AddHalfWord = Add | HalfWord,
    ReplaceHalfWord = Replace | HalfWord,
    SubtractHalfWord = Subtract | HalfWord,

    AddSplitImmediate = Add | SplitImmediate,
    ReplaceSplitImmediate = Replace | SplitImmediate,
    SubtractSplitImmediate = Subtract | SplitImmediate,

    AddFullWord = Add | FullWord,
    ReplaceFullWord = Replace | FullWord,
    SubtractFullWord = Subtract | FullWord,

    AddAddress = Add | Address,
    ReplaceAddress = Replace | Address,
    SubtractAddress = Subtract | Address,

    AddImmediate3 = Add | Immediate3,
    ReplaceImmediate3 = Replace | Immediate3,
    SubtractImmediate3 = Subtract | Immediate3,

#pragma warning restore CS1591
}
