// Adam Dernis 2024

using Microsoft.VisualStudio.TestTools.UnitTesting;
using MIPS.Assembler.Helpers.Tables;
using MIPS.Assembler.Logging;
using MIPS.Assembler.Logging.Enum;
using MIPS.Assembler.Models.Instructions;
using MIPS.Assembler.Parsers;
using MIPS.Assembler.Tokenization;
using MIPS.Disassembler.Services;
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
using System.Text;

namespace MIPS.Assembler.Tests.Parsers;

[TestClass]
public class InstructionParserTests
{
    private const string NOP = "nop";
    private const string Add = "add $t0, $s0, $s1";
    private const string Addi = "addi $t0, $s0, 100";
    private const string Sll = "sll $t0, $s0, 3";
    private const string LoadWord = "lw $t0, 100($s0)";
    private const string StoreByte = "sb $t0, -100($s0)";
    private const string Jump = "j 1000";
    private const string JumpExpression = "j 10*10";
    private const string DI = "di $t1";
    private const string EI = "ei $t0";
    private const string CVT_S_D = "cvt.S.D $f4, $f8";

    private const string LoadImmediate = "li $t0, 0x10001";

    private const string SllWarnTruncate = "sll $t0, $s0, 33";
    private const string SllWarnSigned = "sll $t0, $s0, -1";

    private const string XkcdFail = "xkcd $t0, $s0, $s1";
    private const string TooFewArgs = "add $t0, $s0";
    private const string TooManyArgs = "add $t0, $s0, $s1, $s1";

    [TestMethod(NOP)]
    public void NopTest() => RunTest(NOP, new ParsedInstruction(Instruction.NOP));

    [TestMethod(Add)]
    public void AddTest()
    {
        Instruction expected = Instruction.Create(FunctionCode.Add, Register.Saved0, Register.Saved1, Register.Temporary0);
        RunTest(Add, new ParsedInstruction(expected));
    }

    [TestMethod(Addi)]
    public void AddiTest()
    {
        Instruction expected = Instruction.Create(OperationCode.AddImmediate, Register.Saved0, Register.Temporary0, (short)100);
        RunTest(Addi, new ParsedInstruction(expected));
    }

    [TestMethod(Sll)]
    public void SllTest()
    {
        Instruction expected = Instruction.Create(FunctionCode.ShiftLeftLogical, Register.Zero, Register.Saved0, Register.Temporary0, 3);
        RunTest(Sll, new ParsedInstruction(expected));
    }

    [TestMethod(LoadWord)]
    public void LoadWordTest()
    {
        Instruction expected = Instruction.Create(OperationCode.LoadWord, Register.Saved0, Register.Temporary0, (short)100);
        RunTest(LoadWord, new ParsedInstruction(expected));
    }

    [TestMethod(StoreByte)]
    public void StoreByteTest()
    {
        Instruction expected = Instruction.Create(OperationCode.StoreByte, Register.Saved0, Register.Temporary0, (short)-100);
        RunTest(StoreByte, new ParsedInstruction(expected));
    }

    [TestMethod(Jump)]
    public void JumpTest()
    {
        Instruction expected = Instruction.Create(OperationCode.Jump, 1000);
        RunTest(Jump, new ParsedInstruction(expected));
    }

    [TestMethod(JumpExpression)]
    public void JumpExpressionTest()
    {
        Instruction expected = Instruction.Create(OperationCode.Jump, 100);
        RunTest(JumpExpression, new ParsedInstruction(expected));
    }

    [TestMethod(DI)]
    public void DITest()
    {
        Instruction expected = CoProc0Instruction.Create(MFMC0FuncCode.DisableInterupts, Register.Temporary1, 12);
        RunTest(DI, new ParsedInstruction(expected));
    }

    [TestMethod(EI)]
    public void EITest()
    {
        Instruction expected = CoProc0Instruction.Create(MFMC0FuncCode.EnableInterupts, Register.Temporary0, 12);
        RunTest(EI, new ParsedInstruction(expected));
    }

    [TestMethod(CVT_S_D)]
    public void CVT_S_DTest()
    {
        Instruction expected = FloatInstruction.Create(FloatFuncCode.ConvertToSingle, FloatFormat.Double, FloatRegister.F8, FloatRegister.F4);
        RunTest(CVT_S_D, new ParsedInstruction(expected));
    }
    
    [TestMethod(LoadImmediate)]
    public void LoadImmediateTest()
    {
        PseudoInstruction expected = new(PseudoOp.LoadImmediate) { RT = Register.Temporary0, Immediate = 0x10001 };
        RunTest(LoadImmediate, new ParsedInstruction(expected));
    }

    [TestMethod(SllWarnTruncate)]
    public void SllWarnTruncateTest()
    {
        Instruction expected = Instruction.Create(FunctionCode.ShiftLeftLogical, Register.Zero, Register.Saved0, Register.Temporary0, 1);
        RunTest(SllWarnTruncate, new ParsedInstruction(expected), logId:LogId.IntegerTruncated);
    }

    [TestMethod(SllWarnSigned)]
    public void SllWarnSignedTest()
    {
        Instruction expected = Instruction.Create(FunctionCode.ShiftLeftLogical, Register.Zero, Register.Saved0, Register.Temporary0, 31);
        RunTest(SllWarnSigned, new ParsedInstruction(expected), logId:LogId.IntegerTruncated);
    }

    [TestMethod(XkcdFail)]
    public void XkdFailTest()
    {
        RunTest(XkcdFail, logId: LogId.InvalidInstructionName);
    }

    [TestMethod(TooFewArgs)]
    public void TooFewArgsTest()
    {
        RunTest(TooFewArgs, logId: LogId.InvalidInstructionArgCount);
    }

    [TestMethod(TooManyArgs)]
    public void TooManyArgsTest()
    {
        RunTest(TooManyArgs, logId: LogId.InvalidInstructionArgCount);
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

            // Generate instruction
            StringBuilder line = new(instruction.Name);
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

    private static void RunTest(string input, ParsedInstruction? expected = null, LogId? logId = null, string? expectedSymbol = null)
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
            var expectedReal = expected!.Realize();
            var actualReal = actual!.Realize();

            for (int i = 0 ; i < expectedReal.Length; i++)
            {
                Assert.AreEqual(expectedReal[i], actualReal[i]);
            }
        }

        if (logId.HasValue)
        {
            Assert.IsTrue(logger.Logs[0].Id == logId.Value);
        }
    }
}
