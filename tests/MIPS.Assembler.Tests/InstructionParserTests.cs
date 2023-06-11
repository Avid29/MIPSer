// Adam Dernis 2023

using Microsoft.VisualStudio.TestTools.UnitTesting;
using MIPS.Models.Instructions;
using MIPS.Models.Instructions.Enums;

namespace MIPS.Assembler.Tests;

[TestClass]
public class InstructionParserTests
{
    private const string Add = "add $t0, $s0, $s1";
    private const string Addi = "addi $t0, $s0, 100";
    private const string LoadWord = "lw $t0, 100($s0)";
    private const string Jump = "j 1000";

    [TestMethod(Add)]
    public void AddTest()
    {
        Instruction expected = Instruction.Create(FunctionCode.Add, Register.Saved0, Register.Saved1, Register.Temporary0);
        Test(Add, expected);
    }

    [TestMethod(Addi)]
    public void AddiTest()
    {
        Instruction expected = Instruction.Create(OperationCode.AddImmediate, Register.Saved0, Register.Temporary0, 100);
        Test(Addi, expected);
    }

    [TestMethod(LoadWord)]
    public void LoadWordTest()
    {
        Instruction expected = Instruction.Create(OperationCode.LoadWord, Register.Saved0, Register.Temporary0, 100);
        Test(LoadWord, expected);
    }

    [TestMethod(Jump)]
    public void JumpTest()
    {
        Instruction expected = Instruction.Create(OperationCode.Jump, 1000);
        Test(Jump, expected);
    }

    private static void Test(string input, Instruction expected)
    {
        var parser = new InstructionParser();

        var nameEnd = input.IndexOf(' ');
        var name = input[..nameEnd];
        var args = input[(nameEnd + 1)..].Split(',');
        var actual = parser.Parse(name, args);

        Assert.AreEqual(expected, actual);
    }
}
