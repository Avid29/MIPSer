// Avishai Dernis 2026

using MIPS.Assembler.Models.Instructions;
using MIPS.Assembler.Parsers;
using MIPS.Assembler.Tokenization;
using MIPS.Models.Instructions;
using MIPS.Models.Instructions.Enums.Registers;
using MIPS.Models.Instructions.Enums.SpecialFunctions;
using System.Collections.Generic;

namespace MIPS.Interpreter.Tests;

[TestClass]
public class InstructionTests
{
    public sealed record SimpleInstructionTestCase(
        string Input,
        (GPRegister Regiter, uint Value) Expected,
        params (GPRegister Register, uint Value)[] RegisterInitialization);

    public static IEnumerable<object[]> ArithmeticInstructionTestsList
    {
        get
        {
            // Behaviorally unsigned arithmetic tests
            yield return [new SimpleInstructionTestCase("add $t2, $t0, $t1", (GPRegister.Temporary2, 20 + 10), (GPRegister.Temporary0, 20), (GPRegister.Temporary1, 10))];
            yield return [new SimpleInstructionTestCase("addi $t1, $t0, 10", (GPRegister.Temporary1, 20 + 10), (GPRegister.Temporary0, 20))];
            yield return [new SimpleInstructionTestCase("sub $t2, $t0, $t1", (GPRegister.Temporary2, 30 - 20), (GPRegister.Temporary0, 30), (GPRegister.Temporary1, 20))];
            //yield return [new SimpleInstructionTestCase("mul $t2, $t0, $t1", (GPRegister.Temporary2, 30 * 20), (GPRegister.Temporary0, 30), (GPRegister.Temporary1, 20))];
            yield return [new SimpleInstructionTestCase("sra $t1, $t0, 4", (GPRegister.Temporary1, 101 >> 4), (GPRegister.Temporary0, 101))];
            yield return [new SimpleInstructionTestCase("srav $t2, $t0, $t1", (GPRegister.Temporary2, 101 >> 4), (GPRegister.Temporary0, 101), (GPRegister.Temporary1, 4))];

            // Behaviorally signed arithmetic tests
            yield return [new SimpleInstructionTestCase("add $t2, $t0, $t1", (GPRegister.Temporary2, 30 + (-10)), (GPRegister.Temporary0, 30), (GPRegister.Temporary1, unchecked((uint)-10)))];
            yield return [new SimpleInstructionTestCase("sub $t2, $t0, $t1", (GPRegister.Temporary2, 20 - (-10)), (GPRegister.Temporary0, 20), (GPRegister.Temporary1, unchecked((uint)-10)))];

            // TODO: Test/Handle overflow behavior
        }
    }

    public static IEnumerable<object[]> UnsignedArithmeticInstructionTestsList
    {
        get
        {
            yield return [new SimpleInstructionTestCase("addu $t2, $t0, $t1", (GPRegister.Temporary2, 20 + 10), (GPRegister.Temporary0, 20), (GPRegister.Temporary1, 10))];
            yield return [new SimpleInstructionTestCase("addiu $t1, $t0, 10", (GPRegister.Temporary1, 20 + 10), (GPRegister.Temporary0, 20))];
            yield return [new SimpleInstructionTestCase("subu $t2, $t0, $t1", (GPRegister.Temporary2, 30 - 20), (GPRegister.Temporary0, 30), (GPRegister.Temporary1, 20))];

            // TODO: Test/Handle overflow behavior
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
            // Data (kept here for convience, since it doesn't fit well in the other categories)
            yield return [new SimpleInstructionTestCase("lui $t0, 0x1234", (GPRegister.Temporary0, 0x12340000))];

            // Compare
            yield return [new SimpleInstructionTestCase("slt $t2, $t0, $t1", (GPRegister.Temporary2, 1), (GPRegister.Temporary0, 20), (GPRegister.Temporary1, 30))];
            yield return [new SimpleInstructionTestCase("slt $t2, $t0, $t1", (GPRegister.Temporary2, 0), (GPRegister.Temporary0, 30), (GPRegister.Temporary1, 20))];
            yield return [new SimpleInstructionTestCase("sltu $t2, $t0, $t1", (GPRegister.Temporary2, 1), (GPRegister.Temporary0, 20), (GPRegister.Temporary1, 30))];
            yield return [new SimpleInstructionTestCase("sltu $t2, $t0, $t1", (GPRegister.Temporary2, 0), (GPRegister.Temporary0, 30), (GPRegister.Temporary1, 20))];
        }
    }

    public static IEnumerable<object[]> UncategorizedRegisterOnlyInstructionTestsList
    {
        get
        {
            // lui
            yield return [new SimpleInstructionTestCase("lui $t0, 0x1234", (GPRegister.Temporary0, 0x12340000))];

            // Niche bit-manipulation
        }
    }

    [DataTestMethod]
    [DynamicData(nameof(ArithmeticInstructionTestsList))]
    public void ArithmeticInstructionTests(SimpleInstructionTestCase @case)
        => RunTest(@case.Input, @case.Expected, @case.RegisterInitialization);

    [DataTestMethod]
    [DynamicData(nameof(UnsignedArithmeticInstructionTestsList))]
    public void UnsignedArithmeticInstructionTests(SimpleInstructionTestCase @case)
        => RunTest(@case.Input, @case.Expected, @case.RegisterInitialization);

    [DataTestMethod]
    [DynamicData(nameof(LogicalInstructionTestsList))]
    public void LogicalInstructionTests(SimpleInstructionTestCase @case)
        => RunTest(@case.Input, @case.Expected, @case.RegisterInitialization);

    [DataTestMethod]
    [DynamicData(nameof(CompareInstructionTestsList))]
    public void CompareInstructionTests(SimpleInstructionTestCase @case)
        => RunTest(@case.Input, @case.Expected, @case.RegisterInitialization);

    [DataTestMethod]
    [DynamicData(nameof(UncategorizedRegisterOnlyInstructionTestsList))]
    public void UncategorizedRegisterOnlyInstructionTests(SimpleInstructionTestCase @case)
        => RunTest(@case.Input, @case.Expected, @case.RegisterInitialization);

    private static void RunTest(string line, (GPRegister, uint) check, params (GPRegister, uint)[] regInits)
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

        interpreter.InsertInstructionExecution(instruction);
        Assert.AreEqual(check.Item2, interpreter.GetRegister(check.Item1));
    }
}
