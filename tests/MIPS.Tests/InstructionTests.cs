﻿// Adam Dernis 2024

using MIPS.Models.Instructions;
using MIPS.Models.Instructions.Enums.Operations;
using MIPS.Models.Instructions.Enums.Registers;
using MIPS.Models.Instructions.Enums.SpecialFunctions;
using MIPS.Tests.Helpers;

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
            var instruction = Instruction.Create(i, ArgGenerator.RandomAddress(false));
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
            var instruction = Instruction.Create(
                ArgGenerator.RandomOpCode(false),
                i,
                ArgGenerator.RandomRegister(false),
                ArgGenerator.RandomImmediate(false));
            Assert.AreEqual(i, instruction.RS, $"Error setting rs register to {i}");

            instruction = Instruction.Create(
                ArgGenerator.RandomOpCode(false),
                ArgGenerator.RandomRegister(false),
                i,
                ArgGenerator.RandomImmediate(false));
            Assert.AreEqual(i, instruction.RT, $"Error setting rt register to {i}");

            instruction = Instruction.Create(
                ArgGenerator.RandomFuncCode(false),
                ArgGenerator.RandomRegister(false),
                ArgGenerator.RandomRegister(false),
                i);
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
            var addr = ArgGenerator.RandomAddress();
            var instruction = Instruction.Create(ArgGenerator.RandomOpCode(false), addr);
            Assert.AreEqual(addr, instruction.Address, $"Error setting address to {addr}");
        }
    }

    [TestMethod("Set Offset")]
    public void SetOffsetTest()
    {
        // This test sets the address to a random value 20 times.
        // It asserts the readback is equivilient each time.
        for (var i = 0; i < 20; i++)
        {
            var offset = ArgGenerator.RandomOffset();
            var instruction = Instruction.Create(RegImmFuncCode.BranchOnLessThanZero, ArgGenerator.RandomRegister(), offset);
            Assert.AreEqual(offset, instruction.Offset, $"Error setting offset to {offset}");
        }
    }
}
