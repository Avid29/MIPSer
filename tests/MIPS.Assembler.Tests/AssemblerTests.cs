// Adam Dernis 2023

using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;

namespace MIPS.Assembler.Tests;

[TestClass]
public class AssemblerTests
{
    private const string AssemblyPath = @"..\..\..\ASMs\";

    [TestMethod("test1.asm")]
    public void Test1() => RunTest("test1.asm");

    private void RunTest(string fileName)
    {
        var fullPath = Path.Combine(AssemblyPath, fileName);
        fullPath = Path.GetFullPath(fullPath);
        var stream = File.Open(fullPath, FileMode.Open);
        RunTest(stream);
    }

    private void RunTest(Stream stream)
    {
        var module = Assembler.AssembleAsync(stream);
    }
}
