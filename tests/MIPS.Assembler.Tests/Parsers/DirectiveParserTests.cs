// Adam Dernis 2024

using CommunityToolkit.Diagnostics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MIPS.Assembler.Models.Directives;
using MIPS.Assembler.Models.Directives.Abstract;
using MIPS.Assembler.Parsers;
using MIPS.Assembler.Tokenization;
using System.Runtime.CompilerServices;
using System.Text;

namespace MIPS.Assembler.Tests.Parsers;

// TODO: Test expressions in directive args

[TestClass]
public class DirectiveParserTests
{
    private const string Global = ".globl main";

    private const string Byte = ".byte 10";
    private const string Word = ".word 10";
    private const string Bytes = ".byte 10, 10";
    private const string Ascii = ".ascii \"Test String\"";
    private const string Asciiz = ".asciiz \"Test String\"";

    [TestMethod(Global)]
    public void GlobalTest() => RunGlobalTest(Global, "main");

    [TestMethod(Byte)]
    public void ByteTest() => RunDataTest(Byte, 10);

    [TestMethod(Word)]
    public void WordTest() => RunDataTest(Word, 0, 0, 0, 10);

    [TestMethod(Bytes)]
    public void BytesTest() => RunDataTest(Bytes, 10, 10);

    [TestMethod(Ascii)]
    public void AsciiTest() => RunDataTest(Ascii, Encoding.ASCII.GetBytes("Test String"));

    [TestMethod(Asciiz)]
    public void AsciizTest() => RunDataTest(Asciiz, Encoding.ASCII.GetBytes("Test String\0"));

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
