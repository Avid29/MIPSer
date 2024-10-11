// Adam Dernis 2024

using Microsoft.VisualStudio.TestTools.UnitTesting;
using MIPS.Assembler.Logging.Enum;
using MIPS.Assembler.Tests.Helpers;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MIPS.Assembler.Tests;

[TestClass]
public class AssemblerTests
{
    private const string InvalidInstruction = "xkcd $t0, $t1, 0";
    private const string InvalidLabelNum = "2point: nop";
    private const string InvalidLabelSpecial = "te$t: nop";
    private const string ExtraArgError = "add $s0, $t0, $t2, 2";
    private const string MissingArgError = "add $s0, $t0";

    [TestMethod(nameof(InvalidInstruction))]
    public async Task InvalidInstructionTest() => await RunStringTest(InvalidInstruction, LogId.InvalidInstructionName);

    [TestMethod(nameof(InvalidLabelNum))]
    public async Task InvalidLabelNumTest() => await RunStringTest(InvalidLabelNum, LogId.IllegalSymbolName);

    [TestMethod(nameof(InvalidLabelSpecial))]
    public async Task InvalidLabelSpecialTest() => await RunStringTest(InvalidLabelSpecial, LogId.IllegalSymbolName);

    [TestMethod(nameof(ExtraArgError))]
    public async Task ExtraArgErrorTest() => await RunStringTest(ExtraArgError, LogId.InvalidInstructionArgCount);

    [TestMethod(nameof(MissingArgError))]
    public async Task MissingArgErrorTest() => await RunStringTest(MissingArgError, LogId.InvalidInstructionArgCount);

    [TestMethod(TestFilePathing.BranchLiteralFile)]
    public async Task BranchLiteralFileTest() => await RunFileTest(TestFilePathing.BranchLiteralFile);

    [TestMethod(TestFilePathing.BranchRelativeFile)]
    public async Task BranchRelativeFileTest() => await RunFileTest(TestFilePathing.BranchRelativeFile);

    [TestMethod(TestFilePathing.EmptyTestFile)]
    public async Task EmptyFileTest() => await RunFileTest(TestFilePathing.EmptyTestFile);

    [TestMethod(TestFilePathing.InstructionsTestFile)]
    public async Task InstructionsFileTest() => await RunFileTest(TestFilePathing.InstructionsTestFile);

    [TestMethod(TestFilePathing.PseudoInstructionsTestFile)]
    public async Task PseudoInstructionsFileTest() => await RunFileTest(TestFilePathing.PseudoInstructionsTestFile);

    [TestMethod(TestFilePathing.PlaygroundTestFile1)]
    public async Task PlaygroundTest() => await RunFileTest(TestFilePathing.PlaygroundTestFile1);

    [TestMethod(TestFilePathing.CompositeFailTestFile)]
    public async Task CompositeFailTest() => await RunFileTest(TestFilePathing.CompositeFailTestFile,
        (LogId.InvalidInstructionArgCount, 14),
        (LogId.InvalidInstructionName, 16),
        (LogId.UnparsableExpression, 19),
        (LogId.InvalidAddressOffsetArgument, 24), // Debateably should be an register error
        (LogId.ZeroRegWriteBack, 29),
        (LogId.IntegerTruncated, 30),
        (LogId.InvalidRegisterArgument, 32),
        (LogId.InvalidCharLiteral, 35),
        (LogId.InvalidInstructionArgCount, 35));

    [TestMethod(TestFilePathing.DuplicateSymbolFile)]
    public async Task DuplicateSymbolTest () => await RunFileTest(TestFilePathing.DuplicateSymbolFile,
        (LogId.DuplicateSymbolDefinition, 15));

    [TestMethod(TestFilePathing.SubtractAddressFile)]
    public async Task SubtractAddressTest () => await RunFileTest(TestFilePathing.SubtractAddressFile,
        (LogId.InvalidOperationOnRelocatable, 14));

    private static async Task RunFileTest(string fileName, params (LogId, long)[] expected)
    {
        // Load the file
        var path = TestFilePathing.GetAssemblyFilePath(fileName);
        var stream = File.Open(path, FileMode.Open);

        // Run the test
        await RunTest(stream, fileName, expected);
    }

    private static async Task RunStringTest(string str, params LogId[] expected)
    {
        // Wrap the test in a stream and run the test
        var stream = new MemoryStream(Encoding.Default.GetBytes(str));
        await RunTest(stream, null, expected.Select((x) => (x, 1L)).ToArray());
    }

    private static async Task RunTest(Stream stream, string? filename = null, params (LogId, long)[] expected)
    {
        // Run assembler
        var assembler = await Assembler.AssembleAsync(stream, filename);

        // Find expected errors, warnings, and messages
        if (expected.Length != 0)
        {
            foreach (var (id, line) in expected)
            {
                var logEntry = assembler.Logs.FirstOrDefault(x => x.Id == id && x.LineNumber == line);
                Assert.IsNotNull(logEntry, $"Could not find matching {id} error on line {line}");
            }
        }

        // Don't run output validation for fileless tests
        if (filename is null)
            return;

        // Assembly failed. No expected output
        if (assembler.Failed)
            return;

        // Load output file
        var output = TestFilePathing.GetMatchingObjectFilePath(filename);
        Stream result = File.Open(output, FileMode.OpenOrCreate);

        // Write the module and assert validity
        var module = assembler.WriteModule(result);
        Assert.IsNotNull(module);
    }
}
