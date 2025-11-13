// Adam Dernis 2024

using MIPS.Assembler;
using MIPS.Assembler.Logging.Enum;
using MIPS.Models.Instructions.Enums;
using MIPS.Tests.Helpers;
using RASM.Modules;
using RASM.Modules.Config;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RASM.Tests;

[TestClass]
public class AssemblerTests
{
    private const string InvalidInstruction = "xkcd $t0, $t1, 0";
    private const string MissingInstruction = "$s2, $t4, $t2";
    private const string InvalidLabelNum = "2point: nop";
    private const string ExtraArgError = "add $s0, $t0, $t2, 2";
    private const string MissingArgError = "add $s0, $t0";
    private const string NumericalRegister = "addi $s0, $16, 16";
    private const string NumericalRegisterError = "addi $s0, $56, 16";
    private const string DisabledPseudoInstructions = "move $s0, $t0";
    private const string NotInVersion = "sync";

    [TestMethod(nameof(InvalidInstruction))]
    public async Task InvalidInstructionTest() => await RunStringTest(InvalidInstruction, expected: LogCode.InvalidInstructionName);

    [TestMethod(nameof(MissingInstruction))]
    public async Task MissingInstructionTest() => await RunStringTest(MissingInstruction, expected: LogCode.UnexpectedToken);

    [TestMethod(nameof(InvalidLabelNum))]
    public async Task InvalidLabelNumTest() => await RunStringTest(InvalidLabelNum, expected: LogCode.IllegalSymbolName);

    [TestMethod(nameof(ExtraArgError))]
    public async Task ExtraArgErrorTest() => await RunStringTest(ExtraArgError, expected: LogCode.InvalidInstructionArgCount);

    [TestMethod(nameof(MissingArgError))]
    public async Task MissingArgErrorTest() => await RunStringTest(MissingArgError, expected: LogCode.InvalidInstructionArgCount);

    [TestMethod(nameof(NumericalRegister))]
    public async Task NumericalRegisterTest() => await RunStringTest(NumericalRegister);

    [TestMethod(nameof(NumericalRegisterError))]
    public async Task NumericalRegisterErrorTest() => await RunStringTest(NumericalRegisterError, expected: LogCode.InvalidRegisterArgument);

    [TestMethod(nameof(DisabledPseudoInstructions))]
    public async Task DisabledPseudoInstructionsErrorTest() => await RunStringTest(DisabledPseudoInstructions, new() { AllowPseudos = false }, LogCode.DisabledFeatureInUse);

    [TestMethod(nameof(NotInVersion))]
    public async Task NotInVersionTest() => await RunStringTest(NotInVersion, new(MipsVersion.MipsI), LogCode.NotInVersion);

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
        (LogCode.InvalidInstructionArgCount, 14),
        (LogCode.InvalidInstructionName, 16),
        (LogCode.UnparsableExpression, 19),
        (LogCode.InvalidRegisterArgument, 24), // Debatably should be an InvalidAddressOffsetArgument error
        (LogCode.ZeroRegWriteback, 29),
        (LogCode.IntegerTruncated, 30),
        (LogCode.InvalidInstructionArg, 31),
        (LogCode.InvalidRegisterArgument, 32),
        (LogCode.InvalidCharLiteral, 35));

    [TestMethod(TestFilePathing.DuplicateSymbolFile)]
    public async Task DuplicateSymbolTest() => await RunFileTest(TestFilePathing.DuplicateSymbolFile,
        (LogCode.DuplicateSymbolDefinition, 15));

    [TestMethod(TestFilePathing.SubtractAddressFile)]
    public async Task SubtractAddressTest() => await RunFileTest(TestFilePathing.SubtractAddressFile,
        (LogCode.InvalidExpressionOperation, 14));

    private static async Task RunFileTest(string fileName, params (LogCode, long)[] expected)
    {
        // Load the file
        var path = TestFilePathing.GetAssemblyFilePath(fileName);
        var stream = File.Open(path, FileMode.Open);

        // Run the test
        await RunTest(stream, fileName, null, expected);
    }

    private static async Task RunStringTest(string str, RasmConfig? config = null, params LogCode[] expected)
    {
        // Wrap the test in a stream and run the test
        var stream = new MemoryStream(Encoding.Default.GetBytes(str));
        await RunTest(stream, null, config, [..expected.Select((x) => (x, 1L))]);
    }

    private static async Task RunTest(Stream stream, string? filename = null, RasmConfig? config = null, params (LogCode, long)[] expected)
    {
        // Load output file
        //var output = TestFilePathing.GetMatchingObjectFilePath(filename);
        //Stream result = File.Open(output, FileMode.OpenOrCreate);

        // Run assembler
        var result = await Assembler.AssembleAsync<RasmModule, RasmConfig>(stream, filename, config ?? new RasmConfig());

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
        var constructor = result.ObjectModule?.Abstract(config ?? new RasmConfig());
    }
}
