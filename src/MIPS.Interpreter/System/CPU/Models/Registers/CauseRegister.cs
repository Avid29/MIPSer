// Adam Dernis 2024

using MIPS.Helpers;

namespace MIPS.Emulator.System.CPU.Models.Registers;

/// <summary>
/// TODO
/// </summary>
public struct CauseRegister
{
    private const int EXCEPTION_CODE_OFFSET = 2;
    private const int EXCEPTION_CODE_SIZE = 5;
    private const int PENDING_INTERUPTS_OFFSET = 8;
    private const int PENDING_INTERUPTS_SIZE = 8;
    private const int BRANCH_DELAY_BIT = 31;

    private uint _cause;

    /// <summary>
    /// Gets or sets the pending interupts
    /// </summary>
    public byte PendingInterupts
    {
        get => (byte)UintMasking.GetShiftMask(_cause, PENDING_INTERUPTS_SIZE, PENDING_INTERUPTS_OFFSET);
        set => UintMasking.SetShiftMask(ref _cause, PENDING_INTERUPTS_SIZE, PENDING_INTERUPTS_OFFSET, value);
    }
}
