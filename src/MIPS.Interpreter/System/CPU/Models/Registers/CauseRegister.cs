// Adam Dernis 2024

using MIPS.Helpers;

namespace MIPS.Interpreter.System.CPU.Models.Registers;

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
    private const int COPROCESSOR_EXCEPTION_SIZE = 2;
    private const int COPROCESSOR_EXCEPTION_OFFSET = 28;

    private uint _cause;

    /// <summary>
    /// Gets or sets the pending interupts.
    /// </summary>
    public byte PendingInterupts
    {
        readonly get => (byte)UintMasking.GetShiftMask(_cause, PENDING_INTERUPTS_SIZE, PENDING_INTERUPTS_OFFSET);
        set => UintMasking.SetShiftMask(ref _cause, PENDING_INTERUPTS_SIZE, PENDING_INTERUPTS_OFFSET, value);
    }
    
    /// <summary>
    /// Gets or sets the co-processor for the exception.
    /// </summary>
    public byte CoProcessorException
    {
        readonly get => (byte)UintMasking.GetShiftMask(_cause, COPROCESSOR_EXCEPTION_SIZE, COPROCESSOR_EXCEPTION_OFFSET);
        set => UintMasking.SetShiftMask(ref _cause, COPROCESSOR_EXCEPTION_SIZE, COPROCESSOR_EXCEPTION_OFFSET, value);
    }

    /// <summary>
    /// Gets or sets if the last exception occured in a branch delay slot.
    /// </summary>
    public bool IsBranchDelayed
    {
        readonly get => UintMasking.CheckBit(_cause, BRANCH_DELAY_BIT);
        set => UintMasking.SetBit(ref _cause, BRANCH_DELAY_BIT, value);
    }
}
