// Adam Dernis 2023

using MIPS.Models.Instructions.Enums;

namespace MIPS.Emulator.System;

/// <summary>
/// A class representing a processor unit.
/// </summary>
public partial class ProcessingUnit
{
    private uint _programCounter;
    private RegisterFile _regFile;

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
        Jump(_programCounter * 4);
    }

    private void JumpOffset(short offset)
    {
        _programCounter = (uint)((int)_programCounter + offset);
    }

    private void Link()
    {
        _regFile[Register.ReturnAddress] = _programCounter + 4;
    }
}
