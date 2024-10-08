// Adam Dernis 2023

using MIPS.Helpers.Instructions;
using MIPS.Models.Instructions.Enums;

namespace MIPS.Assembler.Models.Instructions;

/// <summary>
/// A struct containing metadata on an instruction.
/// </summary>
public readonly struct InstructionMetadata
{
    /// <summary>
    /// Initializes a new instance of the <see cref="InstructionMetadata"/> struct.
    /// </summary>
    public InstructionMetadata(string name, OperationCode opCode, Argument[] argumentPattern)
    {
        Name = name;
        OpCode = opCode;
        FuncCode = FunctionCode.None;
        BranchCode = BranchCode.None;
        ArgumentPattern = argumentPattern;
        IsPseudoInstruction = false;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="InstructionMetadata"/> struct.
    /// </summary>
    public InstructionMetadata(string name, FunctionCode funcCode, Argument[] argumentPattern)
    {
        Name = name;
        OpCode = OperationCode.RType;
        FuncCode = funcCode;
        BranchCode = BranchCode.None;
        ArgumentPattern = argumentPattern;
        IsPseudoInstruction = false;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="InstructionMetadata"/> struct.
    /// </summary>
    public InstructionMetadata(string name, BranchCode branchCode, Argument[] argumentPattern)
    {
        Name = name;
        OpCode = OperationCode.BranchConditional;
        FuncCode = FunctionCode.None;
        BranchCode = branchCode;
        ArgumentPattern = argumentPattern;
        IsPseudoInstruction = false;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="InstructionMetadata"/> struct.
    /// </summary>
    /// <remarks>
    /// This is constructor is only for pseudo-instructions.
    /// </remarks>
    public InstructionMetadata(string name, Argument[] argumentPattern)
    {
        Name = name;
        OpCode = OperationCode.PseudoInstruction;
        FuncCode = FunctionCode.None;
        BranchCode = BranchCode.None;
        ArgumentPattern = argumentPattern;
        IsPseudoInstruction = true;
    }

    /// <summary>
    /// Gets the name of the instruction.
    /// </summary>
    public string Name { get; }

    /// <summary>
    /// Gets the instruction operation code.
    /// </summary>
    public OperationCode OpCode { get; }

    /// <summary>
    /// Gets the instruction function code.
    /// </summary>
    public FunctionCode FuncCode { get; }

    /// <summary>
    /// Gets the instruction branch code.
    /// </summary>
    public BranchCode BranchCode { get; }

    /// <summary>
    /// Gets the function type.
    /// </summary>
    public readonly InstructionType Type => InstructionTypeHelper.GetInstructionType(OpCode);

    /// <summary>
    /// Gets the instruction parse type
    /// </summary>
    public Argument[] ArgumentPattern { get; }

    /// <summary>
    /// Gets whether or not the instruction is a pseudo instruction.
    /// </summary>
    public bool IsPseudoInstruction { get; }
}
