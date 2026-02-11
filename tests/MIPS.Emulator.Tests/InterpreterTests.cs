// Avishai Dernis 2025

using MIPS.Emulator.Config;
using MIPS.Emulator.Executor.Enum;
using MIPS.Models.Instructions.Enums.Registers;
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
        var path = TestFilePathing.GetAssemblyFilePath("emulator_tests/usercode_tests/print_int.asm");
        var stream = File.Open(path, FileMode.Open);

        // Run assembler, and assert successful assembly
        var result = await Assembler.Assembler.AssembleAsync<ElfModule, ElfConfig>(stream, path, new ElfConfig());
        Assert.IsNotNull(result.ObjectModule);

        // Setup interpreter
        var module = result.ObjectModule;
        var config = new EmulatorConfig()
        {
            HostedTraps = true
        };
        var emulator = new Emulator(config);
        emulator.Computer.Processor.ProgramCounter = module.EntryAddress;
        emulator.Load(module);

        // Register for traps (we're gonna handle the syscall)
        emulator.Computer.Processor.TrapOccurring += (p, e) =>
        {
            // Assert the emulator is not handling the syscall (interpreter mode)
            Assert.IsTrue(e.Unhandled);

            // Assert the trap was a syscall
            Assert.IsTrue(e.Trap is TrapKind.Syscall);

            // Assert the syscall requested is to print the integer 42
            Assert.IsTrue(p.RegisterFile[GPRegister.ReturnValue0] is 1);
            Assert.IsTrue(p.RegisterFile[GPRegister.Argument0] is 42);

            // Kill the emulator and end the test
            emulator.ShutDown();
        };

        // Start the emulator
        emulator.Start();
    }
}
