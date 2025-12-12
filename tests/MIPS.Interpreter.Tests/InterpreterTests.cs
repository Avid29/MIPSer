// Avishai Dernis 2025

using MIPS.Assembler.Models.Config;
using MIPS.Tests.Helpers;
using ObjectFiles.Elf;
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

        // Load output file
        var output = TestFilePathing.GetMatchingObjectFilePath(path);
        Stream outStream = File.Open(output, FileMode.OpenOrCreate);

        // Run assembler
        var result = await Assembler.Assembler.AssembleAsync<ElfModule, AssemblerConfig>(stream, path, new AssemblerConfig(), outStream);

        // Write the module and assert validity
        Assert.IsNotNull(result.ObjectModule);

        // Setup interpreter
        var module = result.ObjectModule;
        var interpreter = new Interpreter
        {
            ProgramCount = module.EntryAddress
        };
        interpreter.Load(module);

        // Step 3 instructions
        interpreter.StepInstruction();
        interpreter.StepInstruction();
        interpreter.StepInstruction();
    }
}
