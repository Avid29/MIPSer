// Adam Dernis 2024

using Microsoft.VisualStudio.TestTools.UnitTesting;
using MIPS.Assembler.Tokenization;
using MIPS.Tests.Helpers;
using System.IO;
using System.Threading.Tasks;

namespace MIPS.Assembler.Tests;

[TestClass]
public class TokenizerTests
{
    [TestMethod(TestFilePathing.EmptyTestFile)]
    public async Task EmptyFileTest() => await RunFileTest(TestFilePathing.EmptyTestFile);

    [TestMethod(TestFilePathing.InstructionsTestFile)]
    public async Task InstructionsFileTest() => await RunFileTest(TestFilePathing.InstructionsTestFile,
        "ori", "$s0", ",", "$zero", ",", "10",
        "ori", "$s1", ",", "$zero", ",", "10",
        "add", "$t0", ",", "$s0", ",", "$s1");

    private static async Task RunFileTest(string testFile, params string[] canon)
    {
        // Open the file and run the test
        var path = TestFilePathing.GetAssemblyFilePath(testFile);
        var stream = File.Open(path, FileMode.Open);
        await RunTest(stream, canon, testFile);
    }

    private static async Task RunTest(Stream stream, string[] canon, string? fileName = null)
    {
        // Run the test and assert the expected number of tokens came back
        var results = await Tokenizer.TokenizeAsync(stream, fileName);
        Assert.AreEqual(canon.Length, results.TokenCount);

        // TODO: Assert token strings match
    }
}
