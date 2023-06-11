// Adam Dernis 2023

using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;

namespace MIPS.Assembler.Tests;

[TestClass]
public class InstructionTests
{
    [TestMethod]
    public void TestMethod1()
    {
        string code = @"beq $t0, $s0, -100";

        using var stream = new MemoryStream();
        using var writer = new StreamWriter(stream);
        writer.Write(code);
        writer.Flush();
        stream.Position = 0;

        var result = Assembler.AssembleAsync(stream);
    }
}
