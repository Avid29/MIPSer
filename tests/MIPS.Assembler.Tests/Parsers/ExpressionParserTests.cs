// Adam Dernis 2024

using Microsoft.VisualStudio.TestTools.UnitTesting;
using MIPS.Assembler.Models;
using MIPS.Assembler.Models.Modules;
using MIPS.Assembler.Parsers;
using MIPS.Assembler.Tokenization;
using MIPS.Assembler.Tokenization.Models.Enums;
using MIPS.Models.Addressing;
using MIPS.Models.Modules.Tables.Enums;
using System.Collections.Generic;

namespace MIPS.Assembler.Tests.Parsers;

[TestClass]
public class ExpressionParserTests
{
    public static IEnumerable<object[]> ExpressionSuccessTestsList =>
    [
        ["10", 10],
        ["-10", -10],
        ["10", 10],
        ["25 - 10", 25 - 10],
        ["4 * 4", 4 * 4],
        ["8 / 2", 8 / 2],
        ["8 % 3", 8 % 3],
        ["9 & 3", 9 & 3],
        ["9 | 3", 9 | 3],
        ["9 ^ 3", 9 ^ 3],
        ["~10", ~10],
        ["10 * -10", 10 * -10],
        ["0b1010", 0b1010],
        ["0o12", 10], // C# Doesn't support oct
        ["0xa", 0xa],
        ["4 * 2 + 2", 4 * 2 + 2],
        ["4 + 2 * 2", 4 + 2 * 2],
        ["(4 + 2) * 2", (4 + 2) * 2],
        ["'a'", 'a'],
        [@"'\n'", '\n'],
        ["'a' + 10", 'a' + 10],
        ["macro + 10", 10 + 10, ("macro", new Address(10, ".text"))],
    ];

    public static IEnumerable<object[]> ExpressionFailureTestsList =>
    [
        ["+"],
        ["*10"],
        ["10-"],
        ["-*10"],
        ["10 10"],
        ["0b102"],
        ["4 + 2) * 2"],
        ["(4 + 2 * 2"],
        ["'abc'"],
        [@"'\x'"],
        ["macro + macro", ("macro", new Address(10, ".text"))],
    ];

    [DataTestMethod]
    [DynamicData(nameof(ExpressionSuccessTestsList))]
    public void ExpressionSuccessTests(string input, int expected, params (string name, Address addr)[] macros)
        => RunTest(input, expected, macros);

    [DataTestMethod]
    [DynamicData(nameof(ExpressionFailureTestsList))]
    public void ExpressionFailureTests(string input, params (string name, Address addr)[] macros)
        => RunTest(input, macros: macros);

    private static void RunTest(string input, long? expected = null, params (string name, Address addr)[] macros)
    {
        // NOTE: This assumes symbol realization is not implemented!
        var obj = new Module();
        foreach (var (name, addr) in macros)
        {
            obj.TryDefineSymbol(name, SymbolType.Macro, addr);
        }

        var context = new AssemblerContext(obj);
        RunTest(input, expected, context);
    }

    private static void RunTest(string input, long? expected = null, AssemblerContext? context = null)
    {
        var line = Tokenizer.TokenizeLine(input, nameof(RunTest), TokenizerMode.Expression);
        bool success = ExpressionParser.TryParse(line.Tokens, out var actual, context);
        Assert.AreEqual(success, expected.HasValue);
        if (expected.HasValue)
        {
            Assert.AreEqual(expected.Value, actual.Value.Value);
        }
    }
}
