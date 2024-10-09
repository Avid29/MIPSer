// Adam Dernis 2023

using Microsoft.VisualStudio.TestTools.UnitTesting;
using MIPS.Assembler.Tests.Helpers;
using MIPS.Assembler.Tokenization;
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
        var path = TestFilePathing.GetAssemblyFilePath(testFile);
        var stream = File.Open(path, FileMode.Open);
        await RunTest(stream, canon, testFile);
    }

    private static async Task RunTest(Stream stream, string[] canon, string? fileName = null)
    {
        var results = await Tokenizer.TokenizeAsync(stream, fileName);
        Assert.AreEqual(canon.Length, results.TokenCount);
    }
}
