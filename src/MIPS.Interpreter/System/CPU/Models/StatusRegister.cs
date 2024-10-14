// Adam Dernis 2024

using MIPS.Helpers;

namespace MIPS.Emulator.System.CPU.Models;

/// <summary>
/// 
/// </summary>
public struct StatusRegister
{
    private uint _status;

    /// <summary>
    /// Gets or sets if interupts are enabled.
    /// </summary>
    public bool InteruptEnabled
    {
        get => UintMasking.CheckBit(_status, 0);
        set => UintMasking.SetBit(ref _status, 0, value);
    }
}
