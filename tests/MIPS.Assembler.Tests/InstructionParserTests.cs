// Adam Dernis 2023

using Microsoft.VisualStudio.TestTools.UnitTesting;
using MIPS.Assembler.Logging;
using MIPS.Assembler.Logging.Enum;
using MIPS.Assembler.Parsers;
using MIPS.Models.Instructions;
using MIPS.Models.Instructions.Enums;

namespace MIPS.Assembler.Tests;

[TestClass]
public class InstructionParserTests
{
    private const string Add = "add $t0, $s0, $s1";
    private const string Addi = "addi $t0, $s0, 100";
    private const string Sll = "sll $t0, $s0, 3";
    private const string LoadWord = "lw $t0, 100($s0)";
    private const string StoreByte = "sb $t0, -100($s0)";
    private const string Jump = "j 1000";
    private const string JumpExpression = "j 10*10";

    private const string XkcdFail = "xkcd $t0, $s0, $s1";

    [TestMethod(Add)]
    public void AddTest()
    {
        Instruction expected = Instruction.Create(FunctionCode.Add, Register.Saved0, Register.Saved1, Register.Temporary0);
        RunTest(Add, expected);
    }

    [TestMethod(Addi)]
    public void AddiTest()
    {
        Instruction expected = Instruction.Create(OperationCode.AddImmediate, Register.Saved0, Register.Temporary0, 100);
        RunTest(Addi, expected);
    }

    [TestMethod(Sll)]
    public void SllTest()
    {
        Instruction expected = Instruction.Create(FunctionCode.ShiftLeftLogical, Register.Zero, Register.Saved0, Register.Temporary0, 3);
        RunTest(Sll, expected);
    }

    [TestMethod(LoadWord)]
    public void LoadWordTest()
    {
        Instruction expected = Instruction.Create(OperationCode.LoadWord, Register.Saved0, Register.Temporary0, 100);
        RunTest(LoadWord, expected);
    }

    [TestMethod(StoreByte)]
    public void StoreByteTest()
    {
        Instruction expected = Instruction.Create(OperationCode.StoreByte, Register.Saved0, Register.Temporary0, -100);
        RunTest(StoreByte, expected);
    }

    [TestMethod(Jump)]
    public void JumpTest()
    {
        Instruction expected = Instruction.Create(OperationCode.Jump, 1000);
        RunTest(Jump, expected);
    }

    [TestMethod(JumpExpression)]
    public void JumpExpressionTest()
    {
        Instruction expected = Instruction.Create(OperationCode.Jump, 100);
        RunTest(JumpExpression, expected);
    }
    
    [TestMethod(XkcdFail)]
    public void XkdFailTest()
    {
        RunFailTest(XkcdFail, LogId.InvalidInstructionName);
    }

    private static void RunTest(string input, Instruction expected)
    {
        var parser = new InstructionParser();

        TokenizeInstruction(input, out var name, out var args);
        var success = parser.TryParse(name, args, out var actual);

        Assert.IsTrue(success);
        Assert.AreEqual(expected, actual);
    }

    private static void RunFailTest(string input, LogId logId)
    {
        var logger = new AssemblerLogger();
        var parser = new InstructionParser(null, logger);

        TokenizeInstruction(input, out var name, out var args);
        var success = parser.TryParse(name, args, out _);

        Assert.IsFalse(success);
        Assert.IsTrue(logger.Logs[0].Id == logId);
    }

    private static void TokenizeInstruction(string line, out string name, out string[] args)
    {
        var nameEnd = line.IndexOf(' ');
        name = line[..nameEnd];
        args = line[(nameEnd + 1)..].Split(',');
    }
}
