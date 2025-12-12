// Avishai Dernis 2025

using MIPS.Assembler.Logging.Enum;
using MIPS.Assembler.Models.Config;
using MIPS.Models.Instructions.Enums;
using MIPS.Tests.Formats;
using MIPS.Tests.Helpers;
using ObjectFiles.Elf;
using System.Threading.Tasks;

namespace Elf.Tests;

[TestClass]
public class AssemblerTests : AssemblerTests<ElfModule, AssemblerConfig>
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
    public async Task DisabledPseudoInstructionsErrorTest() =>
        await RunStringTest(DisabledPseudoInstructions, new() { PseudoInstructionPermissibility = MIPS.Assembler.Models.Enums.PseudoInstructionPermissibility.Blacklist }, LogCode.DisabledFeatureInUse);

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
}
