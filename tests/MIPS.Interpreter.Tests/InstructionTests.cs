// Avishai Dernis 2026

using MIPS.Assembler.Models.Instructions;
using MIPS.Assembler.Parsers;
using MIPS.Assembler.Tokenization;
using MIPS.Interpreter.Models.System.Execution.Enum;
using MIPS.Models.Instructions;
using MIPS.Models.Instructions.Enums.Registers;
using MIPS.Models.Instructions.Enums.SpecialFunctions;
using System.Collections.Generic;
using System.Numerics;

namespace MIPS.Interpreter.Tests;

[TestClass]
public class InstructionTests
{
    public sealed record SimpleInstructionTestCase(
        string Input,
        (GPRegister Regiter, uint Value)? Expected = null,
        TrapKind Trap = TrapKind.None,
        params (GPRegister Register, uint Value)[] RegisterInitialization)
    {
        public SimpleInstructionTestCase(string input, (GPRegister, uint) expected, params (GPRegister, uint)[] registerInit) : this(input, expected, TrapKind.None, registerInit)
        {
        }

        public SimpleInstructionTestCase(string input, TrapKind trap, params (GPRegister, uint)[] registerInit) : this(input, Trap: trap, RegisterInitialization: registerInit)
        {
        }
    }

    public static IEnumerable<object[]> ArithmeticInstructionTestsList
    {
        get
        {
            // Unsigned
            yield return [new SimpleInstructionTestCase("addu $t2, $t0, $t1", (GPRegister.Temporary2, 20 + 10), (GPRegister.Temporary0, 20), (GPRegister.Temporary1, 10))];
            yield return [new SimpleInstructionTestCase("addiu $t1, $t0, 10", (GPRegister.Temporary1, 20 + 10), (GPRegister.Temporary0, 20))];
            yield return [new SimpleInstructionTestCase("subu $t2, $t0, $t1", (GPRegister.Temporary2, 30 - 20), (GPRegister.Temporary0, 30), (GPRegister.Temporary1, 20))];

            // Signed (without signs)
            yield return [new SimpleInstructionTestCase("add $t2, $t0, $t1", (GPRegister.Temporary2, 20 + 10), (GPRegister.Temporary0, 20), (GPRegister.Temporary1, 10))];
            yield return [new SimpleInstructionTestCase("addi $t1, $t0, 10", (GPRegister.Temporary1, 20 + 10), (GPRegister.Temporary0, 20))];
            yield return [new SimpleInstructionTestCase("sub $t2, $t0, $t1", (GPRegister.Temporary2, 30 - 20), (GPRegister.Temporary0, 30), (GPRegister.Temporary1, 20))];
            //yield return [new SimpleInstructionTestCase("mul $t2, $t0, $t1", (GPRegister.Temporary2, 30 * 20), (GPRegister.Temporary0, 30), (GPRegister.Temporary1, 20))];
            yield return [new SimpleInstructionTestCase("sra $t1, $t0, 4", (GPRegister.Temporary1, 101 >> 4), (GPRegister.Temporary0, 101))];
            yield return [new SimpleInstructionTestCase("srav $t2, $t0, $t1", (GPRegister.Temporary2, 101 >> 4), (GPRegister.Temporary0, 101), (GPRegister.Temporary1, 4))];

            // Signed (with signs)
            unchecked
            {
                yield return [new SimpleInstructionTestCase("add $t2, $t0, $t1", (GPRegister.Temporary2, 30 + (-10)), (GPRegister.Temporary0, 30), (GPRegister.Temporary1, (uint)-10))];
                yield return [new SimpleInstructionTestCase("addi $t1, $t0, -10", (GPRegister.Temporary1, 30 + (-10)), (GPRegister.Temporary0, 30))];
                yield return [new SimpleInstructionTestCase("sub $t2, $t0, $t1", (GPRegister.Temporary2, 20 - (-10)), (GPRegister.Temporary0, 20), (GPRegister.Temporary1, (uint)-10))];
            }

            // Overflowing
            unchecked
            {
                // Unsigned (should not overflow)
                yield return [new SimpleInstructionTestCase("addu $t2, $t0, $t1", (GPRegister.Temporary2, uint.MaxValue + 1), (GPRegister.Temporary0, uint.MaxValue), (GPRegister.Temporary1, 1))];
                yield return [new SimpleInstructionTestCase("addiu $t1, $t0, 1", (GPRegister.Temporary1, uint.MaxValue + 1), (GPRegister.Temporary0, uint.MaxValue))];
                yield return [new SimpleInstructionTestCase("subu $t2, $t0, $t1", (GPRegister.Temporary2, uint.MinValue - 1), (GPRegister.Temporary0, uint.MinValue), (GPRegister.Temporary1, 1))];

                // Signed (without signs)
                yield return [new SimpleInstructionTestCase("add $t2, $t0, $t1", TrapKind.ArithmeticOverflow, (GPRegister.Temporary0, int.MaxValue), (GPRegister.Temporary1, 1))];
                yield return [new SimpleInstructionTestCase("addi $t1, $t0, 1", TrapKind.ArithmeticOverflow, (GPRegister.Temporary0, int.MaxValue))];
                yield return [new SimpleInstructionTestCase("sub $t2, $t0, $t1", TrapKind.ArithmeticOverflow, (GPRegister.Temporary0, (uint)int.MinValue), (GPRegister.Temporary1, 1))];

                // Signed (with signs)
                yield return [new SimpleInstructionTestCase("add $t2, $t0, $t1", TrapKind.ArithmeticOverflow, (GPRegister.Temporary0, (uint)int.MinValue), (GPRegister.Temporary1, (uint)-1))];
                yield return [new SimpleInstructionTestCase("addi $t1, $t0, -1", TrapKind.ArithmeticOverflow, (GPRegister.Temporary0, (uint)int.MinValue))];
                yield return [new SimpleInstructionTestCase("sub $t2, $t0, $t1", TrapKind.ArithmeticOverflow, (GPRegister.Temporary0, int.MaxValue), (GPRegister.Temporary1, (uint)-1))];
            }
        }
    }

    public static IEnumerable<object[]> LogicalInstructionTestsList
    {
        get
        {
            yield return [new SimpleInstructionTestCase("and $t2, $t0, $t1", (GPRegister.Temporary2, 0xbd0 & 0xd16), (GPRegister.Temporary0, 0xbd0), (GPRegister.Temporary1, 0xd16))];
            yield return [new SimpleInstructionTestCase("andi $t1, $t0, 0xd16", (GPRegister.Temporary1, 0xbd0 & 0xd16), (GPRegister.Temporary0, 0xbd0))];
            yield return [new SimpleInstructionTestCase("or $t2, $t0, $t1", (GPRegister.Temporary2, 0xbd0 | 0xd16), (GPRegister.Temporary0, 0xbd0), (GPRegister.Temporary1, 0xd16))];
            yield return [new SimpleInstructionTestCase("ori $t1, $t0, 0xd16", (GPRegister.Temporary1, 0xbd0 | 0xd16), (GPRegister.Temporary0, 0xbd0))];
            yield return [new SimpleInstructionTestCase("xor $t2, $t0, $t1", (GPRegister.Temporary2, 0xbd0 ^ 0xd16), (GPRegister.Temporary0, 0xbd0), (GPRegister.Temporary1, 0xd16))];
            yield return [new SimpleInstructionTestCase("xori $t1, $t0, 0xd16", (GPRegister.Temporary1, 0xbd0 ^ 0xd16), (GPRegister.Temporary0, 0xbd0))];
            yield return [new SimpleInstructionTestCase("nor $t2, $t0, $t1", (GPRegister.Temporary2, ~((uint)0xbd0 | 0xd16)), (GPRegister.Temporary0, 0xbd0), (GPRegister.Temporary1, 0xd16))];
            yield return [new SimpleInstructionTestCase("sll $t1, $t0, 4", (GPRegister.Temporary1, 101 << 4), (GPRegister.Temporary0, 101))];
            yield return [new SimpleInstructionTestCase("srl $t1, $t0, 4", (GPRegister.Temporary1, 101 >> 4), (GPRegister.Temporary0, 101))];
            yield return [new SimpleInstructionTestCase("sllv $t2, $t0, $t1", (GPRegister.Temporary2, 101 << 4), (GPRegister.Temporary0, 101), (GPRegister.Temporary1, 4))];
            yield return [new SimpleInstructionTestCase("srlv $t2, $t0, $t1", (GPRegister.Temporary2, 101 >> 4), (GPRegister.Temporary0, 101), (GPRegister.Temporary1, 4))];

            // TODO: Test/Handle overflow behavior
        }
    }

    public static IEnumerable<object[]> CompareInstructionTestsList
    {
        get
        {
            // Unsigned
            yield return [new SimpleInstructionTestCase("sltu $t2, $t0, $t1", (GPRegister.Temporary2, 1), (GPRegister.Temporary0, 20), (GPRegister.Temporary1, 30))];
            yield return [new SimpleInstructionTestCase("sltu $t2, $t0, $t1", (GPRegister.Temporary2, 0), (GPRegister.Temporary0, 30), (GPRegister.Temporary1, 20))];
            yield return [new SimpleInstructionTestCase("sltiu $t1, $t0, 30", (GPRegister.Temporary1, 1), (GPRegister.Temporary0, 20))];
            yield return [new SimpleInstructionTestCase("sltiu $t1, $t0, 20", (GPRegister.Temporary1, 0), (GPRegister.Temporary0, 30))];

            // Signed (without signs)
            yield return [new SimpleInstructionTestCase("slt $t2, $t0, $t1", (GPRegister.Temporary2, 1), (GPRegister.Temporary0, 20), (GPRegister.Temporary1, 30))];
            yield return [new SimpleInstructionTestCase("slt $t2, $t0, $t1", (GPRegister.Temporary2, 0), (GPRegister.Temporary0, 30), (GPRegister.Temporary1, 20))];
            yield return [new SimpleInstructionTestCase("slti $t1, $t0, 30", (GPRegister.Temporary1, 1), (GPRegister.Temporary0, 20))];
            yield return [new SimpleInstructionTestCase("slti $t1, $t0, 20", (GPRegister.Temporary1, 0), (GPRegister.Temporary0, 30))];

            // Signed (with signs)
            unchecked
            {
                yield return [new SimpleInstructionTestCase("slt $t2, $t0, $t1", (GPRegister.Temporary2, 1), (GPRegister.Temporary0, (uint)-30), (GPRegister.Temporary1, (uint)-20))];
                yield return [new SimpleInstructionTestCase("slt $t2, $t0, $t1", (GPRegister.Temporary2, 0), (GPRegister.Temporary0, (uint)-20), (GPRegister.Temporary1, (uint)-30))];
                yield return [new SimpleInstructionTestCase("slti $t1, $t0, -20", (GPRegister.Temporary1, 1), (GPRegister.Temporary0, (uint)-30))];
                yield return [new SimpleInstructionTestCase("slti $t1, $t0, -30", (GPRegister.Temporary1, 0), (GPRegister.Temporary0, (uint)-20))];
            }
        }
    }

    public static IEnumerable<object[]> UncategorizedRegisterOnlyInstructionTestsList
    {
        get
        {
            // lui
            yield return [new SimpleInstructionTestCase("lui $t0, 0x1234", (GPRegister.Temporary0, 0x12340000))];

            // Niche bit-manipulation
            // TODO: ext, ins, seb, seh, wsbh, wshd
            yield return [new SimpleInstructionTestCase("clz $t1, $t0", (GPRegister.Temporary1, (uint)BitOperations.LeadingZeroCount(0x0080_a244)), (GPRegister.Temporary0, 0x0080_a244))];
            yield return [new SimpleInstructionTestCase("clo $t1, $t0", (GPRegister.Temporary1, (uint)BitOperations.LeadingZeroCount(~(uint)0xFF80_a244)), (GPRegister.Temporary0, 0xFF80_a244))];
        }
    }

    public static IEnumerable<object[]> SystemInstructionTestsList
    {
        get
        {
            yield return [new SimpleInstructionTestCase("syscall", TrapKind.Syscall)];
            yield return [new SimpleInstructionTestCase("break", TrapKind.Break)];
        }
    }

    [DataTestMethod]
    [DynamicData(nameof(ArithmeticInstructionTestsList))]
    public void ArithmeticInstructionTests(SimpleInstructionTestCase @case)
        => RunTest(@case.Input, @case.Expected, @case.Trap, @case.RegisterInitialization);

    [DataTestMethod]
    [DynamicData(nameof(LogicalInstructionTestsList))]
    public void LogicalInstructionTests(SimpleInstructionTestCase @case)
        => RunTest(@case.Input, @case.Expected, @case.Trap, @case.RegisterInitialization);

    [DataTestMethod]
    [DynamicData(nameof(CompareInstructionTestsList))]
    public void CompareInstructionTests(SimpleInstructionTestCase @case)
        => RunTest(@case.Input, @case.Expected, @case.Trap, @case.RegisterInitialization);

    [DataTestMethod]
    [DynamicData(nameof(UncategorizedRegisterOnlyInstructionTestsList))]
    public void UncategorizedRegisterOnlyInstructionTests(SimpleInstructionTestCase @case)
        => RunTest(@case.Input, @case.Expected, @case.Trap, @case.RegisterInitialization);

    [DataTestMethod]
    [DynamicData(nameof(SystemInstructionTestsList))]
    public void SystemInstructionTests(SimpleInstructionTestCase @case)
        => RunTest(@case.Input, @case.Expected, @case.Trap, @case.RegisterInitialization);

    private static void RunTest(string line, (GPRegister, uint)? check, TrapKind expectedTrap = TrapKind.None, params (GPRegister, uint)[] regInits)
    {
        // The instruction parser is only used to convert the instruction string into an Instruction struct, so we can test the interpreter with it.
        var tokenized = Tokenizer.TokenizeLine(line);
        var table = new InstructionTable(new());
        var parser = new InstructionParser(table, null);
        if (!parser.TryParse(tokenized, out var parsed))
            Assert.Fail();

        // TODO: Psuedo instruction support
        var instruction = parsed.Realize()[0];

        // Initialize the register file with the provided values
        var interpreter = new Interpreter();
        foreach (var (reg, value) in regInits)
            interpreter.SetRegister(reg, value);

        interpreter.InsertInstructionExecution(instruction, out var execution);

        if (check is not null)
        {
            // Ensure that the expected register was written to with the expected value
            Assert.AreEqual(check.Value.Item2, interpreter.GetRegister(check.Value.Item1));
        }
        else
        {
            // If no register check was provided, we at least want to make sure no register was written to (as that would be unexpected)
            Assert.IsNull(execution.Destination);
        }

        // Ensure that the expected trap was raised (if any)
        Assert.AreEqual(expectedTrap, execution.Trap);
    }
}
