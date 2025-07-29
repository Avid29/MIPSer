// Adam Dernis 2024

using MIPS.Interpreter.Models.System.CPU.Registers;
using MIPS.Models.Instructions.Enums.Registers;

namespace MIPS.Interpreter.Models.System.CPU.CoProcessors;

/// <summary>
/// A class representing the 0th co-processor unit.
/// </summary>
public class CoProcessor0
{
    private readonly RegisterFile _regFile;

    /// <summary>
    /// Initializes a new instance of the <see cref="CoProcessor0"/> class.
    /// </summary>
    public CoProcessor0()
    {
        _regFile = new RegisterFile();
    }

    /// <summary>
    /// Gets or sets the status register.
    /// </summary>
    public StatusRegister StatusReg
    {
        get => (StatusRegister)_regFile[CP0Registers.Status];
        set => _regFile[CP0Registers.Status] = (uint)value;
    }
}
