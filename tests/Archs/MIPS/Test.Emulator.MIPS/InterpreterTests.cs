// Avishai Dernis 2025

using Test.MIPS.Helpers;
using ObjectFiles.Elf;
using ObjectFiles.Elf.Config;
using System.IO;
using System.Threading.Tasks;
using Zarem.Assembler.MIPS;
using Zarem.Emulator.MIPS.Config;
using Zarem.Emulator.MIPS.Interpreter;

namespace Test.Emulator.MIPS;

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
        var result = await Assembler.AssembleAsync(stream, path, elfConfig);
        Assert.IsNotNull(result.AbstractModule);

        // Link
        var module = Zarem.Linker.MIPS.Linker<ElfModule, ElfConfig>.Link(elfConfig, "entry", result.AbstractModule);
        var elfModule = ElfModule.Create(module, elfConfig);
        Assert.IsNotNull(elfModule);

        // Setup emulator
        var emulatorConfig = new EmulatorConfig()
        {
            HostedTraps = true
        };
        var emulator = new Zarem.Emulator.MIPS.Emulator(emulatorConfig);
        emulator.Computer.Processor.ProgramCounter = elfModule.EntryAddress;
        emulator.Load(elfModule);

        // Setup interpreter
        var interpreter = new MARSInterpreter(emulator.Computer);

        // Start the emulator
        emulator.Start();
    }
}
