// Adam Dernis 2024

using Microsoft.VisualStudio.TestTools.UnitTesting;
using MIPS.Assembler.Helpers.Tables;
using MIPS.Assembler.Logging;
using MIPS.Assembler.Logging.Enum;
using MIPS.Assembler.Models.Instructions;
using MIPS.Assembler.Parsers;
using MIPS.Assembler.Tokenization;
using MIPS.Disassembler.Services;
using MIPS.Models.Addressing;
using MIPS.Models.Instructions;
using MIPS.Models.Instructions.Enums;
using MIPS.Models.Instructions.Enums.Operations;
using MIPS.Models.Instructions.Enums.Registers;
using MIPS.Models.Instructions.Enums.SpecialFunctions;
using MIPS.Models.Instructions.Enums.SpecialFunctions.CoProc0;
using MIPS.Models.Instructions.Enums.SpecialFunctions.FloatProc;
using MIPS.Services;
using MIPS.Tests.Helpers;
using System;
using System.Collections.Generic;
using System.Text;

namespace MIPS.Assembler.Tests.Parsers;

[TestClass]
public class InstructionParserTests
{
    public static IEnumerable<object[]> RawInstructionSuccessTestsList { get; } =
    [
        Flatten("nop", Instruction.NOP),
        Flatten("add $t0, $s0, $s1", Instruction.Create(FunctionCode.Add, GPRegister.Saved0, GPRegister.Saved1, GPRegister.Temporary0)),
        Flatten("addi $t0, $s0, 100", Instruction.Create(OperationCode.AddImmediate, GPRegister.Saved0, GPRegister.Temporary0, (short)100)),
        Flatten("sll $t0, $s0, 3", Instruction.Create(FunctionCode.ShiftLeftLogical, GPRegister.Zero, GPRegister.Saved0, GPRegister.Temporary0, 3)),
        Flatten("lw $t0, 100($s0)", Instruction.Create(OperationCode.LoadWord, GPRegister.Saved0, GPRegister.Temporary0, (short)100)),
        Flatten("sb $t0, -100($s0)", Instruction.Create(OperationCode.StoreByte, GPRegister.Saved0, GPRegister.Temporary0, (short)-100)),
        Flatten("j 1000", Instruction.Create(OperationCode.Jump, 1000)),
        Flatten("j 10*10", Instruction.Create(OperationCode.Jump, 10 * 10)),
        Flatten("di", CoProc0Instruction.Create(MFMC0FuncCode.DisableInterupts, GPRegister.Zero, 12)),
        Flatten("di $t1", CoProc0Instruction.Create(MFMC0FuncCode.DisableInterupts, GPRegister.Temporary1, 12)),
        Flatten("ei", CoProc0Instruction.Create(MFMC0FuncCode.EnableInterupts, GPRegister.Zero, 12)),
        Flatten("cvt.S.D $f4, $f8", FloatInstruction.Create(FloatFuncCode.ConvertToSingle, FloatFormat.Double, FloatRegister.F8, FloatRegister.F4)),
    ];

    public static object[] Flatten(string input, Instruction instruction)
    {
        var array = new object[2];
        array[0] = input;
        array[1] = (uint)instruction;
        return array;
    }

    [DataTestMethod]
    [DynamicData(nameof(RawInstructionSuccessTestsList))]
    public void RawInstructionSuccessTests(string input, uint expected)
        => RunTest(input, new ParsedInstruction((Instruction)expected));

    private const string LoadImmediate = "li $t0, 0x10001";

    private const string SllWarnTruncate = "sll $t0, $s0, 33";
    private const string SllWarnSigned = "sll $t0, $s0, -1";
    private const string JWarnTruncate = "j 0x1";

    private const string XkcdFail = "xkcd $t0, $s0, $s1";
    private const string TooFewArgs = "add $t0, $s0";
    private const string TooManyArgs = "add $t0, $s0, $s1, $s1";
    
    [TestMethod(LoadImmediate)]
    public void LoadImmediateTest()
    {
        PseudoInstruction expected = new(PseudoOp.LoadImmediate) { RT = GPRegister.Temporary0, Immediate = 0x10001 };
        RunTest(LoadImmediate, new ParsedInstruction(expected));
    }

    [TestMethod(SllWarnTruncate)]
    public void SllWarnTruncateTest()
    {
        Instruction expected = Instruction.Create(FunctionCode.ShiftLeftLogical, GPRegister.Zero, GPRegister.Saved0, GPRegister.Temporary0, 1);
        RunTest(SllWarnTruncate, new ParsedInstruction(expected), logCode:LogCode.IntegerTruncated);
    }

    [TestMethod(SllWarnSigned)]
    public void SllWarnSignedTest()
    {
        Instruction expected = Instruction.Create(FunctionCode.ShiftLeftLogical, GPRegister.Zero, GPRegister.Saved0, GPRegister.Temporary0, 31);
        RunTest(SllWarnSigned, new ParsedInstruction(expected), logCode:LogCode.IntegerTruncated);
    }

    [TestMethod(JWarnTruncate)]
    public void JWarnTruncateTest()
    {
        Instruction expected = Instruction.Create(OperationCode.Jump, 0x1);
        RunTest(JWarnTruncate, new ParsedInstruction(expected), logCode:LogCode.IntegerTruncated);
    }

    [TestMethod(XkcdFail)]
    public void XkdFailTest()
    {
        RunTest(XkcdFail, logCode: LogCode.InvalidInstructionName);
    }

    [TestMethod(TooFewArgs)]
    public void TooFewArgsTest()
    {
        RunTest(TooFewArgs, logCode: LogCode.InvalidInstructionArgCount);
    }

    [TestMethod(TooManyArgs)]
    public void TooManyArgsTest()
    {
        RunTest(TooManyArgs, logCode: LogCode.InvalidInstructionArgCount);
    }

    [TestMethod("Generated Tests")]
    public void GeneratedTests()
    {
        #if DEBUG
        ServiceCollection.DisassemblerService = new DisassemblerService();
        #endif

        var table = new InstructionTable(MipsVersion.MipsII);
        var parser = new InstructionParser(table, null);

        foreach(var instruction in table.GetInstructions())
        {
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

            foreach(var arg in instruction.ArgumentPattern)
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
                line.Remove(line.Length-2,2);
            
            // Parse instruction
            var input = line.ToString();
            var tokenized = Tokenizer.TokenizeLine(input, nameof(RunTest));
            var succeeded = parser.TryParse(tokenized, out var actual);

            // Validate execution
            Assert.IsTrue(succeeded, input);
            var result = actual?.Realize()[0];
            Assert.IsTrue(result.HasValue, input);

#if DEBUG
            Assert.IsTrue(input == result.Value.Disassembled, $"\"{input}\" != \"{result.Value.Disassembled}\"");
#endif
        }
    }

    private static void RunTest(string input, ParsedInstruction? expected = null, LogCode? logCode = null, string? expectedSymbol = null)
    {
        bool succeeds = expected is not null;

        // Initialize parser
        var logger = new Logger();
        var parser = new InstructionParser(new InstructionTable(MipsVersion.MipsII), logger);

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
            Assert.IsTrue(logger.Logs[0].Code == logCode.Value);
        }
    }
}
