// Avishai Dernis 2026

using MIPS.Assembler.Models.Instructions;
using MIPS.Assembler.Parsers;
using MIPS.Assembler.Tokenization;
using MIPS.Emulator.Models.System.CPU.Registers;
using MIPS.Emulator.Models.System.Execution.Enum;
using MIPS.Models.Instructions.Enums.Registers;
using MIPS.Models.Instructions.Enums.SpecialFunctions.CoProc0;
using System;
using System.Collections.Generic;
using System.Numerics;

namespace MIPS.Emulator.Tests;

[TestClass]
public class ExecutionTests
{
    private const uint K0 = 0xbd0;
    private const uint K1 = 0xd16;

    public sealed record SimpleInstructionTestCase
    {
        private SimpleInstructionTestCase(string input)
        {
            Input = input;

            PrivilegeMode = PrivilegeMode.User;

            unchecked
            {
                RegisterInitialization =
                    [
                        // Max/Min values to test edge cases, as well as some arbitrary non-edge-case values for good measure
                        // Stored in the argument registers
                        (GPRegister.Argument0, int.MaxValue), (GPRegister.Argument1, (uint)int.MinValue),
                        (GPRegister.Argument2, uint.MaxValue), (GPRegister.Argument3, uint.MinValue),

                        // Saved 1 - 4 are assigned to 1 through 4 respectively,
                        // while saved 5 and 6 are assigned to -1 and -2 (to test sign handling in arithmetic instructions)
                        (GPRegister.Saved1, 1), (GPRegister.Saved2, 2), (GPRegister.Saved3, 3), (GPRegister.Saved4, 4),
                        (GPRegister.Saved5, (uint)-1), (GPRegister.Saved6, (uint)-2),

                        // Temp 1 - 4 are assigned to 10, 20, 30, 40 respectively,
                        // while temp 5 and 6 are assigned to -10 and -20 (to test sign handling in arithmetic instructions)
                        (GPRegister.Temporary1, 10), (GPRegister.Temporary2, 20), (GPRegister.Temporary3, 30), (GPRegister.Temporary4, 40),
                        (GPRegister.Temporary5, (uint)-10), (GPRegister.Temporary6, (uint)-20), (GPRegister.Temporary7, (uint)-30),

                        // Assign some arbitrary values to the rest of the registers as well, just in case
                        (GPRegister.Temporary8, 101), (GPRegister.AssemblerTemporary, 0x89ab_cdef), (GPRegister.Kernel0, K0), (GPRegister.Kernel1, K1)
                    ];

                InitialHighLow = (0x1234, 0x5678);

                MemoryInitialization =
                    [(0x1000, [0x12, 0x34, 0x56, 0x78])];
            }
        }

        public SimpleInstructionTestCase(string input, TrapKind trap) : this(input)
        {
            ExpectedTrap = trap;
        }

        public SimpleInstructionTestCase(string input, uint writeBack) : this(input)
        {
            ExpectedWriteBack = (GPRegister.ReturnValue0, writeBack);
        }

        public SimpleInstructionTestCase(string input, GPRegister reg, uint? writeBack = null) : this(input)
        {
            ExpectedWriteBack = (reg, writeBack);
        }

        public SimpleInstructionTestCase(string input, (uint, byte[]) memory) : this(input)
        {
            ExpectedMemory = memory;
        }

        public SimpleInstructionTestCase(string input, ulong highLow) : this(input)
        {
            ExpectedHighLow = ((uint)(highLow >> 32), (uint)highLow);
        }

        public SimpleInstructionTestCase(string input, (uint, uint) highLow) : this(input)
        {
            ExpectedHighLow = highLow;
        }

        public SimpleInstructionTestCase(string input, SecondaryEffect sideEffects) : this(input)
        {
            ExpectedSideEffects = sideEffects;
        }

        public string Input { get; }

        public TrapKind ExpectedTrap { get; init; } = TrapKind.None;

        public (GPRegister Regiter, uint? Value)? ExpectedWriteBack { get; init; } = null;

        public SecondaryEffect? ExpectedSideEffects { get; init; }

        public (uint Address, byte[] Data)? ExpectedMemory { get; init; }

        public (uint High, uint Low)? ExpectedHighLow { get; init; }

        public (GPRegister Register, uint Value)[] RegisterInitialization { get; init; } = [];

        public (uint Address, byte[] Data)[] MemoryInitialization { get; init; } = [];

        public (uint High, uint Low)? InitialHighLow { get; init; }

        public PrivilegeMode PrivilegeMode
        {
            get => Status.PrivilegeMode;
            init => Status = Status with { PrivilegeMode = value };
        }

        public StatusRegister Status { get; init; }
    }

    public static IEnumerable<object[]> ArithmeticInstructionTestsList
    {
        get
        {
            // Unsigned
            yield return [new SimpleInstructionTestCase("addu $v0, $t2, $t1", 30)];
            yield return [new SimpleInstructionTestCase("addiu $v0, $t2, 10", 30)];
            yield return [new SimpleInstructionTestCase("subu $v0, $t3, $t2", 30 - 20)];
            yield return [new SimpleInstructionTestCase("multu $t3, $t2", (ulong)(30 * 20))];
            yield return [new SimpleInstructionTestCase("divu $t3, $t2", (30 % 20, 30 / 20))];

            // Signed (without signs)
            yield return [new SimpleInstructionTestCase("add $v0, $t2, $t1", 30)];
            yield return [new SimpleInstructionTestCase("addi $v0, $t2, 10", 30)];
            yield return [new SimpleInstructionTestCase("sub $v0, $t3, $t2", 30 - 20)];
            yield return [new SimpleInstructionTestCase("mul $v0, $t3, $t2", 30 * 20)];
            yield return [new SimpleInstructionTestCase("mult $t3, $t2", (ulong)(30 * 20))];
            yield return [new SimpleInstructionTestCase("div $t3, $t2", (30 % 20, 30 / 20))];
            yield return [new SimpleInstructionTestCase("sra $v0, $t8, 4", 101 >> 4)];
            yield return [new SimpleInstructionTestCase("srav $v0, $t8, $s4", 101 >> 4)];

            // Signed (with signs)
            unchecked
            {
                yield return [new SimpleInstructionTestCase("add $v0, $t3, $t5", 30 + (-10))];
                yield return [new SimpleInstructionTestCase("addi $v0, $t3, -10", 30 + (-10))];
                yield return [new SimpleInstructionTestCase("sub $v0, $t2, $t5", 20 - (-10))];
                yield return [new SimpleInstructionTestCase("mul $v0, $t3, $t6", (uint)(30 * -20))];
                yield return [new SimpleInstructionTestCase("mult $t3, $t6", (ulong)(30 * -20))];
                yield return [new SimpleInstructionTestCase("div $t3, $t6", (30 % -20, (uint)(30 / -20)))];
            }

            // Overflowing
            unchecked
            {
                // Unsigned (should not overflow)
                yield return [new SimpleInstructionTestCase("addu $v0, $a2, $s1", uint.MaxValue + 1)];
                yield return [new SimpleInstructionTestCase("addiu $v0, $a2, 1", uint.MaxValue + 1)];
                yield return [new SimpleInstructionTestCase("subu $v0, $a3, $s1", uint.MinValue - 1)];
                yield return [new SimpleInstructionTestCase("multu $a2, $a2", (ulong)uint.MaxValue * uint.MaxValue)];
                yield return [new SimpleInstructionTestCase("divu $a2, $a2", (uint.MaxValue % uint.MaxValue, uint.MaxValue / uint.MaxValue))];

                // Note:
                // "mul" does not trap on overflow. We expect the low 32 bits of the result to be written back, and the high 32 bits to be discarded.
                // "mult" also does not trap on overflow, but instead writes the full 64-bit result into the high and low registers.
                // "div" does not trap on overflow either. The behavior is undefined if the quotient is too large to fit in 32 bits.
                // In practice, we will just take the low 32 bits of the quotient and discard the high 32 bits, and write the remainder to the high register.

                // Signed (without signs)
                yield return [new SimpleInstructionTestCase("add $v0, $a0, $s1", TrapKind.ArithmeticOverflow)];             // max + 1
                yield return [new SimpleInstructionTestCase("addi $v0, $a0, 1", TrapKind.ArithmeticOverflow)];              // max + 1
                yield return [new SimpleInstructionTestCase("sub $v0, $a1, $s1", TrapKind.ArithmeticOverflow)];             // min - 1
                yield return [new SimpleInstructionTestCase("mul $v0, $a0, $a0", int.MaxValue * int.MaxValue)];             // max * max
                yield return [new SimpleInstructionTestCase("mult $a0, $a0", (long)int.MaxValue * int.MaxValue)];           // max * max
                yield return [new SimpleInstructionTestCase("div $a0, $a0", ((uint)((long)int.MaxValue % int.MaxValue), (uint)((long)int.MaxValue / int.MaxValue)))];

                // Signed (with signs)
                yield return [new SimpleInstructionTestCase("add $v0, $a1, $s5", TrapKind.ArithmeticOverflow)];             // min + (-1)
                yield return [new SimpleInstructionTestCase("addi $v0, $a1, -1", TrapKind.ArithmeticOverflow)];             // min + (-1)
                yield return [new SimpleInstructionTestCase("sub $v0, $a0, $s5", TrapKind.ArithmeticOverflow)];             // max - (-1)
                yield return [new SimpleInstructionTestCase("mul $v0, $a1, $a1", (uint)(int.MinValue * int.MinValue))];     // min * min
                yield return [new SimpleInstructionTestCase("mult $a1, $a1", (long)int.MinValue * int.MinValue)];           // min * min
                yield return [new SimpleInstructionTestCase("div $a1, $a1", ((uint)((long)int.MinValue % int.MinValue), (uint)((long)int.MinValue / int.MinValue)))];
            }

            // Division by zero. Undefined behavior, but NOT a trap! (Shouldn't crash the emulator either)
            yield return [new SimpleInstructionTestCase("divu $t3, $zero", TrapKind.None)];
            yield return [new SimpleInstructionTestCase("div $t3, $zero", TrapKind.None)];

            // Multiply and Add/Subtract
            yield return [new SimpleInstructionTestCase("maddu $t3, $t2", (0x1234, 0x5678 + (30 * 20)))];
            yield return [new SimpleInstructionTestCase("madd $t3, $t2", (0x1234, 0x5678 + (30 * 20)))];
            yield return [new SimpleInstructionTestCase("msubu $t3, $t2", (0x1234, 0x5678 - (30 * 20)))];
            yield return [new SimpleInstructionTestCase("msub $t3, $t2", (0x1234, 0x5678 - (30 * 20)))];
        }
    }

    public static IEnumerable<object[]> LogicalInstructionTestsList
    {
        get
        {
            yield return [new SimpleInstructionTestCase("and $v0, $k0, $k1", K0 & K1)];
            yield return [new SimpleInstructionTestCase("andi $v0, $k0, 0xd16", K0 & K1)];
            yield return [new SimpleInstructionTestCase("or $v0, $k0, $k1", K0 | K1)];
            yield return [new SimpleInstructionTestCase("ori $v0, $k0, 0xd16", K0 | K1)];
            yield return [new SimpleInstructionTestCase("xor $v0, $k0, $k1", K0 ^ K1)];
            yield return [new SimpleInstructionTestCase("xori $v0, $k0, 0xd16", K0 ^ K1)];
            yield return [new SimpleInstructionTestCase("nor $v0, $k0, $k1", ~(K0 | K1))];
            yield return [new SimpleInstructionTestCase("sll $v0, $t8, 4", 101 << 4)];
            yield return [new SimpleInstructionTestCase("srl $v0, $t8, 4", 101 >> 4)];
            yield return [new SimpleInstructionTestCase("sllv $v0, $t8, $s4", 101 << 4)];
            yield return [new SimpleInstructionTestCase("srlv $v0, $t8, $s4", 101 >> 4)];
        }
    }

    public static IEnumerable<object[]> CompareInstructionTestsList
    {
        get
        {
            // Unsigned
            yield return [new SimpleInstructionTestCase("sltu $v0, $t2, $t3", 1)];
            yield return [new SimpleInstructionTestCase("sltu $v0, $t3, $t2", (uint)0)];
            yield return [new SimpleInstructionTestCase("sltu $v0, $t1, $t1", (uint)0)];
            yield return [new SimpleInstructionTestCase("sltiu $v0, $t2, 30", 1)];
            yield return [new SimpleInstructionTestCase("sltiu $v0, $t3, 20", (uint)0)];
            yield return [new SimpleInstructionTestCase("sltiu $v0, $t1, 10", (uint)0)];

            // Signed (without signs)
            yield return [new SimpleInstructionTestCase("slt $v0, $t2, $t3", 1)];
            yield return [new SimpleInstructionTestCase("slt $v0, $t3, $t2", (uint)0)];
            yield return [new SimpleInstructionTestCase("slt $v0, $t1, $t1", (uint)0)];
            yield return [new SimpleInstructionTestCase("slti $v0, $t2, 30", 1)];
            yield return [new SimpleInstructionTestCase("slti $v0, $t3, 20", (uint)0)];
            yield return [new SimpleInstructionTestCase("slti $v0, $t1, 10", (uint)0)];

            // Signed (with signs)
            unchecked
            {
                yield return [new SimpleInstructionTestCase("slt $v0, $t7, $t6", 1)];
                yield return [new SimpleInstructionTestCase("slt $v0, $t6, $t7", (uint)0)];
                yield return [new SimpleInstructionTestCase("slt $v0, $t5, $t5", (uint)0)];
                yield return [new SimpleInstructionTestCase("slti $v0, $t7, -20", 1)];
                yield return [new SimpleInstructionTestCase("slti $v0, $t6, -30", (uint)0)];
                yield return [new SimpleInstructionTestCase("slti $v0, $t5, -10", (uint)0)];
            }
        }
    }

    public static IEnumerable<object[]> TrapInstructionTestsList
    {
        get
        {
            // Equality
            yield return [new SimpleInstructionTestCase("teq $t2, $t3", TrapKind.None)];
            yield return [new SimpleInstructionTestCase("teq $t1, $t1", TrapKind.Trap)];
            yield return [new SimpleInstructionTestCase("tne $t1, $t1", TrapKind.None)];
            yield return [new SimpleInstructionTestCase("tne $t3, $t2", TrapKind.Trap)];

            // Unsigned
            yield return [new SimpleInstructionTestCase("tltu $t3, $t2", TrapKind.None)];
            yield return [new SimpleInstructionTestCase("tltu $t2, $t3", TrapKind.Trap)];
            yield return [new SimpleInstructionTestCase("tltu $t1, $t1", TrapKind.None)];
            yield return [new SimpleInstructionTestCase("tgeu $t2, $t3", TrapKind.None)];
            yield return [new SimpleInstructionTestCase("tgeu $t3, $t2", TrapKind.Trap)];
            yield return [new SimpleInstructionTestCase("tgeu $t1, $t1", TrapKind.Trap)];

            // Signed (without signs)
            yield return [new SimpleInstructionTestCase("tlt $t3, $t2", TrapKind.None)];
            yield return [new SimpleInstructionTestCase("tlt $t2, $t3", TrapKind.Trap)];
            yield return [new SimpleInstructionTestCase("tlt $t1, $t1", TrapKind.None)];
            yield return [new SimpleInstructionTestCase("tge $t2, $t3", TrapKind.None)];
            yield return [new SimpleInstructionTestCase("tge $t3, $t2", TrapKind.Trap)];
            yield return [new SimpleInstructionTestCase("tge $t1, $t1", TrapKind.Trap)];

            // Signed (with signs)
            unchecked
            {
                yield return [new SimpleInstructionTestCase("tlt $t6, $t7", TrapKind.None)];
                yield return [new SimpleInstructionTestCase("tlt $t7, $t6", TrapKind.Trap)];
                yield return [new SimpleInstructionTestCase("tlt $t5, $t5", TrapKind.None)];
                yield return [new SimpleInstructionTestCase("tge $t7, $t6", TrapKind.None)];
                yield return [new SimpleInstructionTestCase("tge $t6, $t7", TrapKind.Trap)];
                yield return [new SimpleInstructionTestCase("tge $t5, $t5", TrapKind.Trap)];
            }
        }
    }

    public static IEnumerable<object[]> MemoryInstructionTestsList
    {
        get
        {
            // Load
            yield return [new SimpleInstructionTestCase("lb $v0, 0x1000($zero)", 0x12)];
            yield return [new SimpleInstructionTestCase("lh $v0, 0x1000($zero)", 0x1234)];
            yield return [new SimpleInstructionTestCase("lw $v0, 0x1000($zero)", 0x1234_5678)];

            // Store
            yield return [new SimpleInstructionTestCase("sb $at, 0x1000($zero)", (0x1000, [0x12, 0x34, 0x56, 0xef]))];
            yield return [new SimpleInstructionTestCase("sh $at, 0x1000($zero)", (0x1000, [0x12, 0x34, 0xcd, 0xef]))];
            yield return [new SimpleInstructionTestCase("sw $at, 0x1000($zero)", (0x1000, [0x89, 0xab, 0xcd, 0xef]))];
        }
    }

    public static IEnumerable<object[]> UncategorizedRegisterOnlyInstructionTestsList
    {
        get
        {
            // lui
            yield return [new SimpleInstructionTestCase("lui $v0, 0x1234", 0x12340000)];

            // Move from/to high and low registers
            yield return [new SimpleInstructionTestCase("mtlo $k0", (0x1234, K0))];
            yield return [new SimpleInstructionTestCase("mthi $k1", (K1, 0x5678))];
            yield return [new SimpleInstructionTestCase("mflo $v0", 0x5678)];
            yield return [new SimpleInstructionTestCase("mfhi $v0", 0x1234)];

            // Niche bit-manipulation
            // TODO: ext, ins, seb, seh, wsbh, wshd
            yield return [new SimpleInstructionTestCase("clz $v0, $k0", (uint)BitOperations.LeadingZeroCount(K0))];
            yield return [new SimpleInstructionTestCase("clo $v0, $k0", (uint)BitOperations.LeadingZeroCount(~K0))];
        }
    }

    public static IEnumerable<object[]> SystemInstructionTestsList
    {
        get
        {
            yield return [new SimpleInstructionTestCase("syscall", TrapKind.Syscall)];
            yield return [new SimpleInstructionTestCase("break", TrapKind.Breakpoint)];

            // Exception Return
            yield return [new SimpleInstructionTestCase("eret", TrapKind.ReservedInstruction)];
            yield return [new SimpleInstructionTestCase("eret", SecondaryEffect.WriteCoProc)
            { Status = new StatusRegister { ExceptionLevel = true } }];

            // Enable Interrupts
            yield return [new SimpleInstructionTestCase("ei", TrapKind.ReservedInstruction)];
            yield return [new SimpleInstructionTestCase("ei", SecondaryEffect.WriteCoProc)
            { PrivilegeMode = PrivilegeMode.Kernel }];
            yield return [new SimpleInstructionTestCase("ei $v0", GPRegister.ReturnValue0)
            {
                ExpectedSideEffects = SecondaryEffect.WriteCoProc,
                PrivilegeMode = PrivilegeMode.Kernel
            }];

            // Disable Interrupts
            yield return [new SimpleInstructionTestCase("di", TrapKind.ReservedInstruction)];
            yield return [new SimpleInstructionTestCase("di", SecondaryEffect.WriteCoProc)
            { PrivilegeMode = PrivilegeMode.Kernel }];
            yield return [new SimpleInstructionTestCase("di $v1", GPRegister.ReturnValue1)
            {
                ExpectedSideEffects = SecondaryEffect.WriteCoProc,
                PrivilegeMode = PrivilegeMode.Kernel
            }];
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
    [DynamicData(nameof(TrapInstructionTestsList))]
    public void TrapInstructionTests(SimpleInstructionTestCase @case) => RunTest(@case);

    [DataTestMethod]
    [DynamicData(nameof(MemoryInstructionTestsList))]
    public void MemoryInstructionTests(SimpleInstructionTestCase @case) => RunTest(@case);

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

        var interpreter = new Emulator();

        // Initialize the status register
        interpreter.Computer.Processor.CoProcessor0.StatusRegister = @case.Status;

        // Initialize the register file with the provided values
        foreach (var (reg, value) in @case.RegisterInitialization)
            interpreter.Computer.Processor[reg] = value;

        // Initialize the high and low registers if specified in the test case
        if (@case.InitialHighLow.HasValue)
        {
            interpreter.Computer.Processor.Low = @case.InitialHighLow.Value.Low;
            interpreter.Computer.Processor.High = @case.InitialHighLow.Value.High;
        }

        // Initialize the memory, if specified in the test case
        foreach (var (address, data) in @case.MemoryInitialization)
            interpreter.Computer.Memory.Write(address, data);

        interpreter.InsertInstructionExecution(instruction, out var execution, out var trap);

        // Ensure that the expected trap was raised (if any)
        Assert.AreEqual(@case.ExpectedTrap, trap);

        var writeback = @case.ExpectedWriteBack;
        if (writeback.HasValue)
        {
            // Ensure that the expected register was written to with the expected value
            Assert.AreEqual(writeback.Value.Regiter, execution.GPR);

            var writeBackValue = writeback.Value.Value;
            if (writeBackValue.HasValue)
            {
                Assert.AreEqual(writeBackValue.Value, interpreter.Computer.Processor.RegisterFile[execution.GPR]);
            }
        }
        else
        {
            // If no register check was provided, we at least want to make sure no register was written to (as that would be unexpected)
            Assert.AreEqual(execution.GPR, GPRegister.Zero);
        }

        var highLow = @case.ExpectedHighLow;
        if (highLow.HasValue)
        {
            Assert.AreEqual(highLow.Value.Low, interpreter.Computer.Processor.Low);
            Assert.AreEqual(highLow.Value.High, interpreter.Computer.Processor.High);
        }

        var expectedMemory = @case.ExpectedMemory;
        if (expectedMemory is not null)
        {
            var buffer = new byte[expectedMemory.Value.Data.Length];
            interpreter.Computer.Memory.Read(expectedMemory.Value.Address, buffer);
            CollectionAssert.AreEqual(expectedMemory.Value.Data, buffer);
        }
    }
}
