// Adam Dernis 2024

using CommunityToolkit.Diagnostics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MIPS.Assembler.Models.Directives;
using MIPS.Assembler.Models.Directives.Abstract;
using MIPS.Assembler.Parsers;
using MIPS.Assembler.Tokenization;
using MIPS.Models.Addressing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;

namespace MIPS.Assembler.Tests.Parsers;

// TODO: Test expressions in directive args

[TestClass]
public class DirectiveParserTests
{
    private const string Global = ".globl main";

    public static IEnumerable<object[]> DataTests =>
    [
        Flatten(".byte 10", 10),
        Flatten(".word 10", 0, 0, 0, 10),
        Flatten(".byte 10, 10", 10, 10),
        Flatten(".ascii \"Test String\"", Encoding.ASCII.GetBytes("Test String")),
        Flatten(".asciiz \"Test String\"", Encoding.ASCII.GetBytes("Test String\0")),
    ];

    private static object[] Flatten(string input, params byte[] bytes)
    {
        var arr = new object[bytes.Length + 1];
        arr[0] = input;

        for (int i = 0; i < bytes.Length; i++)
            arr[i + 1] = bytes[i];

        return arr;
    }


    [TestMethod(Global)]
    public void GlobalTest() => RunGlobalTest(Global, "main");

    [DataTestMethod]
    [DynamicData(nameof(DataTests))]
    public void DirectiveDataTest(string input, params byte[] expected) =>
        RunDataTest(input, expected);

    private static Directive ParseDirective(string input)
    {
        var parser = new DirectiveParser();

        // Tokenize directive
        var line = Tokenizer.TokenizeLine(input, nameof(RunGlobalTest));
        if (line.Directive is null)
            Assert.Fail();

        if(!parser.TryParseDirective(line, out var directive))
            Assert.Fail();

        if (directive is null)
            Assert.Fail();

        return directive;
    }

    private static void RunGlobalTest(string input, string expected)
    {
        // Get directive and validate type
        var directive = ParseDirective(input);
        if (directive is not GlobalDirective)
            Assert.Fail();

        var actual = ((GlobalDirective)directive).Symbol;
        Guard.IsNotNull(actual);

        Assert.AreEqual(expected, actual);
    }

    private static void RunDataTest(string input, params byte[] expected)
    {
        // Get directive and validate type
        var directive = ParseDirective(input);
        if (directive is not DataDirective)
            Assert.Fail();

        var actual = ((DataDirective)directive).Data;
        Guard.IsNotNull(actual);

        Assert.AreEqual(expected.Length, actual.Length);
        for (int i = 0; i < expected.Length; i++)
        {
            Assert.AreEqual(expected[i], actual[i]);
        }
    }
}
