// Adam Dernis 2024

using MIPS.Interpreter.System.CPU.Models.Registers;

namespace MIPS.Interpreter.System.CPU.CoProcessors;

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
