// Adam Dernis 2024

namespace MIPS.Models.Instructions.Enums;

/// <summary>
/// An enum for the mips instruction types.
/// </summary>
public enum InstructionType
{
#pragma warning disable CS1591

    BasicR,
    BasicI,
    BasicJ,
    RegisterImmediate,
    RegisterImmediateBranch,
    Special2R,
    Special3R,
    Coproc0,
    Coproc0C0,
    Coproc0MFMC0,
    Float,
    Pseudo,

#pragma warning restore CS1591
}
