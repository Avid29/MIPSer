// Adam Dernis 2023

using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using System.Threading.Tasks;

namespace MIPS.Assembler.Tests;

[TestClass]
public class AssemblerTests
{
    [TestMethod]
    public async Task Test()
    {
        Stream stream = new MemoryStream();
        await using var writer = new StreamWriter(stream);
        await writer.WriteLineAsync("test:");
        await writer.WriteLineAsync(".text");
        await writer.WriteLineAsync("addi $t0, $s0, 10");
        await writer.FlushAsync();

        stream.Position = 0;

        var module = Assembler.AssembleAsync(stream);
    }
}
