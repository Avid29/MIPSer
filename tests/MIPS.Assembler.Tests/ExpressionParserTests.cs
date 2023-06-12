﻿// Adam Dernis 2023

using Microsoft.VisualStudio.TestTools.UnitTesting;
using MIPS.Assembler.Parsers;
using MIPS.Assembler.Parsers.Expressions.Evaluator;

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

    [TestMethod(Self)]
    public void SelfTest() => Test(Self, 10);

    [TestMethod(NegativeSelf)]
    public void NegativeSelfTest() => Test(NegativeSelf, -10);

    [TestMethod(Add)]
    public void AddTest() => Test(Add, 10 + 10);

    [TestMethod(Subtract)]
    public void SubtractTest() => Test(Subtract, 25 - 10);

    [TestMethod(Multiply)]
    public void MultiplyTest() => Test(Multiply, 4 * 4);

    [TestMethod(Divide)]
    public void DivideTest() => Test(Divide, 8 / 2);

    [TestMethod(Mod)]
    public void ModTest() => Test(Mod, 8 % 3);

    [TestMethod(And)]
    public void AndTest() => Test(And, 9 & 3);

    [TestMethod(Or)]
    public void OrTest() => Test(Or, 9 | 3);

    [TestMethod(Xor)]
    public void XorTest() => Test(Xor, 9 ^ 3);

    private static void Test(string input, long expected)
    {
        var parser = new ExpressionParser(new IntegerEvaluator());
        _ = parser.TryParse(input, out var actual);
        Assert.AreEqual(expected, actual);
    }
}
