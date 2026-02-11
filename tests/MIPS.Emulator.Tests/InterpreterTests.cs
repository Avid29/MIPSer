// Avishai Dernis 2025

using MIPS.Emulator.Config;
using MIPS.Emulator.Interpreter;
using MIPS.Tests.Helpers;
using ObjectFiles.Elf;
using ObjectFiles.Elf.Config;
using System.IO;
using System.Threading.Tasks;

namespace MIPS.Emulator.Tests;

[TestClass]
public class InterpreterTests
{
    [TestMethod]
    public async Task RunPrintIntTest()
    {
        // Load the file
        var path = TestFilePathing.GetAssemblyFilePath("emulator_tests/usercode_tests/hello_world.asm");
        var stream = File.Open(path, FileMode.Open);

        var elfConfig = new ElfConfig();

        // Run assembler, and assert successful assembly
        var result = await Assembler.Assembler.AssembleAsync(stream, path, elfConfig);
        Assert.IsNotNull(result.AbstractModule);

        // Link
        var module = Linker.Linker<ElfModule, ElfConfig>.Link(elfConfig, "entry", result.AbstractModule);
        var elfModule = ElfModule.Create(module, elfConfig);
        Assert.IsNotNull(elfModule);

        // Setup emulator
        var emulatorConfig = new EmulatorConfig()
        {
            HostedTraps = true
        };
        var emulator = new Emulator(emulatorConfig);
        emulator.Computer.Processor.ProgramCounter = elfModule.EntryAddress;
        emulator.Load(elfModule);

        // Setup interpreter
        var interpreter = new MARSInterpreter(emulator.Computer);

        // Start the emulator
        emulator.Start();
    }
}
