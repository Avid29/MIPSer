// Adam Dernis 2023

using Microsoft.VisualStudio.TestTools.UnitTesting;
using MIPS.Assembler.Parsers;

namespace MIPS.Assembler.Tests;

[TestClass]
public class StringParserTests
{
    [TestMethod("Empty")]
    public void EmptyTest() => RunTest("", "");

    [TestMethod("\"\"")]
    public void LiteralEmptyTest() => RunTest("\"\"", "");

    [TestMethod("Hello\nWorld")]
    public void HelloWorld2LineTest() => RunTest("\"Hello\\nWorld\"", "Hello\nWorld");

    private static void RunTest(string input, string expected)
    {
        var parser = new StringParser();

        bool result = parser.TryParseString(input, out var actual);

        Assert.AreEqual(expected, actual);
    }
}
