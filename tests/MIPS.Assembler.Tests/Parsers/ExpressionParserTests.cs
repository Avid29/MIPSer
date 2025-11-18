// Adam Dernis 2024

using Microsoft.VisualStudio.TestTools.UnitTesting;
using MIPS.Assembler.Models;
using MIPS.Assembler.Models.Modules;
using MIPS.Assembler.Parsers;
using MIPS.Assembler.Tokenization;
using MIPS.Assembler.Tokenization.Models.Enums;
using MIPS.Models.Addressing;
using MIPS.Models.Addressing.Enums;
using MIPS.Models.Modules.Tables.Enums;

namespace MIPS.Assembler.Tests.Parsers;

[TestClass]
public class ExpressionParserTests
{
    [DataTestMethod]
    [DataRow("10", 10)]
    [DataRow("-10", -10)]
    [DataRow("10 + 10", 10 + 10)]
    [DataRow("25 - 10", 25 - 10)]
    [DataRow("4 * 4", 4 * 4)]
    [DataRow("8 / 2", 8 / 2)]
    [DataRow("8 % 3", 8 % 3)]
    [DataRow("9 & 3", 9 & 3)]
    [DataRow("9 | 3", 9 | 3)]
    [DataRow("9 ^ 3", 9 ^ 3)]
    [DataRow("~10", ~10)]
    [DataRow("10 * -10", 10 * -10)]
    [DataRow("0b1010", 0b1010)]
    [DataRow("0o12", 10)]   // C# Doesn't support oct
    [DataRow("0xa", 0xa)]
    [DataRow("4 * 2 + 2", 4 * 2 + 2)]
    [DataRow("4 + 2 * 2", 4 + 2 * 2)]
    [DataRow("(4 + 2) * 2", (4 + 2) * 2)]
    [DataRow("'a'", 'a')]
    [DataRow(@"'\n'", '\n')]
    [DataRow("'a' + 10", 'a' + 10)]
    public void ExpressionSuccessTests(string input, int expected, params (string name, Address addr)[] macros)
    => RunTest(input, expected);

    [DataTestMethod]
    [DataRow("+")]
    [DataRow("*10")]
    [DataRow("10-")]
    [DataRow("-*10")]
    [DataRow("10 10")]
    [DataRow("0b102")]
    [DataRow("4 + 2) * 2")]
    [DataRow("(4 + 2 * 2")]
    [DataRow("'abc'")]
    [DataRow(@"'\x'")]
    public void ExpressionFailureTests(string input)
        => RunTest(input);

    private const string Macro = "macro + 10";
    private const string MacroFail = "macro + macro";

    [TestMethod(Macro)]
    public void MarcoTest() => RunTest(Macro, 10 + 10, ("macro", new Address(10, Section.Text)));

    [TestMethod(MacroFail)]
    public void MarcoFailTest() => RunTest(MacroFail, null, ("macro", new Address(10, Section.Text)));

    private static void RunTest(string input, long? expected = null, params (string name, Address addr)[] macros)
    {
        // NOTE: This assumes symbol realization is not implemented!
        var obj = new ModuleConstructor();
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
        bool success = ExpressionParser.TryParse(line.Tokens, out var actual, out _, context);
        Assert.AreEqual(success, expected.HasValue);
        if (expected.HasValue)
        {
            Assert.AreEqual(expected.Value, actual.Value);
        }
    }
}
