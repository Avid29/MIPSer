// Adam Dernis 2024

using MIPS.Models.Instructions.Enums;

namespace MIPS.Emulator.System.CPU;

/// <summary>
/// A class representing a processor unit.
/// </summary>
public partial class ProcessingUnit
{
    private readonly RegisterFile _regFile;

    private uint _programCounter;

    /// <summary>
    /// Initializes a new instance of the <see cref="ProcessingUnit"/> class.
    /// </summary>
    public ProcessingUnit()
    {
        _regFile = new RegisterFile();
    }

    private void Jump(uint address)
        => _programCounter = address;

    private void JumpPartial(uint address)
    {
        // TODO: Maintain paging
        Jump(address * 4);
    }

    private void JumpOffset(short offset)
    {
        _programCounter = (uint)((int)_programCounter + offset);
    }

    private void Link()
    {
        // Assign return address to instruction after caller location 
        _regFile[Register.ReturnAddress] = _programCounter + 4;
    }
}
