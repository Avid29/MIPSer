// Avishai Dernis 2025

using System;

namespace MIPS.Assembler.Parsers.Enums;

/// <summary>
/// An enum representing the changes to an integer as a result of truncation or signed conversion.
/// </summary>
[Flags]
public enum CastingChanges
{
    /// <summary>
    /// The truncation and sign change maintained after cast.
    /// </summary>
    None = 0,

    /// <summary>
    /// The value was changed as a result of the cast.
    /// </summary>
    SignChanged = 0x1,

    /// <summary>
    /// The original integer was too large to be represented by a truncation in the cast.
    /// </summary>
    TruncatedHigh = 0x2,

    /// <summary>
    /// The original integer had lower bits that were dropped as a result of the shift.
    /// </summary>
    TruncatedLow = 0x3,

    /// <summary>
    /// The integer value was too large and the sign was dropped.
    /// </summary>
    TruncatedHighAndSignChanged = TruncatedHigh | SignChanged,

    /// <summary>
    /// The integer value dropped lower bits and the sign.
    /// </summary>
    TruncatedLowAndSignChanged = TruncatedLow | SignChanged,

    /// <summary>
    /// The integer value was truncated on both ends.
    /// </summary>
    TruncatedLowAndHigh = TruncatedLow | TruncatedHigh,

    /// <summary>
    /// The integer value was truncated on both ends and the sign was changed.
    /// </summary>
    TruncatedLowAndHighAndSignChanged = TruncatedLow | TruncatedHigh | SignChanged
}
