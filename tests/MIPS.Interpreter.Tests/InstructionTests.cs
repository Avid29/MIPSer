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

    public static IEnumerable<object[]> SimpleInstructionTestsList
    {
        get
        {
            yield return [new SimpleInstructionTestCase("sll $t1, $t0, 1", (GPRegister.Temporary1, 20), (GPRegister.Temporary0, 10))];
            yield return [new SimpleInstructionTestCase("add $t2, $t0, $t1", (GPRegister.Temporary2, 30), (GPRegister.Temporary0, 20), (GPRegister.Temporary1, 10))];
            yield return [new SimpleInstructionTestCase("sub $t2, $t0, $t1", (GPRegister.Temporary2, 10), (GPRegister.Temporary0, 30), (GPRegister.Temporary1, 20))];

            yield return [new SimpleInstructionTestCase("add $t2, $t0, $t1", (GPRegister.Temporary2, 20), (GPRegister.Temporary0, 30), (GPRegister.Temporary1, unchecked((uint)-10)))];
            yield return [new SimpleInstructionTestCase("sub $t2, $t0, $t1", (GPRegister.Temporary2, 30), (GPRegister.Temporary0, 20), (GPRegister.Temporary1, unchecked((uint)-10)))];
        }
    }

    [DataTestMethod]
    [DynamicData(nameof(SimpleInstructionTestsList))]
    public void SimpleInstructionTests(SimpleInstructionTestCase @case)
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
