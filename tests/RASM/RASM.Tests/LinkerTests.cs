// Avishai Dernis 2025

using MIPS.Assembler;
using MIPS.Linker;
using MIPS.Tests.Helpers;
using RASM.Modules;
using RASM.Modules.Config;
using System.IO;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace RASM.Tests;

[TestClass]
public class LinkerTests
{
    [TestMethod]
    public async Task DefRefTest()
    {
        var defPath = TestFilePathing.GetAssemblyFilePath("link_tests/def.asm");
        var refPath = TestFilePathing.GetAssemblyFilePath("link_tests/ref.asm");

        Stream defStream = File.Open(defPath, FileMode.Open);
        Stream refStream = File.Open(refPath, FileMode.Open);

        var config = new RasmConfig();
        var defAssembler = await Assembler.AssembleAsync(defStream, "def", config);
        var refAssembler = await Assembler.AssembleAsync(refStream, "ref", config);

        defStream = new MemoryStream();
        refStream = new MemoryStream();

        var defModule = defAssembler.CompleteModule<RasmModule>(defStream);
        var refModule = refAssembler.CompleteModule<RasmModule>(refStream);

        Assert.IsNotNull(defModule);
        Assert.IsNotNull(refModule);

        Linker.Link([defModule, refModule], config);
    }
}
