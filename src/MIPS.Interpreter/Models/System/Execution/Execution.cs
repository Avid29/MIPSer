// Avishai Dernis 2025

using MIPS.Interpreter.Models.System.Execution.Enum;
using MIPS.Models.Instructions.Enums.Registers;

namespace MIPS.Interpreter.Models.System.Execution;

/// <summary>
/// A struct representing the results of an instruction's execution.
/// </summary>
public struct Execution()
{
    private readonly uint _writeValue;
    private readonly uint _pc;
    private readonly uint _memAddress;
    private readonly Register _reg;

    /// <summary>
    /// Gets the execution output.
    /// </summary>
    public uint Output
    {
        readonly get => _writeValue;
        init => _writeValue = value;
    }
    
    /// <summary>
    /// Gets the general purpose register destination of the output.
    /// </summary>
    /// <remarks>
    /// <see cref="Register.Zero"/> if none.
    /// </remarks>
    public Register Destination
    {
        readonly get => Get(_reg, RegisterSet.GeneralPurpose);
        init => Set(ref _reg, value, RegisterSet.GeneralPurpose);
    }
    
    /// <summary>
    /// Gets the coprocess0 register destination of the output.
    /// </summary>
    public CP0Registers CPR0
    {
        readonly get => Get((CP0Registers)_reg, RegisterSet.CoProc0);
        init => Set(ref _reg, (Register)value, RegisterSet.CoProc0);
    }
    
    /// <summary>
    /// Gets the float register destination of the output.
    /// </summary>
    public FloatRegister FPR
    {
        readonly get => Get((FloatRegister)_reg, RegisterSet.FloatingPoints);
        init => Set(ref _reg, (Register)value, RegisterSet.FloatingPoints);
    }

    /// <summary>
    /// Inits the new value of the high/low registers if applicable.
    /// </summary>
    public ulong HighLow
    {
        init
        {
            Low = (uint)(value & uint.MaxValue);
            High = (uint)(value >> 32);
            SideEffects = SecondaryWritebacks.HighLow;
        }
    }

    /// <summary>
    /// Gets the new value of the low register if applicable.
    /// </summary>
    public uint Low
    {
        get => Get(_memAddress, SecondaryWritebacks.High);
        init => Set(ref _memAddress, value, SecondaryWritebacks.High);
    }

    /// <summary>
    /// Gets the new value of the low register if applicable.
    /// </summary>
    public uint High
    {
        get => Get(_memAddress, SecondaryWritebacks.High);
        init => Set(ref _memAddress, value, SecondaryWritebacks.High);
    }

    /// <summary>
    /// Gets the new PC value, if application.
    /// </summary>
    public uint ProgramCounter
    {
        readonly get => Get(_pc, SecondaryWritebacks.ProgramCounter);
        init => Set(ref _pc, value, SecondaryWritebacks.ProgramCounter);
    }

    /// <summary>
    /// Gets the new address writeback value, if application.
    /// </summary>
    public uint MemAddress
    {
        readonly get => Get(_memAddress, SecondaryWritebacks.Memory);
        init => Set(ref _memAddress, value, SecondaryWritebacks.Memory);
    }

    /// <summary>
    /// Gets the register set to writeback to.
    /// </summary>
    public RegisterSet RegisterSet { get; private set; }

    /// <summary>
    /// 
    /// </summary>
    public SecondaryWritebacks SideEffects {get; private set; }

    /// <summary>
    /// Gets an execution that does nothing.
    /// </summary>
    public static Execution NoOp => new()
    {
        SideEffects = SecondaryWritebacks.None,
        Destination = Register.Zero,
        RegisterSet = RegisterSet.GeneralPurpose,
    };

    /// <summary>
    /// Gets a value indicating whether or not execution handled the PC changing.
    /// </summary>
    public readonly bool PCHandled => SideEffects == SecondaryWritebacks.ProgramCounter;

    private readonly T Get<T>(T field, RegisterSet when, T fallback = default)
        where T : unmanaged
    {
        if (RegisterSet == when)
            return field;

        return fallback;
    }

    private void Set<T>(ref T field, T value, RegisterSet set)
    {
        field = value;
        RegisterSet = set;
    }

    private readonly T Get<T>(T field, SecondaryWritebacks when, T fallback = default)
        where T : unmanaged
    {
        // TODO: Design around this instead of handling it specially
        if (when is SecondaryWritebacks.Low or SecondaryWritebacks.High
            && SideEffects is SecondaryWritebacks.HighLow)
            return field;

        if (SideEffects == when)
            return field;

        return fallback;
    }

    private void Set<T>(ref T field, T value, SecondaryWritebacks set)
    {
        field = value;
        SideEffects = set;
    }
}
