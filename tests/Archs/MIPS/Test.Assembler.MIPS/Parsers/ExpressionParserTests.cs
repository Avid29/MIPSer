// Adam Dernis 2024

using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using Zarem.Assembler.MIPS.Tokenization;
using Zarem.Assembler.Models;
using Zarem.Assembler.Parsers;
using Zarem.Assembler.Tokenization.Models.Enums;
using Zarem.Models.Addressing;
using Zarem.Models.Modules;
using Zarem.Models.Modules.Tables.Enums;

namespace Test.Assembler.MIPS.Parsers;

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

        var context = new MIPSAssemblerContext(obj);
        RunTest(input, expected, context);
    }

    private static void RunTest(string input, long? expected = null, MIPSAssemblerContext? context = null)
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
