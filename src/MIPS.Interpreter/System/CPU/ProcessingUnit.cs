// Adam Dernis 2024

using MIPS.Emulator.System.CPU.Models;
using MIPS.Models.Instructions.Enums.Registers;

namespace MIPS.Emulator.System.CPU;

/// <summary>
/// A class representing a processor unit.
/// </summary>
public partial class ProcessingUnit
{
    private readonly RegisterFile _regFile;

    /// <summary>
    /// Initializes a new instance of the <see cref="ProcessingUnit"/> class.
    /// </summary>
    public ProcessingUnit()
    {
        _regFile = new RegisterFile();
    }

    private void Jump(uint address)
        => _regFile.ProgramCounter = address;

    private void JumpPartial(uint address)
    {
        // TODO: Maintain paging
        Jump(address * 4);
    }

    private void JumpOffset(short offset)
    {
        _regFile.ProgramCounter = (uint)((int)_regFile.ProgramCounter + offset);
    }

    private void Link()
    {
        // Assign return address to instruction after caller location 
        _regFile[Register.ReturnAddress] = _regFile.ProgramCounter + 4;
    }
}
