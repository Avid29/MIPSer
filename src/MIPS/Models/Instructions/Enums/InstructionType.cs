// Adam Dernis 2024

namespace MIPS.Models.Instructions.Enums;

/// <summary>
/// An enum for the mips instruction types
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
    Coproc0,
    Pseudo,
#pragma warning restore CS1591
}
