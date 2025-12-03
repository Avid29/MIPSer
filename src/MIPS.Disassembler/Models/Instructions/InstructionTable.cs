// Adam Dernis 2025

using MIPS.Assembler.Models.Config;
using MIPS.Assembler.Models.Instructions;
using MIPS.Assembler.Models.Instructions.Abstract;
using MIPS.Models.Instructions.Enums;

namespace MIPS.Disassembler.Models.Instructions;

/// <summary>
/// A class for managing instruction lookup by opcode and function code.
/// </summary>
public class InstructionTable : InstructionTableBase<(byte op, byte func, bool)>
{   
    /// <summary>
    /// Initializes a new instance of the <see cref="InstructionTable"/> class.
    /// </summary>
    public InstructionTable(AssemblerConfig config) : base(config)
    {
    }

    /// <inheritdoc/>
    protected override void LoadInstruction(InstructionMetadata metadata)
    {
        if (metadata.MIPSVersions.Contains(Config.MipsVersion))
        {
            byte? funcCode = metadata.Type switch
            {
                InstructionType.BasicR => (byte?)metadata.FuncCode,
                
                InstructionType.BasicI or
                InstructionType.BasicJ => 0,
                
                InstructionType.Special2R => (byte?)metadata.Function2Code,
                InstructionType.Special3R => (byte?)metadata.Function3Code,
                
                InstructionType.RegisterImmediate or
                InstructionType.RegisterImmediateBranch => (byte?)metadata.RegisterImmediateFuncCode,
                
                // TODO: Disassembling CoProc0 instructions
                //InstructionType.Coproc0 => (byte?)metadata.CoProc0RS,

                InstructionType.Coproc1 => (byte?)metadata.CoProc1RS,
                InstructionType.Float => (byte?)metadata.FloatFuncCode,
                _ => 0,
            };

            if (metadata.OpCode is null || funcCode is null)
                return;
            
            bool hasFormat = metadata.FloatFormats is not null;
            (byte, byte, bool) key = ((byte)metadata.OpCode, (byte)funcCode, hasFormat);
            LoadInstruction(key, metadata);
        }
    }
}
