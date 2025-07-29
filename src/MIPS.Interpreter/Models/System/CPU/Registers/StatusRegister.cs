// Avishai Dernis 2025

using MIPS.Helpers;
using System.Runtime.CompilerServices;

namespace MIPS.Interpreter.Models.System.CPU.Registers;

/// <summary>
/// TODO
/// </summary>
public struct StatusRegister
{
    private const int INTERUPT_ENABLED_BIT = 0;
    private const int EXCEPTION_LEVEL_BIT = 1;
    private const int USER_MODE_BIT = 4;
    private const int INTERUPT_MASK_SIZE = 8;
    private const int INTERUPT_MASK_OFFSET = 8;

    private uint _status;

    /// <summary>
    /// Gets or sets if interupts are enabled.
    /// </summary>
    public bool InteruptEnabled
    {
        readonly get => UintMasking.CheckBit(_status, INTERUPT_ENABLED_BIT);
        set => UintMasking.SetBit(ref _status, INTERUPT_ENABLED_BIT, value);
    }

    /// <summary>
    /// Gets or sets the exception level.
    /// </summary>
    public bool ExceptionLevel
    {
        readonly get => UintMasking.CheckBit(_status, EXCEPTION_LEVEL_BIT);
        set => UintMasking.SetBit(ref _status, EXCEPTION_LEVEL_BIT, value);
    }

    /// <summary>
    /// Gets or sets if user mode is enabled.
    /// </summary>
    public bool UserMode
    {
        readonly get => UintMasking.CheckBit(_status, USER_MODE_BIT);
        set => UintMasking.SetBit(ref _status, USER_MODE_BIT, value);
    }

    /// <summary>
    /// Gets or sets the interupt mask.
    /// </summary>
    public byte InteruptMask
    {
        readonly get => (byte)UintMasking.GetShiftMask(_status, INTERUPT_MASK_SIZE, INTERUPT_MASK_OFFSET);
        set => UintMasking.SetShiftMask(ref _status, INTERUPT_MASK_SIZE, INTERUPT_MASK_OFFSET, value);
    }

    /// <summary>
    /// Casts a <see cref="uint"/> to a <see cref="StatusRegister"/>.
    /// </summary>
    public static unsafe explicit operator StatusRegister(uint value) => Unsafe.As<uint, StatusRegister>(ref value);

    /// <summary>
    /// Casts a <see cref="StatusRegister"/> to a <see cref="uint"/>.
    /// </summary>
    public static unsafe explicit operator uint(StatusRegister value) => Unsafe.As<StatusRegister, uint>(ref value);
}
