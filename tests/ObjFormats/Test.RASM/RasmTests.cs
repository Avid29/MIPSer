// Adam Dernis 2024

using System.Threading.Tasks;
using Test.MIPS.Helpers;
using Test.ObjFormats;
using Zarem.Assembler.Logging.Enum;
using Zarem.Assembler.Models.Enums;
using Zarem.Models.Instructions.Enums;

namespace Test.RASM;

[TestClass]
public class RasmTests : AssemblerTests
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
    public async Task InvalidInstructionTest() => await RunStringTest(InvalidInstruction, expected: LogId.InvalidInstructionName);

    [TestMethod(nameof(MissingInstruction))]
    public async Task MissingInstructionTest() => await RunStringTest(MissingInstruction, expected: LogId.UnexpectedToken);

    [TestMethod(nameof(InvalidLabelNum))]
    public async Task InvalidLabelNumTest() => await RunStringTest(InvalidLabelNum, expected: LogId.IllegalSymbolName);

    [TestMethod(nameof(ExtraArgError))]
    public async Task ExtraArgErrorTest() => await RunStringTest(ExtraArgError, expected: LogId.InvalidInstructionArgCount);

    [TestMethod(nameof(MissingArgError))]
    public async Task MissingArgErrorTest() => await RunStringTest(MissingArgError, expected: LogId.InvalidInstructionArgCount);

    [TestMethod(nameof(NumericalRegister))]
    public async Task NumericalRegisterTest() => await RunStringTest(NumericalRegister);

    [TestMethod(nameof(NumericalRegisterError))]
    public async Task NumericalRegisterErrorTest() => await RunStringTest(NumericalRegisterError, expected: LogId.InvalidRegisterArgument);

    [TestMethod(nameof(DisabledPseudoInstructions))]
    public async Task DisabledPseudoInstructionsErrorTest() =>
        await RunStringTest(DisabledPseudoInstructions, new() { PseudoInstructionPermissibility = PseudoInstructionPermissibility.Blacklist }, LogId.DisabledFeatureInUse);

    [TestMethod(nameof(NotInVersion))]
    public async Task NotInVersionTest() => await RunStringTest(NotInVersion, new(MipsVersion.MipsI), LogId.NotInVersion);

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
        (LogId.InvalidRegisterArgument, 24), // Debatably should be an InvalidAddressOffsetArgument error
        (LogId.ZeroRegWriteback, 29),
        (LogId.IntegerTruncated, 30),
        (LogId.InvalidInstructionArg, 31),
        (LogId.InvalidRegisterArgument, 32),
        (LogId.InvalidCharLiteral, 35));

    [TestMethod(TestFilePathing.DuplicateSymbolFile)]
    public async Task DuplicateSymbolTest() => await RunFileTest(TestFilePathing.DuplicateSymbolFile,
        (LogId.DuplicateSymbolDefinition, 15));

    [TestMethod(TestFilePathing.SubtractAddressFile)]
    public async Task SubtractAddressTest() => await RunFileTest(TestFilePathing.SubtractAddressFile,
        (LogId.InvalidExpressionOperation, 14));
}
