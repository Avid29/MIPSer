// Avishai Dernis 2025

using MIPS.Assembler.Models;
using MIPS.Tests.Helpers;
using Raw.Modules;
using System.IO;
using System.Threading.Tasks;

namespace MIPS.Interpreter.Tests;

[TestClass]
public class InterpreterTests
{
    [TestMethod]
    public async Task RunTest1()
    {
        // Load the file
        var path = TestFilePathing.GetAssemblyFilePath("sample_tests/test1.asm");
        var stream = File.Open(path, FileMode.Open);

        // Run assembler
        var assembler = await Assembler.Assembler.AssembleAsync(stream, path, new AssemblerConfig());

        // Load output file
        var output = TestFilePathing.GetMatchingObjectFilePath(path);
        Stream result = File.Open(output, FileMode.OpenOrCreate);

        // Write the module and assert validity
        var module = assembler.CompleteModule<RawModule>(result);
        Assert.IsNotNull(module);

        var interpreter = new Interpreter(module);
        interpreter.StepInstruction();
        interpreter.StepInstruction();
        interpreter.StepInstruction();
    }
}
