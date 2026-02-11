// Adam Dernis 2024

using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using Test.MIPS.Helpers;
using Zarem.Assembler.MIPS.Config;
using Zarem.Assembler.MIPS.Helpers.Tables;
using Zarem.Assembler.MIPS.Logging;
using Zarem.Assembler.MIPS.Logging.Enum;
using Zarem.Assembler.MIPS.Models.Instructions;
using Zarem.Assembler.MIPS.Parsers;
using Zarem.Assembler.MIPS.Tokenization;
using Zarem.Disassembler.MIPS.Services;
using Zarem.MIPS.Models.Instructions;
using Zarem.MIPS.Models.Instructions.Enums;
using Zarem.MIPS.Models.Instructions.Enums.Operations;
using Zarem.MIPS.Models.Instructions.Enums.Registers;
using Zarem.MIPS.Models.Instructions.Enums.SpecialFunctions;
using Zarem.MIPS.Models.Instructions.Enums.SpecialFunctions.CoProc0;
using Zarem.MIPS.Models.Instructions.Enums.SpecialFunctions.FloatProc;
using Zarem.MIPS.Services;

namespace Test.Assembler.MIPS.Parsers;

[TestClass]
public class InstructionParserTests
{
    public sealed record InstructionParsingTestCase(
        string Input,
        Instruction? Expected,
        LogCode? Code)
    {
        public InstructionParsingTestCase(string input, Instruction expected) : this(input, expected, null)
        {
        }

        public InstructionParsingTestCase(string input, LogCode code) : this(input, null, code)
        {
        }

        public override string ToString() => Input;
    }

    public static string InstructionParsingTestCaseDisplayName(MethodInfo _, object[] data)
        => $"{(InstructionParsingTestCase)data[0]}";

    public static IEnumerable<object[]> RawInstructionSuccessTestsList
    {
        get
        {
            yield return [new InstructionParsingTestCase("nop", Instruction.NOP)];
            yield return [new InstructionParsingTestCase("add $t0, $s0, $s1", Instruction.Create(FunctionCode.Add, GPRegister.Saved0, GPRegister.Saved1, GPRegister.Temporary0))];
            yield return [new InstructionParsingTestCase("addi $t0, $s0, 100", Instruction.Create(OperationCode.AddImmediate, GPRegister.Saved0, GPRegister.Temporary0, (short)100))];
            yield return [new InstructionParsingTestCase("sll $t0, $s0, 3", Instruction.Create(FunctionCode.ShiftLeftLogical, GPRegister.Zero, GPRegister.Saved0, GPRegister.Temporary0, 3))];
            yield return [new InstructionParsingTestCase("lw $t0, 100($s0)", Instruction.Create(OperationCode.LoadWord, GPRegister.Saved0, GPRegister.Temporary0, (short)100))];
            yield return [new InstructionParsingTestCase("sb $t0, -100($s0)", Instruction.Create(OperationCode.StoreByte, GPRegister.Saved0, GPRegister.Temporary0, (short)-100))];
            yield return [new InstructionParsingTestCase("j 1000", Instruction.Create(OperationCode.Jump, 1000))];
            yield return [new InstructionParsingTestCase("j 10*10", Instruction.Create(OperationCode.Jump, 10 * 10))];
            yield return [new InstructionParsingTestCase("di", CoProc0Instruction.Create(MFMC0FuncCode.DisableInterrupts, GPRegister.Zero, 12))];
            yield return [new InstructionParsingTestCase("di $t1", CoProc0Instruction.Create(MFMC0FuncCode.DisableInterrupts, GPRegister.Temporary1, 12))];
            yield return [new InstructionParsingTestCase("ei", CoProc0Instruction.Create(MFMC0FuncCode.EnableInterrupts, GPRegister.Zero, 12))];
            yield return [new InstructionParsingTestCase("cvt.S.D $f4, $f8", FloatInstruction.Create(FloatFuncCode.ConvertToSingle, FloatFormat.Double, FloatRegister.F8, FloatRegister.F4))];
        }
    }

    public static IEnumerable<object[]> RawInstructionFailureTestsList
    {
        get
        {
            yield return [new InstructionParsingTestCase("xkcd $t0, $s0, $s1", LogCode.InvalidInstructionName)];
            yield return [new InstructionParsingTestCase("add $t0, $s0", LogCode.InvalidInstructionArgCount)];
            yield return [new InstructionParsingTestCase("add $t0, $s0, $s1, $s1", LogCode.InvalidInstructionArgCount)];
        }
    }

    public static IEnumerable<object[]> RawInstructionWarningTestsList
    {
        get
        {
            yield return [new InstructionParsingTestCase("sll $t0, $s0, 33", Instruction.Create(FunctionCode.ShiftLeftLogical, GPRegister.Zero, GPRegister.Saved0, GPRegister.Temporary0, 1), LogCode.IntegerTruncated)];
            yield return [new InstructionParsingTestCase("sll $t0, $s0, -1", Instruction.Create(FunctionCode.ShiftLeftLogical, GPRegister.Zero, GPRegister.Saved0, GPRegister.Temporary0, 31), LogCode.IntegerTruncated)];
            yield return [new InstructionParsingTestCase("j 0x1", Instruction.Create(OperationCode.Jump, 0x1), LogCode.IntegerTruncated)];
        }
    }

    public static IEnumerable<object[]> GeneratedTestList
    {
        get
        {
            var table = new InstructionTable(new());
            foreach (var instruction in table.GetInstructions())
            {
                // TODO: Disassembling pseudo instructions
                if (instruction.IsPseudoInstruction)
                    continue;

                // TODO: Disassembling CoProc0 instructions
                if (instruction.OpCode is OperationCode.Coprocessor0)
                    continue;

                // Apply format to instruction name, if applicable
                var name = instruction.Name;
                if (name.EndsWith(".fmt"))
                {
                    name = FloatFormatTable.ApplyFormat(name, ArgGenerator.RandomFormat(instruction.FloatFormats));
                }

                // Generate instruction
                StringBuilder line = new(name);
                line.Append(' ');

                foreach (var arg in instruction.ArgumentPattern)
                {
                    line.Append(arg switch
                    {
                        Argument.RS or Argument.RT or Argument.RD => RegistersTable.GetRegisterString(ArgGenerator.RandomRegister()),
                        Argument.FS or Argument.FT or Argument.FD => RegistersTable.GetRegisterString(ArgGenerator.RandomRegister(), RegisterSet.FloatingPoints),
                        Argument.Immediate => $"{ArgGenerator.RandomImmediate()}",
                        Argument.Offset => $"{ArgGenerator.RandomOffset()}",
                        Argument.Address => $"{ArgGenerator.RandomAddress()}",
                        Argument.AddressBase => $"{ArgGenerator.RandomImmediate()}({RegistersTable.GetRegisterString(ArgGenerator.RandomRegister())})",
                        Argument.Shift => $"{ArgGenerator.RandomShift()}",
                        Argument.FullImmediate => Random.Shared.Next(),
                        _ => throw new NotImplementedException(),
                    });

                    line.Append(", ");
                }

                // Remove final ", "
                if (instruction.ArgumentPattern.Length > 0)
                    line.Remove(line.Length - 2, 2);

                // Return test case
                yield return [$"{line}"];
            }
        }
    }

    [DataTestMethod]
    [DynamicData(nameof(RawInstructionSuccessTestsList),
        DynamicDataDisplayName = nameof(InstructionParsingTestCaseDisplayName),
        DynamicDataDisplayNameDeclaringType = typeof(InstructionParserTests))]
    public void RawInstructionSuccessTests(InstructionParsingTestCase @case)
        => RunTest(@case.Input, new ParsedInstruction(@case.Expected!.Value));

    [DataTestMethod]
    [DynamicData(nameof(RawInstructionFailureTestsList),
        DynamicDataDisplayName = nameof(InstructionParsingTestCaseDisplayName),
        DynamicDataDisplayNameDeclaringType = typeof(InstructionParserTests))]
    public void RawInstructionFailureTests(InstructionParsingTestCase @case)
        => RunTest(@case.Input, logCode: @case.Code);

    [DataTestMethod]
    [DynamicData(nameof(RawInstructionWarningTestsList),
        DynamicDataDisplayName = nameof(InstructionParsingTestCaseDisplayName),
        DynamicDataDisplayNameDeclaringType = typeof(InstructionParserTests))]
    public void RawInstructionWarningTests(InstructionParsingTestCase @case)
        => RunTest(@case.Input, new ParsedInstruction(@case.Expected!.Value), @case.Code);

    private const string LoadImmediate = "li $t0, 0x10001";
    
    [TestMethod(LoadImmediate)]
    public void LoadImmediateTest()
    {
        PseudoInstruction expected = new(PseudoOp.LoadImmediate) { RT = GPRegister.Temporary0, Immediate = 0x10001 };
        RunTest(LoadImmediate, new ParsedInstruction(expected));
    }

    [TestMethod("Generated Tests")]
    [DynamicData(nameof(GeneratedTestList))]
    public void GeneratedTests(string input)
    {
        var config = new AssemblerConfig();
#if DEBUG
        ServiceCollection.DisassemblerService = new DisassemblerService(config);
#endif

        var table = new InstructionTable(config);
        var parser = new InstructionParser(table, null);

        var tokenized = Tokenizer.TokenizeLine(input, nameof(RunTest));
        var succeeded = parser.TryParse(tokenized, out var actual);

        // Validate execution
        Assert.IsTrue(succeeded);

        var result = actual?.Realize()[0];
        Assert.IsTrue(result.HasValue);

#if DEBUG
        Assert.AreEqual(input, result.Value.Disassembled);
#endif
    }

    private static void RunTest(string input, ParsedInstruction? expected = null, LogCode? logCode = null, string? expectedSymbol = null)
    {
        bool succeeds = expected is not null;

        // Initialize parser
        var logger = new Logger();
        var parser = new InstructionParser(new InstructionTable(new()), logger);

        // Parse instruction
        var line = Tokenizer.TokenizeLine(input, nameof(RunTest));
        var succeeded = parser.TryParse(line, out var actual);

        // Validate results
        Assert.AreEqual(succeeds, succeeded);
        if (succeeds)
        {
            Assert.IsNotNull(expected);
            Assert.IsNotNull(actual);

            var expectedReal = expected.Realize();
            var actualReal = actual.Realize();

            for (int i = 0 ; i < expectedReal.Length; i++)
            {
                Assert.AreEqual(expectedReal[i], actualReal[i]);
            }
        }

        if (logCode.HasValue)
        {
            Assert.IsTrue(logger.CurrentLog[0].Code == logCode.Value);
        }
    }
}
