// Adam Dernis 2024

using MIPS.Helpers.Instructions;
using MIPS.Models.Instructions.Enums;
using MIPS.Models.Instructions.Enums.SpecialFunctions;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace MIPS.Assembler.Models.Instructions;

/// <summary>
/// A struct containing metadata on an instruction.
/// </summary>
public readonly struct InstructionMetadata
{
    private static readonly MipsVersion[] AllVersions = [MipsVersion.MipsI, MipsVersion.MipsII, MipsVersion.MipsIII, MipsVersion.MipsIV, MipsVersion.MipsV, MipsVersion.MipsVI];

    /// <summary>
    /// Initializes a new instance of the <see cref="InstructionMetadata"/> struct.
    /// </summary>
    public InstructionMetadata(string name, OperationCode opCode, Argument[] argumentPattern, params MipsVersion[] versions)
    {
        Name = name;
        OpCode = opCode;
        ArgumentPattern = argumentPattern;

        if (versions.Length is 0)
            versions = AllVersions;

        MIPSVersions = new(versions);
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="InstructionMetadata"/> struct.
    /// </summary>
    public InstructionMetadata(string name, FunctionCode funcCode, Argument[] argumentPattern, params MipsVersion[] versions)
    {
        Name = name;
        OpCode = OperationCode.Special;
        FuncCode = funcCode;
        ArgumentPattern = argumentPattern;

        if (versions.Length is 0)
            versions = AllVersions;

        MIPSVersions = new(versions);
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="InstructionMetadata"/> struct.
    /// </summary>
    public InstructionMetadata(string name, Func2Code funcCode, Argument[] argumentPattern, params MipsVersion[] versions)
    {
        Name = name;
        OpCode = OperationCode.Special2;
        Function2Code = funcCode;
        ArgumentPattern = argumentPattern;

        if (versions.Length is 0)
            versions = AllVersions;
        
        MIPSVersions = new(versions);
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="InstructionMetadata"/> struct.
    /// </summary>
    public InstructionMetadata(string name, RegImmFuncCode branchCode, Argument[] argumentPattern, params MipsVersion[] versions)
    {
        Name = name;
        OpCode = OperationCode.RegisterImmediate;
        RegisterImmediateFuncCode = branchCode;
        ArgumentPattern = argumentPattern;

        if (versions.Length is 0)
            versions = AllVersions;
        
        MIPSVersions = new(versions);
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="InstructionMetadata"/> struct.
    /// </summary>
    /// <remarks>
    /// This is constructor is only for pseudo-instructions.
    /// </remarks>
    public InstructionMetadata(string name, PseudoOp pseudoOp, Argument[] argumentPattern, int realizedCount, params MipsVersion[] versions)
    {
        Name = name;
        PseudoOp = pseudoOp;
        ArgumentPattern = argumentPattern;
        RealizedInstructionCount = realizedCount;

        if (versions.Length is 0)
            versions = AllVersions;
        
        MIPSVersions = new(versions);
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="InstructionMetadata"/> struct.
    /// </summary>
    [JsonConstructor]
    internal InstructionMetadata(string name, OperationCode? opCode, FunctionCode? funcCode, Func2Code? function2Code, RegImmFuncCode? registerImmediateFuncCode, PseudoOp? pseudoOp, Argument[] argumentPattern, int? realizedInstructionCount, HashSet<MipsVersion> mipsVersions)
    {
        Name = name;
        OpCode = opCode;
        FuncCode = funcCode;
        Function2Code = function2Code;
        RegisterImmediateFuncCode = registerImmediateFuncCode;
        PseudoOp = pseudoOp;
        ArgumentPattern = argumentPattern;
        RealizedInstructionCount = realizedInstructionCount;
        MIPSVersions = mipsVersions;
    }

    /// <summary>
    /// Gets the name of the instruction.
    /// </summary>
    [JsonPropertyName("name")]
    [JsonIgnore(Condition = JsonIgnoreCondition.Never)]
    public string Name { get; }

    /// <summary>
    /// Gets the instruction operation code.
    /// </summary>
    [JsonPropertyName("op_code")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public OperationCode? OpCode { get; }

    /// <summary>
    /// Gets the instruction function code.
    /// </summary>
    [JsonPropertyName("func_code")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public FunctionCode? FuncCode { get; }

    /// <summary>
    /// Gets the instruction function code.
    /// </summary>
    [JsonPropertyName("func2_code")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public Func2Code? Function2Code { get; }

    /// <summary>
    /// Gets the instruction register immediate code.
    /// </summary>
    [JsonPropertyName("rt_func_code")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public RegImmFuncCode? RegisterImmediateFuncCode { get; }

    /// <summary>
    /// Gets the pseudo op for a pseudo-instruction.
    /// </summary>
    [JsonPropertyName("pseudo_op")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public PseudoOp? PseudoOp { get; }

    /// <summary>
    /// Gets the function type.
    /// </summary>
    [JsonIgnore]
    public readonly InstructionType Type => InstructionTypeHelper.GetInstructionType(OpCode, RegisterImmediateFuncCode);

    /// <summary>
    /// Gets the instruction parse type
    /// </summary>
    [JsonPropertyName("args")]
    [JsonIgnore(Condition = JsonIgnoreCondition.Never)]
    public Argument[] ArgumentPattern { get; }

    /// <summary>
    /// Gets the number of real instructions required to execute the instruction.
    /// </summary>
    /// <remarks>
    /// This exists for pseudo instructions.
    /// </remarks>
    [JsonPropertyName("real_instruction_count")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public int? RealizedInstructionCount { get; }

    /// <summary>
    /// Gets whether or not the instruction is a pseudo instruction.
    /// </summary>
    [JsonIgnore]
    public bool IsPseudoInstruction => !OpCode.HasValue;

    /// <summary>
    /// Gets the version of MIPS required for the instruction.
    /// </summary>
    [JsonPropertyName("versions")]
    [JsonIgnore(Condition = JsonIgnoreCondition.Never)]
    public HashSet<MipsVersion> MIPSVersions { get; }
}
