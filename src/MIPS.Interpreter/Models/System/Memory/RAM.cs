// Avishai Dernis 2025

namespace MIPS.Interpreter.Models.System.Memory;

/// <summary>
/// Represents the RAM (Random Access Memory) in a MIPS interpreter.
/// </summary>
public class RAM
{
    /// <summary>
    /// Gets or sets the TLB (Translation Lookaside Buffer) for the RAM.
    /// </summary>
    public TLB? TLB { get; set; } = null;


}
