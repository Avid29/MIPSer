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
        where T : IBinaryInteger<T>
    {
        uint baseAddr = RS;
        int offset = Instruction.ImmediateValue; // already sign-extended
        uint addr = baseAddr + (uint)offset;

        // Alignment check (bytes are always aligned)
        int size = Unsafe.SizeOf<T>();
        if (size > 1 && (addr & (uint)(size - 1)) != 0)
        {
            return CreateTrap(TrapKind.AddressErrorStore);
        }

        bool signed = (-T.MultiplicativeIdentity) < T.Zero;
        return new Execution(Instruction.RT, addr, size, signed);
    }

    private Execution Store<T>()
    {
        uint baseAddr = RS;
        int offset = Instruction.ImmediateValue; // already sign-extended
        uint addr = baseAddr + (uint)offset;

        // Alignment check (bytes are always aligned)
        int size = Unsafe.SizeOf<T>();
        if (size > 1 && (addr & (uint)(size - 1)) != 0)
        {
            return CreateTrap(TrapKind.AddressErrorStore);
        }

        return new Execution(RT, addr, size);
    }
}
