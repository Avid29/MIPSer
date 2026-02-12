// Avishai Dernis 2025

using System.IO;
using System.Threading.Tasks;
using Test.MIPS.Helpers;
using Zarem.Assembler;
using Zarem.Assembler.Config;
using Zarem.Linker;

namespace Test.RASM;

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

        var config = new MIPSAssemblerConfig();
        var defResult = await MIPSAssembler.AssembleAsync(defStream, "def.asm", config);
        var refResult = await MIPSAssembler.AssembleAsync(refStream, "ref.asm", config);

        var defModule = defResult.Module;
        var refModule = refResult.Module;

        Assert.IsNotNull(defModule);
        Assert.IsNotNull(refModule);

        MIPSLinker.Link(defModule, refModule);
    }
}
