// Adam Dernis 2024

using Microsoft.VisualStudio.TestTools.UnitTesting;
using MIPS.Assembler.Parsers;

namespace MIPS.Assembler.Tests.Parsers;

[TestClass]
public class StringParserTests
{
    [TestMethod("Empty")]
    public void EmptyTest() => RunTest("", null);

    [TestMethod("\"\"")]
    public void LiteralEmptyTest() => RunTest("\"\"", "");

    [TestMethod("Hello\nWorld")]
    public void HelloWorld2LineTest() => RunTest("\"Hello\\nWorld\"", "Hello\nWorld");

    private static void RunTest(string input, string? expected = null)
    {
        // Declare parser and attempt parsing
        var parser = new StringParser();
        if(!parser.TryParseString(input, out var actual))
        {
            Assert.IsNull(expected);
            return;
        }

        // Assert the result matches the expected
        Assert.AreEqual(expected, actual);
    }
}
