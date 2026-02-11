// Avishai Dernis 2025

using CommunityToolkit.Diagnostics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Test.MIPS.Helpers;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Zarem.Assembler.MIPS;
using Zarem.Assembler.MIPS.Config;
using Zarem.Assembler.MIPS.Models.Modules.Interfaces;

namespace Test.ObjFormats;

public class AbstractionTests<TModule, TConfig>
    where TModule : IBuildModule<TModule>
    where TConfig : AssemblerConfig, new()
{
    protected static async Task RunFileTest(string fileName, TConfig? config = null)
    {
        // Load the file
        var path = TestFilePathing.GetAssemblyFilePath(fileName);
        var stream = File.Open(path, FileMode.Open);

        // Run the test
        await RunTest(stream, fileName, config);
    }

    protected static async Task RunTest(Stream stream, string? filename = null, TConfig? config = null)
    {
        config ??= new TConfig();

        var assemblyResult = await Assembler.AssembleAsync<TModule, TConfig>(stream, filename, config);
        Guard.IsNotNull(assemblyResult.Module);
        Guard.IsNotNull(assemblyResult.AbstractModule);

        var reconvertedAbstractModule = assemblyResult.ObjectModule?.Abstract(config);
        Guard.IsNotNull(reconvertedAbstractModule);

        // Compare original and compare
        var original = assemblyResult.AbstractModule;
        var compare = reconvertedAbstractModule;

        foreach (var (key, value) in original.Symbols)
        {
            if (!compare.Symbols.TryGetValue(key, out var symbol))
                Assert.Fail();

            Assert.AreEqual(value.IsDefined, symbol.IsDefined);
        }

        foreach(var @ref in original.References)
        {
            var matchingRef = compare.References.FirstOrDefault(r => r.Location == @ref.Location && r.Type == @ref.Type);

            Assert.IsNotNull(matchingRef);
            Assert.AreEqual(@ref.Symbol, matchingRef.Symbol);
        }
    }
}
