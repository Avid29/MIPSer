﻿// Adam Dernis 2023

using Microsoft.VisualStudio.TestTools.UnitTesting;
using MIPS.Assembler.Models.Modules;
using MIPS.Assembler.Parsers;
using MIPS.Models.Addressing;
using MIPS.Models.Addressing.Enums;

namespace MIPS.Assembler.Tests;

[TestClass]
public class ExpressionParserTests
{
    private const string Self = "10";
    private const string NegativeSelf = "-10";
    private const string Add = "10 + 10";
    private const string Subtract = "25 - 10";
    private const string Multiply = "4 * 4";
    private const string Divide = "8 / 2";
    private const string Mod = "8 % 3";
    private const string And = "9 & 3";
    private const string Or = "9 | 3";
    private const string Xor = "9 ^ 3";

    private const string Macro = "macro + 10";

    private const string MacroFail = "macro + macro";

    [TestMethod(Self)]
    public void SelfTest() => RunTest(Self, 10);

    [TestMethod(NegativeSelf)]
    public void NegativeSelfTest() => RunTest(NegativeSelf, -10);

    [TestMethod(Add)]
    public void AddTest() => RunTest(Add, 10 + 10);

    [TestMethod(Subtract)]
    public void SubtractTest() => RunTest(Subtract, 25 - 10);

    [TestMethod(Multiply)]
    public void MultiplyTest() => RunTest(Multiply, 4 * 4);

    [TestMethod(Divide)]
    public void DivideTest() => RunTest(Divide, 8 / 2);

    [TestMethod(Mod)]
    public void ModTest() => RunTest(Mod, 8 % 3);

    [TestMethod(And)]
    public void AndTest() => RunTest(And, 9 & 3);

    [TestMethod(Or)]
    public void OrTest() => RunTest(Or, 9 | 3);

    [TestMethod(Xor)]
    public void XorTest() => RunTest(Xor, 9 ^ 3);

    [TestMethod(Macro)]
    public void MarcoTest() => RunTest(Macro, 10 + 10, ("macro", new Address(10, Section.Text)));

    [TestMethod(MacroFail)]
    public void MarcoFailTest() => RunTest(MacroFail, null, ("macro", new Address(10, Section.Text)));

    private static void RunTest(string input, long? expected = null) => RunTest(new ExpressionParser(), input, expected);

    private static void RunTest(string input, long? expected = null, params (string name, Address addr)[] macros)
    {
        // NOTE: This assumes symbol realization is not implemented!
        var obj = new ModuleConstruction();
        foreach (var macro in macros)
        {
            obj.TryDefineSymbol(macro.name, macro.addr);
        }

        var parser = new ExpressionParser(obj);
        RunTest(parser, input, expected);
    }

    private static void RunTest(ExpressionParser parser, string input, long? expected = null)
    {
        bool success = parser.TryParse(input, out var actual);
        Assert.AreEqual(success, expected.HasValue);
        if (expected.HasValue)
        {
            Assert.AreEqual(expected.Value, actual.Value);
        }
    }
}
