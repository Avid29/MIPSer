// Adam Dernis 2023

using Microsoft.VisualStudio.TestTools.UnitTesting;
using MIPS.Assembler.Tokenization;
using System.IO;
using System.Threading.Tasks;

namespace MIPS.Assembler.Tests;

[TestClass]
public class TokenizerTests
{
    private const string AssemblyPath = @"..\..\..\ASMs\";

    [TestMethod("test1.asm")]
    public async Task Test1() => await RunTest("test1.asm",
        //".globl", "main",
        //"main:",
        "ori", "$s0", ",", "$zero", ",", "10",
        "ori", "$s1", ",", "$zero", ",", "10",
        "add", "$t0", ",", "$s0", ",", "$s1");

    private async Task RunTest(string fileName, params string[] canon)
    {
        var fullPath = Path.Combine(AssemblyPath, fileName);
        fullPath = Path.GetFullPath(fullPath);
        var stream = File.Open(fullPath, FileMode.Open);
        await RunTest(stream, canon, fileName);
    }

    private async Task RunTest(Stream stream, string[] canon, string? fileName = null)
    {
        var results = await Tokenizer.TokenizeAsync(stream, fileName);

        Assert.AreEqual(canon.Length, results.TokenCount);
    }
}
