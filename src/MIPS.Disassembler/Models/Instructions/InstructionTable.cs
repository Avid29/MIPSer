// Adam Dernis 2025

using MIPS.Assembler.Models.Instructions;
using MIPS.Assembler.Models.Instructions.Abstract;
using MIPS.Models.Instructions.Enums;

namespace MIPS.Disassembler.Models.Instructions;

/// <summary>
/// A class for managing instruction lookup by opcode and function code.
/// </summary>
public class InstructionTable : InstructionTableBase<(byte, byte)>
{   
    /// <summary>
    /// Initializes a new instance of the <see cref="InstructionTable"/> class.
    /// </summary>
    public InstructionTable(MipsVersion version) : base(version)
    {
    }

    /// <inheritdoc/>
    protected override void LoadInsturction(InstructionMetadata metadata)
    {
        if (metadata.MIPSVersions.Contains(Version))
        {
            byte? funcCode = metadata.Type switch {
                InstructionType.BasicR => (byte?)metadata.FuncCode,
                
                InstructionType.BasicI or
                InstructionType.BasicJ => 0,
                
                InstructionType.Special2R => (byte?)metadata.Function2Code,
                InstructionType.Special3R => (byte?)metadata.Function3Code,
                
                InstructionType.RegisterImmediate or
                InstructionType.RegisterImmediateBranch => (byte?)metadata.RegisterImmediateFuncCode,
                _ => 0,
            };

            if (metadata.OpCode is null || funcCode is null)
                return;

            (byte, byte) key = ((byte)metadata.OpCode, (byte)funcCode);
            LookupTable.TryAdd(key, metadata);
        }
    }
}
