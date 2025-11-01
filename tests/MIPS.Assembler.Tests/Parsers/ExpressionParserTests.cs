// Adam Dernis 2024

using Microsoft.VisualStudio.TestTools.UnitTesting;
using MIPS.Assembler.Models;
using MIPS.Assembler.Models.Modules;
using MIPS.Assembler.Parsers;
using MIPS.Assembler.Tokenization;
using MIPS.Assembler.Tokenization.Enums;
using MIPS.Models.Addressing;
using MIPS.Models.Addressing.Enums;
using MIPS.Models.Modules.Tables.Enums;

namespace MIPS.Assembler.Tests.Parsers;

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
    private const string Not = "~10";

    private const string Binary = "0b1010";
    private const string Oct = "0o12";
    private const string Hex = "0xa";
    private const string BadBinary = "0b102";


    private const string Char = "'a'";
    private const string AddChar = "'a' + 10";

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

    [TestMethod(Not)]
    public void NotTest() => RunTest(Not, ~10);

    [TestMethod(Binary)]
    public void BinaryTest() => RunTest(Binary, 0b1010);
    
    [TestMethod(BadBinary)]
    public void BadBinaryTest() => RunTest(BadBinary);

    [TestMethod(Oct)]
    public void OctTest() => RunTest(Oct, 10); // C# doesn't support oct...

    [TestMethod(Hex)]
    public void HexTest() => RunTest(Hex, 0xa);

    [TestMethod(Char)]
    public void CharTest() => RunTest(Char, 'a');

    [TestMethod(AddChar)]
    public void AddCharTest() => RunTest(AddChar, 'a' + 10);

    [TestMethod(Macro)]
    public void MarcoTest() => RunTest(Macro, 10 + 10, ("macro", new Address(10, Section.Text)));

    [TestMethod(MacroFail)]
    public void MarcoFailTest() => RunTest(MacroFail, null, ("macro", new Address(10, Section.Text)));

    private static void RunTest(string input, long? expected = null) => RunTest(new ExpressionParser(), input, expected);

    private static void RunTest(string input, long? expected = null, params (string name, Address addr)[] macros)
    {
        // NOTE: This assumes symbol realization is not implemented!
        var obj = new ModuleConstructor();
        foreach (var (name, addr) in macros)
            obj.TryDefineSymbol(name, SymbolType.Macro, addr);

        var context = new AssemblerContext(obj);
        var parser = new ExpressionParser(context);
        RunTest(parser, input, expected);
    }

    private static void RunTest(ExpressionParser parser, string input, long? expected = null)
    {
        var line = Tokenizer.TokenizeLine(input, nameof(RunTest), TokenizerMode.Expression);
        bool success = parser.TryParse(line.Tokens, out var actual, out _);
        Assert.AreEqual(success, expected.HasValue);
        if (expected.HasValue)
        {
            Assert.AreEqual(expected.Value, actual.Value);
        }
    }
}
