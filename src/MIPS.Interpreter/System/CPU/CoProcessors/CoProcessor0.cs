// Adam Dernis 2024

using MIPS.Emulator.System.CPU.Models.Registers;

namespace MIPS.Emulator.System.CPU.CoProcessors;

/// <summary>
/// A class representing the 0th co-processor unit.
/// </summary>
public class CoProcessor0
{
    /// <summary>
    /// Gets or sets the status register.
    /// </summary>
    public StatusRegister StatusReg { get; set; }
}
