// Avishai Dernis 2026

using MIPS.Emulator.Models.System.Execution;
using MIPS.Emulator.Models.System.Execution.Enum;
using MIPS.Models.Instructions;
using MIPS.Models.Instructions.Enums.Operations;
using System;
using System.Numerics;
using System.Runtime.CompilerServices;

namespace MIPS.Emulator.Executor;

/// <summary>
/// A class which handles converting decoded instructions into <see cref="Execution"/> models.
/// </summary>
public partial class InstructionExecutor
{
    private Execution CreateMemoryExecution()
    {
        return Instruction.OpCode switch
        {
            // TODO: Implement unaligned accesses (LWL, LWR, SWL, SWR)
            OperationCode.LoadByte => Load<sbyte>(),
            OperationCode.LoadHalfWord => Load<short>(),
            OperationCode.LoadWordLeft => throw new NotImplementedException(),
            OperationCode.LoadWord => Load<int>(),
            OperationCode.LoadByteUnsigned => Load<byte>(),
            OperationCode.LoadHalfWordUnsigned => Load<ushort>(),
            OperationCode.LoadWordRight => throw new NotImplementedException(),

            OperationCode.StoreByte => Store<sbyte>(),
            OperationCode.StoreHalfWord => Store<short>(),
            OperationCode.StoreWordLeft => throw new NotImplementedException(),
            OperationCode.StoreWord => Store<int>(),
            OperationCode.StoreWordRight => throw new NotImplementedException(),

            _ => throw new NotImplementedException()
        };
    }

    private Execution Load<T>()
        where T : unmanaged, IBinaryInteger<T>
    {
        uint baseAddr = RS;
        int offset = Instruction.ImmediateValue; // already sign-extended
        uint addr = baseAddr + (uint)offset;

        int size = Unsafe.SizeOf<T>();

        // Alignment check (bytes are always aligned)
        if (size > 1 && (addr & (uint)(size - 1)) != 0)
        {
            return CreateTrap(TrapKind.AddressErrorLoad);
        }

        // Load the value from memory and zero/sign-extend it to 32 bits
        var value = 0;
        uint result = uint.CreateTruncating(value);

        return new Execution(Instruction.RT, result);
    }

    private Execution Store<T>()
        where T : unmanaged, IBinaryInteger<T>
    {
        uint baseAddr = RS;
        int offset = Instruction.ImmediateValue; // already sign-extended
        uint addr = baseAddr + (uint)offset;

        int size = Unsafe.SizeOf<T>();

        // Alignment check (bytes are always aligned)
        if (size > 1 && (addr & (uint)(size - 1)) != 0)
        {
            return CreateTrap(TrapKind.AddressErrorLoad);
        }

        // TODO: Handle writeback size in apply stage
        // In the meantime, we'll read the existing value from memory, combine it with the value from the register, and write it back
        var regValue = RT;
        uint memValue = 0;
        var regMask = uint.MaxValue >> (32 - size * 8);
        var memMask = ~regMask;
        var value = (regValue & regMask) | (memValue & memMask);

        return new Execution(addr, value);
    }
}
