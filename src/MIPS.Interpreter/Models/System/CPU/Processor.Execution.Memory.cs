// Avishai Dernis 2025

using MIPS.Interpreter.Models.System.Execution;
using MIPS.Interpreter.Models.System.Execution.Enum;
using MIPS.Models.Instructions;
using MIPS.Models.Instructions.Enums.Operations;
using System;
using System.Numerics;
using System.Runtime.CompilerServices;

namespace MIPS.Interpreter.System.CPU;

public partial class Processor
{
    private Execution CreateExecutionMemory(Instruction instruction)
    {
        return instruction.OpCode switch
        {
            // TODO: Implement unaligned accesses (LWL, LWR, SWL, SWR)
            OperationCode.LoadByte => Load<sbyte>(instruction),
            OperationCode.LoadHalfWord => Load<short>(instruction),
            OperationCode.LoadWordLeft => throw new NotImplementedException(),
            OperationCode.LoadWord => Load<int>(instruction),
            OperationCode.LoadByteUnsigned => Load<byte>(instruction),
            OperationCode.LoadHalfWordUnsigned => Load<ushort>(instruction),
            OperationCode.LoadWordRight => throw new NotImplementedException(),

            OperationCode.StoreByte => Store<sbyte>(instruction),
            OperationCode.StoreHalfWord => Store<short>(instruction),
            OperationCode.StoreWordLeft => throw new NotImplementedException(),
            OperationCode.StoreWord => Store<int>(instruction),
            OperationCode.StoreWordRight => throw new NotImplementedException(),

            _ => throw new NotImplementedException()
        };
    }

    private Execution Load<T>(Instruction instruction)
        where T : unmanaged, IBinaryInteger<T>
    {
        uint baseAddr = RegisterFile[instruction.RS];
        int offset = instruction.ImmediateValue; // already sign-extended
        uint addr = baseAddr + (uint)offset;

        int size = Unsafe.SizeOf<T>();

        // Alignment check (bytes are always aligned)
        if (size > 1 && (addr & (uint)(size - 1)) != 0)
        {
            return new Execution(TrapKind.AddressErrorLoad);
        }

        // Load the value from memory and zero/sign-extend it to 32 bits
        var value = _computer.Memory.Read<T>(addr);
        uint result = uint.CreateTruncating(value);

        return new Execution(instruction.RT, result);
    }

    private Execution Store<T>(Instruction instruction)
        where T : unmanaged, IBinaryInteger<T>
    {
        uint baseAddr = RegisterFile[instruction.RS];
        int offset = instruction.ImmediateValue; // already sign-extended
        uint addr = baseAddr + (uint)offset;

        int size = Unsafe.SizeOf<T>();

        // Alignment check (bytes are always aligned)
        if (size > 1 && (addr & (uint)(size - 1)) != 0)
        {
            return new Execution(TrapKind.AddressErrorLoad);
        }

        // TODO: Handle writeback size in apply stage
        // In the meantime, we'll read the existing value from memory, combine it with the value from the register, and write it back
        var regValue = RegisterFile[instruction.RT];
        var memValue = _computer.Memory.Read<uint>(addr);
        var regMask = uint.MaxValue >> (32 - size * 8);
        var memMask = ~regMask;
        var value = (regValue & regMask) | (memValue & memMask);

        return new Execution(addr, value);
    }
}
