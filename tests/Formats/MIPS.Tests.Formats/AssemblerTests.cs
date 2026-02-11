// Avishai Dernis 2025

using Microsoft.VisualStudio.TestTools.UnitTesting;
using MIPS.Assembler.Config;
using MIPS.Assembler.Logging.Enum;
using MIPS.Assembler.Models.Modules.Interfaces;
using MIPS.Tests.Helpers;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MIPS.Tests.Formats;

public class AssemblerTests<TModule, TConfig>
    where TModule : IBuildModule<TModule>
    where TConfig : AssemblerConfig, new()
{
    protected static async Task RunFileTest(string fileName, params (LogCode, long)[] expected)
    {
        // Load the file
        var path = TestFilePathing.GetAssemblyFilePath(fileName);
        var stream = File.Open(path, FileMode.Open);

        // Run the test
        await RunTest(stream, fileName, null, expected);
    }

    protected static async Task RunStringTest(string str, TConfig? config = null, params LogCode[] expected)
    {
        // Wrap the test in a stream and run the test
        var stream = new MemoryStream(Encoding.Default.GetBytes(str));
        await RunTest(stream, null, config, [.. expected.Select((x) => (x, 1L))]);
    }

    protected static async Task RunTest(Stream stream, string? filename = null, TConfig? config = null, params (LogCode, long)[] expected)
    {
        // Load output file
        //var output = TestFilePathing.GetMatchingObjectFilePath(filename);
        //Stream result = File.Open(output, FileMode.OpenOrCreate);

        // Run assembler
        var result = await Assembler.Assembler.AssembleAsync<TModule, TConfig>(stream, filename, config ?? new TConfig());

        // Find expected errors, warnings, and messages
        if (expected.Length == result.Logs.Count)
        {
            foreach (var (code, line) in expected)
            {
                var logEntry = result.Logs.FirstOrDefault(x => x.Code == code && x.Location.Line == line);
                Assert.IsNotNull(logEntry, $"Could not find matching {code} error on line {line}");
            }
        }

        // Don't run output validation for fileless tests
        if (filename is null)
            return;

        // Assembly failed. No expected output
        if (result.Failed)
            return;

        // Write the module and assert validity
        var constructor = result.ObjectModule?.Abstract(config ?? new TConfig());
    }
}
