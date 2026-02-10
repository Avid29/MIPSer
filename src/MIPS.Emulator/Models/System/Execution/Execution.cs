// Avishai Dernis 2025

using MIPS.Emulator.Models.System.Execution.Enum;
using MIPS.Helpers;
using MIPS.Models.Instructions.Enums.Registers;
using System;

namespace MIPS.Emulator.Models.System.Execution;

/// <summary>
/// A struct representing the results of an instruction's execution.
/// </summary>
public readonly struct Execution
{
    private const int REG_BITCOUNT = 5;
    private const int REGSET_OFFSET = REG_BITCOUNT;
    private const int REGSET_BITCOUNT = 4;

    // These values are used for secondary effects
    // They can be (low, high), (memAddress, size*(-signed)), (pc, _), (writeback, register|regset)
    private readonly uint _secondary1; 
    private readonly uint _secondary2;

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
        CoProc0Reg = dest;
        CoProcWriteBack = writeBack;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Execution"/> struct.
    /// </summary>
    public Execution(GPRegister dest, uint address, int size, bool signed = true)
    {
        GPR = dest;
        MemAddress = address;
        SideEffects = SecondaryEffect.ReadMemory;

        size *= signed ? -1 : 1;
        _secondary2 = (uint)size;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Execution"/> struct.
    /// </summary>
    public Execution(uint writeBack, uint address, int size)
    {
        WriteBack = writeBack;
        MemAddress = address;
        SideEffects = SecondaryEffect.WriteMemory;

        _secondary2 = (uint)size;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Execution"/> struct.
    /// </summary>
    public Execution(uint pc)
    {
        ProgramCounter = pc;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Execution"/> struct.
    /// </summary>
    public Execution(ulong highLow)
    {
        High = (uint)(highLow >> 32);
        Low = (uint)highLow;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Execution"/> struct.
    /// </summary>
    public Execution((uint High, uint Low) highLow)
    {
        High = highLow.High;
        Low = highLow.Low;
    }

    /// <summary>
    /// Gets the writeback value to the selected GPR register.
    /// </summary>
    public readonly uint WriteBack { get; init; }

    /// <summary>
    /// Gets the general purpose register destination of the output.
    /// </summary>
    /// <remarks>
    /// <see cref="GPRegister.Zero"/> if none.
    /// </remarks>
    public GPRegister GPR { get; init; }

    /// <summary>
    /// Gets the type of secondary writeback from the execution, if any.
    /// </summary>
    public SecondaryEffect SideEffects { get; init; }

    /// <summary>
    /// Gets the new value of the low register if applicable.
    /// </summary>
    public readonly uint Low
    {
        get => _secondary1;
        init
        {
            _secondary1 = value;
            SideEffects = MergeHighLow(SecondaryEffect.Low);
        }
    }

    /// <summary>
    /// Gets the new value of the low register if applicable.
    /// </summary>
    public readonly uint High
    {
        get => _secondary2;
        init
        {
            _secondary2 = value;
            SideEffects = MergeHighLow(SecondaryEffect.High);
        }
    }

    /// <summary>
    /// Gets the new PC value, if application.
    /// </summary>
    public readonly uint ProgramCounter
    {
        get => _secondary1;
        init
        {
            _secondary1 = value;
            SideEffects = SecondaryEffect.ProgramCounter;
        }
    }

    /// <summary>
    /// Gets the memory address to read or write at, if applicable.
    /// </summary>
    public readonly uint MemAddress
    {
        get => _secondary1;
        init => _secondary1 = value;
    }

    /// <summary>
    /// Gets the size of the memory operation to perform, if applicable
    /// </summary>
    /// <remarks>
    /// Number of bytes to read/write.
    /// </remarks>
    public readonly uint MemSize => (uint)int.Abs((int)_secondary2);

    /// <summary>
    /// Gets whether or not the memory operation is singed (should sign-extend)
    /// </summary>
    public readonly bool MemSigned => (int)_secondary2 < 0;

    /// <summary>
    /// Gets the register set to writeback to for co-process writeback.
    /// </summary>
    public readonly GPRegister CoProcReg
    {
        get => (GPRegister)UintMasking.GetShiftMask(_secondary2, REG_BITCOUNT, 0);
        init
        {
            UintMasking.SetShiftMask(ref _secondary2, REG_BITCOUNT, 0, (uint)value);
            SideEffects = SecondaryEffect.WriteCoProc;
        }
    }

    /// <summary>
    /// Gets the coproc0 register for a co-process writeback.
    /// </summary>
    public readonly CP0Registers CoProc0Reg
    {
        get => (CP0Registers)UintMasking.GetShiftMask(_secondary2, REG_BITCOUNT, 0);
        init
        {
            UintMasking.SetShiftMask(ref _secondary2, REG_BITCOUNT, 0, (uint)value);
            CoProcRegisterSet = RegisterSet.CoProc0;
        }
    }

    /// <summary>
    /// Gets the register set to writeback to for co-process writeback.
    /// </summary>
    public readonly RegisterSet CoProcRegisterSet
    {
        get => (RegisterSet)UintMasking.GetShiftMask(_secondary2, REGSET_BITCOUNT, REGSET_OFFSET);
        init
        {
            UintMasking.SetShiftMask(ref _secondary2, REGSET_BITCOUNT, REGSET_OFFSET, (uint)value);
            SideEffects = SecondaryEffect.WriteCoProc;
        }
    }

    /// <summary>
    /// Gets the value writing back to a co-processor.
    /// </summary>
    public readonly uint CoProcWriteBack
    {
        get => _secondary1;
        init
        {
            _secondary1 = value;
            SideEffects = SecondaryEffect.WriteCoProc;
        }
    }

    /// <summary>
    /// Gets a value indicating whether or not execution handled the PC changing.
    /// </summary>
    public readonly bool PCHandled => SideEffects == SecondaryEffect.ProgramCounter;

    private SecondaryEffect MergeHighLow(SecondaryEffect @new)
    {
        if (SideEffects is SecondaryEffect.Low or
            SecondaryEffect.High or SecondaryEffect.HighLow)
        {
            return SideEffects | @new;
        }

        return @new;
    }
}
