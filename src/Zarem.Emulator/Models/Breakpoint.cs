// Avishai Dernis 2026

namespace Zarem.Emulator.Models;

/// <summary>
/// A breakpoint in the code.
/// </summary>
public class Breakpoint
{
    /// <summary>
    /// Gets or sets the breakpoint address.
    /// </summary>
    public uint Address { get; }

    ///// <summary>
    ///// Gets or sets the instruction to break in-place of.
    ///// </summary>
    ///// <remarks>
    ///// This instruction is replaced with a break instruction in the code.
    ///// It must be executed after the breakpoint is hit.
    ///// </remarks>
    //public Instruction Instruction { get; }

    /// <summary>
    /// Gets or sets whether or not the breakpoint is enabled.
    /// </summary>
    public bool IsEnabled { get; set; }
}
