// Adam Dernis 2023

using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using System.Threading.Tasks;

namespace MIPS.Assembler.Tests;

[TestClass]
public class AssemblerTests
{
    private const string AssemblyPath = @"..\..\..\ASMs\";

    [TestMethod("test1.asm")]
    public async Task Test1() => await RunTest("test1.asm");

    [TestMethod("failed1.asm")]
    public async Task Fail1() => await RunTest("fail1.asm");

    private async Task RunTest(string fileName)
    {
        var fullPath = Path.Combine(AssemblyPath, fileName);
        fullPath = Path.GetFullPath(fullPath);
        var stream = File.Open(fullPath, FileMode.Open);
        await RunTest(stream);
    }

    private async Task RunTest(Stream stream)
    {
        var module = await Assembler.AssembleAsync(stream);

        var result = new MemoryStream();
        module.WriteModule(result);
    }
}
