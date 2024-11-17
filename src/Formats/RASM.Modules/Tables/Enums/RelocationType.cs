// Adam Dernis 2024

namespace RASM.Modules.Tables.Enums;

/// <summary>
/// Gets how to preform the relocation.
/// </summary>
public enum RelocationType : byte
{
    /// <remarks>
    /// Use lower 16 bits to update 16 bit immediate field.
    /// </remarks>
    SimpleImmediate = 0x01,

    /// <remarks>
    /// Split across two 16 bit immediate fields.
    /// </remarks>
    SplitImmediate = 0x02,

    /// <remarks>
    /// All 32 bits.
    /// </remarks>
    FullWord = 0x03,

    /// <remarks>
    /// 26-bit jump target.
    /// </remarks>
    Address = 0x04,

    /// <remarks>
    /// "like [Split Immediate], but split across first and last words of a three-word series", whatever that means.
    /// </remarks>
    Immediate3 = 0x05,
}
