// Avishai Dernis 2025

using MIPS.Assembler.Models.Config;
using MIPS.Tests.Helpers;
using ObjectFiles.Elf;
using ObjectFiles.Elf.Config;
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
        var result = await Assembler.Assembler.AssembleAsync<ElfModule, ElfConfig>(stream, path, new ElfConfig());

        // Write the module and assert validity
        Assert.IsNotNull(result.ObjectModule);

        // Setup interpreter
        var module = result.ObjectModule;
        var interpreter = new Interpreter
        {
            ProgramCounter = module.EntryAddress
        };
        interpreter.Load(module);

        // Step 3 instructions
        interpreter.StepInstruction();
        interpreter.StepInstruction();
        interpreter.StepInstruction();
    }
}
