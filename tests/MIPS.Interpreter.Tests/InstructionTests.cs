// Avishai Dernis 2026

using MIPS.Assembler.Models.Instructions;
using MIPS.Assembler.Parsers;
using MIPS.Assembler.Tokenization;
using MIPS.Interpreter.Models.System.Execution.Enum;
using MIPS.Models.Instructions.Enums.Registers;
using System.Collections.Generic;
using System.Numerics;

namespace MIPS.Interpreter.Tests;

[TestClass]
public class InstructionTests
{
    public sealed record SimpleInstructionTestCase
    {
        public SimpleInstructionTestCase(string input, (GPRegister, uint) expected, params (GPRegister, uint)[] registerInit)
        {
            Input = input;
            ExpectedWriteBack = expected;
            RegisterInitialization = registerInit;
        }

        public SimpleInstructionTestCase(string input, TrapKind trap, params (GPRegister, uint)[] registerInit)
        {
            Input = input;
            ExpectedTrap = trap;
            RegisterInitialization = registerInit;
        }

        public SimpleInstructionTestCase(string input, (uint, uint) highLow, params (GPRegister, uint)[] registerInit)
        {
            Input = input;
            ExpectedHighLow = highLow;
            RegisterInitialization = registerInit;
        }

        public SimpleInstructionTestCase(string input, ulong highLow, params (GPRegister, uint)[] registerInit)
        {
            Input = input;
            ExpectedHighLow = ((uint)(highLow >> 32), (uint)highLow);
            RegisterInitialization = registerInit;
        }

        public string Input { get; }

        public (GPRegister Regiter, uint Value)? ExpectedWriteBack { get; init; } = null;

        public TrapKind ExpectedTrap { get; init; } = TrapKind.None;

        public (uint High, uint Low)? ExpectedHighLow { get; init; }

        public (uint High, uint Low)? InitialHighLow { get; init; }

        public (GPRegister Register, uint Value)[] RegisterInitialization { get; } = [];
    }

    public static IEnumerable<object[]> ArithmeticInstructionTestsList
    {
        get
        {
            // Unsigned
            yield return [new SimpleInstructionTestCase("addu $t2, $t0, $t1", (GPRegister.Temporary2, 20 + 10), (GPRegister.Temporary0, 20), (GPRegister.Temporary1, 10))];
            yield return [new SimpleInstructionTestCase("addiu $t1, $t0, 10", (GPRegister.Temporary1, 20 + 10), (GPRegister.Temporary0, 20))];
            yield return [new SimpleInstructionTestCase("subu $t2, $t0, $t1", (GPRegister.Temporary2, 30 - 20), (GPRegister.Temporary0, 30), (GPRegister.Temporary1, 20))];
            yield return [new SimpleInstructionTestCase("multu $t0, $t1", 30 * 20, (GPRegister.Temporary0, 30), (GPRegister.Temporary1, 20))];
            yield return [new SimpleInstructionTestCase("divu $t0, $t1", highLow: (30 % 20, 30 / 20), (GPRegister.Temporary0, 30), (GPRegister.Temporary1, 20))];

            // Signed (without signs)
            yield return [new SimpleInstructionTestCase("add $t2, $t0, $t1", (GPRegister.Temporary2, 20 + 10), (GPRegister.Temporary0, 20), (GPRegister.Temporary1, 10))];
            yield return [new SimpleInstructionTestCase("addi $t1, $t0, 10", (GPRegister.Temporary1, 20 + 10), (GPRegister.Temporary0, 20))];
            yield return [new SimpleInstructionTestCase("sub $t2, $t0, $t1", (GPRegister.Temporary2, 30 - 20), (GPRegister.Temporary0, 30), (GPRegister.Temporary1, 20))];
            yield return [new SimpleInstructionTestCase("mul $t2, $t0, $t1", (GPRegister.Temporary2, 30 * 20), (GPRegister.Temporary0, 30), (GPRegister.Temporary1, 20))];
            yield return [new SimpleInstructionTestCase("mult $t0, $t1", 30 * 20, (GPRegister.Temporary0, 30), (GPRegister.Temporary1, 20))];
            yield return [new SimpleInstructionTestCase("div $t0, $t1", highLow: (30 % 20, 30 / 20), (GPRegister.Temporary0, 30), (GPRegister.Temporary1, 20))];
            yield return [new SimpleInstructionTestCase("sra $t1, $t0, 4", (GPRegister.Temporary1, 101 >> 4), (GPRegister.Temporary0, 101))];
            yield return [new SimpleInstructionTestCase("srav $t2, $t0, $t1", (GPRegister.Temporary2, 101 >> 4), (GPRegister.Temporary0, 101), (GPRegister.Temporary1, 4))];

            // Signed (with signs)
            unchecked
            {
                yield return [new SimpleInstructionTestCase("add $t2, $t0, $t1", (GPRegister.Temporary2, 30 + (-10)), (GPRegister.Temporary0, 30), (GPRegister.Temporary1, (uint)-10))];
                yield return [new SimpleInstructionTestCase("addi $t1, $t0, -10", (GPRegister.Temporary1, 30 + (-10)), (GPRegister.Temporary0, 30))];
                yield return [new SimpleInstructionTestCase("sub $t2, $t0, $t1", (GPRegister.Temporary2, 20 - (-10)), (GPRegister.Temporary0, 20), (GPRegister.Temporary1, (uint)-10))];
                yield return [new SimpleInstructionTestCase("mul $t2, $t0, $t1", (GPRegister.Temporary2, (uint)(30 * -20)), (GPRegister.Temporary0, 30), (GPRegister.Temporary1, (uint)-20))];
                yield return [new SimpleInstructionTestCase("mult $t0, $t1", (ulong)(30 * -20), (GPRegister.Temporary0, 30), (GPRegister.Temporary1, (uint)-20))];
                yield return [new SimpleInstructionTestCase("div $t0, $t1", highLow: (30 % -20, (uint)(30 / -20)), (GPRegister.Temporary0, 30), (GPRegister.Temporary1, (uint)-20))];
            }

            // Overflowing
            unchecked
            {
                // Unsigned (should not overflow)
                yield return [new SimpleInstructionTestCase("addu $t2, $t0, $t1", (GPRegister.Temporary2, uint.MaxValue + 1), (GPRegister.Temporary0, uint.MaxValue), (GPRegister.Temporary1, 1))];
                yield return [new SimpleInstructionTestCase("addiu $t1, $t0, 1", (GPRegister.Temporary1, uint.MaxValue + 1), (GPRegister.Temporary0, uint.MaxValue))];
                yield return [new SimpleInstructionTestCase("subu $t2, $t0, $t1", (GPRegister.Temporary2, uint.MinValue - 1), (GPRegister.Temporary0, uint.MinValue), (GPRegister.Temporary1, 1))];
                yield return [new SimpleInstructionTestCase("multu $t0, $t1", (ulong)uint.MaxValue * uint.MaxValue, (GPRegister.Temporary0, uint.MaxValue), (GPRegister.Temporary1, uint.MaxValue))];
                yield return [new SimpleInstructionTestCase("divu $t0, $t1", highLow: (uint.MaxValue % uint.MaxValue, uint.MaxValue / uint.MaxValue), (GPRegister.Temporary0, uint.MaxValue), (GPRegister.Temporary1, uint.MaxValue))];

                // Note:
                // "mul" does not trap on overflow. We expect the low 32 bits of the result to be written back, and the high 32 bits to be discarded.
                // "mult" also does not trap on overflow, but instead writes the full 64-bit result into the high and low registers.
                // "div" does not trap on overflow either. The behavior is undefined if the quotient is too large to fit in 32 bits.
                // In practice, we will just take the low 32 bits of the quotient and discard the high 32 bits, and write the remainder to the high register.

                // Signed (without signs)
                yield return [new SimpleInstructionTestCase("add $t2, $t0, $t1", TrapKind.ArithmeticOverflow, (GPRegister.Temporary0, int.MaxValue), (GPRegister.Temporary1, 1))];
                yield return [new SimpleInstructionTestCase("addi $t1, $t0, 1", TrapKind.ArithmeticOverflow, (GPRegister.Temporary0, int.MaxValue))];
                yield return [new SimpleInstructionTestCase("sub $t2, $t0, $t1", TrapKind.ArithmeticOverflow, (GPRegister.Temporary0, (uint)int.MinValue), (GPRegister.Temporary1, 1))];
                yield return [new SimpleInstructionTestCase("mul $t1, $t0, $t0", (GPRegister.Temporary1, int.MaxValue * int.MaxValue), (GPRegister.Temporary0, int.MaxValue))];
                yield return [new SimpleInstructionTestCase("mult $t0, $t0", (long)int.MaxValue * int.MaxValue, (GPRegister.Temporary0, int.MaxValue))];
                yield return [new SimpleInstructionTestCase("div $t0, $t0", highLow: ((uint)((long)int.MaxValue % int.MaxValue), (uint)((long)int.MaxValue / int.MaxValue)), (GPRegister.Temporary0, int.MaxValue))];

                // Signed (with signs)
                yield return [new SimpleInstructionTestCase("add $t2, $t0, $t1", TrapKind.ArithmeticOverflow, (GPRegister.Temporary0, (uint)int.MinValue), (GPRegister.Temporary1, (uint)-1))];
                yield return [new SimpleInstructionTestCase("addi $t1, $t0, -1", TrapKind.ArithmeticOverflow, (GPRegister.Temporary0, (uint)int.MinValue))];
                yield return [new SimpleInstructionTestCase("sub $t2, $t0, $t1", TrapKind.ArithmeticOverflow, (GPRegister.Temporary0, int.MaxValue), (GPRegister.Temporary1, (uint)-1))];
                yield return [new SimpleInstructionTestCase("mul $t1, $t0, $t0", (GPRegister.Temporary2, int.MinValue * int.MinValue), (GPRegister.Temporary0, (uint)int.MinValue))];
                yield return [new SimpleInstructionTestCase("mult $t0, $t0", (long)int.MinValue * int.MinValue, (GPRegister.Temporary0, (uint)int.MinValue))];
                yield return [new SimpleInstructionTestCase("div $t0, $t0", highLow: ((uint)((long)int.MinValue % int.MinValue), (uint)((long)int.MinValue / int.MinValue)), (GPRegister.Temporary0, (uint)int.MinValue))];
            }

            // Division by zero. Undefined behavior, but NOT a trap! (Shouldn't crash the emulator either)
            yield return [new SimpleInstructionTestCase("divu $t0, $t1", trap: TrapKind.None, (GPRegister.Temporary0, 30), (GPRegister.Temporary1, 0))];
            yield return [new SimpleInstructionTestCase("div $t0, $t1", trap: TrapKind.None, (GPRegister.Temporary0, 30), (GPRegister.Temporary1, 0))];

            // Multiply and Add/Subtract
            yield return [new SimpleInstructionTestCase("maddu $t0, $t1", highLow: (1, 1024 + (30 * 20)), (GPRegister.Temporary0, 30), (GPRegister.Temporary1, 20), (GPRegister.Temporary2, 1))
            {
                    InitialHighLow = (1, 1024)
            }];
            yield return [new SimpleInstructionTestCase("madd $t0, $t1", highLow: (1, 1024 + (30 * 20)), (GPRegister.Temporary0, 30), (GPRegister.Temporary1, 20), (GPRegister.Temporary2, 1))
            {
                    InitialHighLow = (1, 1024)
            }];
            yield return [new SimpleInstructionTestCase("msubu $t0, $t1", highLow: (1, 1024 - (30 * 20)), (GPRegister.Temporary0, 30), (GPRegister.Temporary1, 20), (GPRegister.Temporary2, 1))
            {
                    InitialHighLow = (1, 1024)
            }];
            yield return [new SimpleInstructionTestCase("msub $t0, $t1", highLow: (1, 1024 - (30 * 20)), (GPRegister.Temporary0, 30), (GPRegister.Temporary1, 20), (GPRegister.Temporary2, 1))
            {
                    InitialHighLow = (1, 1024)
            }];
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
            yield return [new SimpleInstructionTestCase("sltu $t1, $t0, $t0", (GPRegister.Temporary1, 0), (GPRegister.Temporary0, 10))];
            yield return [new SimpleInstructionTestCase("sltiu $t1, $t0, 30", (GPRegister.Temporary1, 1), (GPRegister.Temporary0, 20))];
            yield return [new SimpleInstructionTestCase("sltiu $t1, $t0, 20", (GPRegister.Temporary1, 0), (GPRegister.Temporary0, 30))];
            yield return [new SimpleInstructionTestCase("sltiu $t1, $t0, 10", (GPRegister.Temporary1, 0), (GPRegister.Temporary0, 10))];

            // Signed (without signs)
            yield return [new SimpleInstructionTestCase("slt $t2, $t0, $t1", (GPRegister.Temporary2, 1), (GPRegister.Temporary0, 20), (GPRegister.Temporary1, 30))];
            yield return [new SimpleInstructionTestCase("slt $t2, $t0, $t1", (GPRegister.Temporary2, 0), (GPRegister.Temporary0, 30), (GPRegister.Temporary1, 20))];
            yield return [new SimpleInstructionTestCase("slt $t1, $t0, $t0", (GPRegister.Temporary1, 0), (GPRegister.Temporary0, 10))];
            yield return [new SimpleInstructionTestCase("slti $t1, $t0, 30", (GPRegister.Temporary1, 1), (GPRegister.Temporary0, 20))];
            yield return [new SimpleInstructionTestCase("slti $t1, $t0, 20", (GPRegister.Temporary1, 0), (GPRegister.Temporary0, 30))];
            yield return [new SimpleInstructionTestCase("slti $t1, $t0, 10", (GPRegister.Temporary1, 0), (GPRegister.Temporary0, 10))];

            // Signed (with signs)
            unchecked
            {
                yield return [new SimpleInstructionTestCase("slt $t2, $t0, $t1", (GPRegister.Temporary2, 1), (GPRegister.Temporary0, (uint)-30), (GPRegister.Temporary1, (uint)-20))];
                yield return [new SimpleInstructionTestCase("slt $t2, $t0, $t1", (GPRegister.Temporary2, 0), (GPRegister.Temporary0, (uint)-20), (GPRegister.Temporary1, (uint)-30))];
                yield return [new SimpleInstructionTestCase("slt $t1, $t0, $t0", (GPRegister.Temporary2, 0), (GPRegister.Temporary0, (uint)-10))];
                yield return [new SimpleInstructionTestCase("slti $t1, $t0, -20", (GPRegister.Temporary1, 1), (GPRegister.Temporary0, (uint)-30))];
                yield return [new SimpleInstructionTestCase("slti $t1, $t0, -30", (GPRegister.Temporary1, 0), (GPRegister.Temporary0, (uint)-20))];
                yield return [new SimpleInstructionTestCase("slti $t1, $t0, -10", (GPRegister.Temporary1, 0), (GPRegister.Temporary0, (uint)-10))];
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
            yield return [new SimpleInstructionTestCase("break", TrapKind.Breakpoint)];
        }
    }

    [DataTestMethod]
    [DynamicData(nameof(ArithmeticInstructionTestsList))]
    public void ArithmeticInstructionTests(SimpleInstructionTestCase @case) => RunTest(@case);

    [DataTestMethod]
    [DynamicData(nameof(LogicalInstructionTestsList))]
    public void LogicalInstructionTests(SimpleInstructionTestCase @case) => RunTest(@case);

    [DataTestMethod]
    [DynamicData(nameof(CompareInstructionTestsList))]
    public void CompareInstructionTests(SimpleInstructionTestCase @case) => RunTest(@case);

    [DataTestMethod]
    [DynamicData(nameof(UncategorizedRegisterOnlyInstructionTestsList))]
    public void UncategorizedRegisterOnlyInstructionTests(SimpleInstructionTestCase @case) => RunTest(@case);

    [DataTestMethod]
    [DynamicData(nameof(SystemInstructionTestsList))]
    public void SystemInstructionTests(SimpleInstructionTestCase @case) => RunTest(@case);

    private static void RunTest(SimpleInstructionTestCase @case)
    {
        // The instruction parser is only used to convert the instruction string into an Instruction struct, so we can test the interpreter with it.
        var tokenized = Tokenizer.TokenizeLine(@case.Input);
        var table = new InstructionTable(new());
        var parser = new InstructionParser(table, null);
        if (!parser.TryParse(tokenized, out var parsed))
            Assert.Fail();

        // TODO: Psuedo instruction support
        var instruction = parsed.Realize()[0];

        // Initialize the register file with the provided values
        var interpreter = new Interpreter();
        foreach (var (reg, value) in @case.RegisterInitialization)
            interpreter.Computer.Processor[reg] = value;

        // Initialize the high and low registers if specified in the test case
        if (@case.InitialHighLow.HasValue)
        {
            interpreter.Computer.Processor.HighLow = @case.InitialHighLow.Value;
        }

        interpreter.InsertInstructionExecution(instruction, out var execution);

        var writeback = @case.ExpectedWriteBack;
        if (writeback.HasValue)
        {
            // Ensure that the expected register was written to with the expected value
            Assert.AreEqual(writeback.Value.Value, interpreter.Computer.Processor[writeback.Value.Regiter]);
        }
        else
        {
            // If no register check was provided, we at least want to make sure no register was written to (as that would be unexpected)
            Assert.IsNull(execution.Destination);
        }

        var highLow = @case.ExpectedHighLow;
        if (highLow.HasValue)
        {
            Assert.AreEqual(highLow.Value, interpreter.Computer.Processor.HighLow);
        }

        // Ensure that the expected trap was raised (if any)
        Assert.AreEqual(@case.ExpectedTrap, execution.Trap);
    }
}
