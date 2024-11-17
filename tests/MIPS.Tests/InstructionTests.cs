// Adam Dernis 2024

using MIPS.Models.Instructions;
using MIPS.Models.Instructions.Enums;
using MIPS.Models.Instructions.Enums.Registers;
using MIPS.Models.Instructions.Enums.SpecialFunctions;
using System;

namespace MIPS.Tests;

[TestClass]
public class InstructionTests
{
    [TestMethod("Set OpCode")]
    public void SetOpCodeTest()
    {
        // This test sets the opcode to each potential operation code with a random address.
        // It then asserts the readback is equivilient.
        for (var i = OperationCode.Special; i <= OperationCode.StoreWordCoprocessor3; i++)
        {
            var instruction = Instruction.Create(i, RandomAddress(false));
            Assert.AreEqual(i, instruction.OpCode, $"Error setting operation code to {i}");
        }
    }

    [TestMethod("Set Registers")]
    public void SetRegistersTest()
    {
        // This test sets each register argument to each potential register with an otherwise random instruction.
        // It then asserts the readback is equivilient.
        for (var i = Register.Zero; i <= Register.ReturnAddress; i++)
        {
            var instruction = Instruction.Create(RandomOpCode(false), i, RandomRegister(false), RandomImmediate(false));
            Assert.AreEqual(i, instruction.RS, $"Error setting rs register to {i}");

            instruction = Instruction.Create(RandomOpCode(false), RandomRegister(false), i, RandomImmediate(false));
            Assert.AreEqual(i, instruction.RT, $"Error setting rt register to {i}");

            instruction = Instruction.Create(RandomFuncCode(false), RandomRegister(false), RandomRegister(false), i);
            Assert.AreEqual(i, instruction.RD, $"Error setting rd register to {i}");
        }
    }

    [TestMethod("Set Address")]
    public void SetAddressTest()
    {
        // This test sets the address to a random value 20 times.
        // It asserts the readback is equivilient each time.
        for (var i = 0; i < 20; i++)
        {
            var addr = RandomAddress(true);
            var instruction = Instruction.Create(RandomOpCode(false), addr);
            Assert.AreEqual(instruction.Address, addr, $"Error seeting address to {addr}");
        }
    }

    /// <remarks>
    /// Safe constrains the value to within the 16 bit range.
    /// For good testing purposes, this shouldn't always be used as the masking should fix overflowing immediates.
    /// </remarks>
    private static short RandomImmediate(bool safe = true) => (short)Random.Shared.Next(safe ? ushort.MaxValue : int.MaxValue);

    private static uint RandomAddress(bool safe = true) => (uint)Random.Shared.Next(safe ? (1 << 26)-1 : int.MaxValue) & ~(uint)(0b11);

    private static Register RandomRegister(bool safe) => (Register)Random.Shared.Next(safe ? (int)Register.ReturnAddress : int.MaxValue);

    private static OperationCode RandomOpCode(bool safe) => (OperationCode)Random.Shared.Next(safe ? (int)OperationCode.StoreWordCoprocessor3 : int.MaxValue);

    private static FunctionCode RandomFuncCode(bool safe) => (FunctionCode)Random.Shared.Next(safe ? (int)FunctionCode.SetLessThanUnsigned : int.MaxValue);
}
