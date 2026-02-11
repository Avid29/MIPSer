// Avishai Dernis 2025

using MIPS.Assembler;
using MIPS.Linker;
using MIPS.Tests.Helpers;
using RASM.Modules;
using RASM.Modules.Config;
using System.IO;
using System.Threading.Tasks;

namespace RASM.Tests;

[TestClass]
public class LinkerTests
{
    [TestMethod]
    public async Task DefRefTest()
    {
        var defPath = TestFilePathing.GetAssemblyFilePath("linker_tests/def.asm");
        var refPath = TestFilePathing.GetAssemblyFilePath("linker_tests/ref.asm");

        Stream defStream = File.Open(defPath, FileMode.Open);
        Stream refStream = File.Open(refPath, FileMode.Open);

        var config = new RasmConfig();
        var defResult = await Assembler.AssembleAsync<RasmModule, RasmConfig>(defStream, "def.asm", config);
        var refResult = await Assembler.AssembleAsync<RasmModule, RasmConfig>(refStream, "ref.asm", config);

        var defModule = defResult.AbstractModule;
        var refModule = refResult.AbstractModule;

        Assert.IsNotNull(defModule);
        Assert.IsNotNull(refModule);

        Linker<RasmModule, RasmConfig>.Link(config, defModule, refModule);
    }
}
