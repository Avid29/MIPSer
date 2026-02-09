// Avishai Dernis 2025

using MIPS.Interpreter.Models.System.Execution.Enum;
using MIPS.Models.Instructions.Enums.Registers;

namespace MIPS.Interpreter.Models.System.Execution;

/// <summary>
/// A struct representing the results of an instruction's execution.
/// </summary>
public struct Execution
{
    private readonly uint _secondary1;
    private readonly uint _secondary2;
    private readonly GPRegister _reg;

    /// <summary>
    /// Initializes a new instance of the <see cref="Execution"/> struct.
    /// </summary>
    public Execution(TrapKind trap)
    {
        Trap = trap;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Execution"/> struct.
    /// </summary>
    public Execution(GPRegister dest, uint writeBack)
    {
        GPR = dest;
        WriteBack = writeBack;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Execution"/> struct.
    /// </summary>
    public Execution(CP0Registers dest, uint writeBack)
    {
        CPR0 = dest;
        WriteBack = writeBack;
    }

    /// <summary>
    /// Gets the writeback to the destination.
    /// </summary>
    public readonly uint WriteBack { get; init; }

    /// <summary>
    /// Gets the destination of the output.
    /// </summary>
    /// <remarks>
    /// Will set the register set to none if <see langword="null"/>.
    /// </remarks>
    public GPRegister? Destination
    {
        readonly get
        {
            if (RegisterSet is RegisterSet.None)
                return null;

            return Get(_reg, RegisterSet.GeneralPurpose);
        }
        init
        {
            if (value is not null)
            {
                Set(ref _reg, value.Value, RegisterSet.GeneralPurpose);
            }
            else
            {
                _reg = GPRegister.Zero;
                RegisterSet = RegisterSet.None;
            }
        }
    }

    /// <summary>
    /// Gets the general purpose register destination of the output.
    /// </summary>
    /// <remarks>
    /// <see cref="GPRegister.Zero"/> if none.
    /// </remarks>
    public GPRegister GPR
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
        init => Set(ref _reg, (GPRegister)value, RegisterSet.CoProc0);
    }
    
    /// <summary>
    /// Gets the float register destination of the output.
    /// </summary>
    public FloatRegister FPR
    {
        readonly get => Get((FloatRegister)_reg, RegisterSet.FloatingPoints);
        init => Set(ref _reg, (GPRegister)value, RegisterSet.FloatingPoints);
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
        get => Get(_secondary1, SecondaryWritebacks.Low);
        init => Set(ref _secondary1, value, SecondaryWritebacks.Low);
    }

    /// <summary>
    /// Gets the new value of the low register if applicable.
    /// </summary>
    public uint High
    {
        get => Get(_secondary2, SecondaryWritebacks.High);
        init => Set(ref _secondary2, value, SecondaryWritebacks.High);
    }

    /// <summary>
    /// Gets the new PC value, if application.
    /// </summary>
    public uint ProgramCounter
    {
        readonly get => Get(field, SecondaryWritebacks.ProgramCounter);
        init => Set(ref field, value, SecondaryWritebacks.ProgramCounter);
    }

    /// <summary>
    /// Gets the new address writeback value, if application.
    /// </summary>
    public uint MemAddress
    {
        readonly get => Get(_secondary1, SecondaryWritebacks.Memory);
        init => Set(ref _secondary1, value, SecondaryWritebacks.Memory);
    }

    /// <summary>
    /// Gets the register set to writeback to.
    /// </summary>
    public RegisterSet RegisterSet { get; private set; }

    /// <summary>
    /// Gets the type of secondary writeback from the execution, if any.
    /// </summary>
    public SecondaryWritebacks SideEffects { get; private set; }

    /// <summary>
    /// Gets the type of trap that occurred during execution, if any.
    /// </summary>
    public TrapKind Trap { get; init; }

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
        // This is to handle the fact that some instructions write back to both high and low,
        // but we want to be able to read them both without having to check for both separately.
        if (when is SecondaryWritebacks.Low or SecondaryWritebacks.High
            && SideEffects is SecondaryWritebacks.HighLow)
            return field;

        if (SideEffects == when)
            return field;

        return fallback;
    }

    private void Set<T>(ref T field, T value, SecondaryWritebacks sideEffects)
    {
        // Handle special case for high/low writebacks, since they can be written to separately or together.
        if (sideEffects is SecondaryWritebacks.Low or SecondaryWritebacks.High)
        {
            // If side effects is high or low, and the current side effects is the opposite, set it to highlow.
            // Otherwise, just set it to the new side effect.
            if (sideEffects is SecondaryWritebacks.High && SideEffects is SecondaryWritebacks.Low
                || sideEffects is SecondaryWritebacks.Low && SideEffects is SecondaryWritebacks.High)
            {
                SideEffects = SecondaryWritebacks.HighLow;
            }
            else if (SideEffects is not SecondaryWritebacks.HighLow)
            {
                SideEffects = sideEffects;
            }

            field = value;
            return;
        }

        field = value;
        SideEffects = sideEffects;
    }
}
