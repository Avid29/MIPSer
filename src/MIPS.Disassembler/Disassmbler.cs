// Adam Dernis 2025

using MIPS.Assembler.Helpers.Tables;
using MIPS.Assembler.Models;
using MIPS.Disassembler.Models.Instructions;
using MIPS.Models.Instructions;
using MIPS.Models.Instructions.Enums;
using MIPS.Models.Instructions.Enums.Registers;
using System.Text;

namespace MIPS.Disassembler;

/// <summary>
/// A MIPS disassmbler.
/// </summary>
public class Disassmbler
{
    /// <summary>
    /// Initializes a new instance of the <see cref="Disassmbler"/> class.
    /// </summary>
    public Disassmbler(AssemblerConfig config)
    {
        Config = config;
        InstructionTable = new InstructionTable(config.MipsVersion);
    }

    /// <summary>
    /// Gets the assembler configuration to use for disassembly.
    /// </summary>
    public AssemblerConfig Config { get; }

    /// <summary>
    /// Gets the instruction table for this disassembler instance.
    /// </summary>
    public InstructionTable InstructionTable { get; }

    /// <summary>
    /// Disassmbles the <paramref name="instruction"/> into a string.
    /// </summary>
    /// <param name="instruction">The instruction to disassmble.</param>
    /// <returns>The instruction as a string.</returns>
    public string DisassmbleInstruction(Instruction instruction)
    {
        byte funcCode = instruction.Type switch
        {
            // Technically could be done with 'or', but clarity is nice.
            InstructionType.BasicR => (byte)instruction.FuncCode,
            InstructionType.Special2R => (byte)instruction.Func2Code,
            InstructionType.Special3R => (byte)instruction.Func3Code,
            
            InstructionType.BasicI or
            InstructionType.BasicJ => 0,
            
            InstructionType.RegisterImmediate or
            InstructionType.RegisterImmediateBranch => (byte)instruction.RTFuncCode,

            InstructionType.Coproc1 => (byte)((FloatInstruction)instruction).CoProc1RSCode,
            InstructionType.Float => (byte)((FloatInstruction)instruction).FloatFuncCode,

            _ => 255,
        };

        byte fmt = 0;
        if (instruction.Type is InstructionType.Float)
        {
            fmt = (byte)((FloatInstruction)instruction).Format;
        }

        var key = ((byte)instruction.OpCode, funcCode, fmt);
        if (!InstructionTable.TryGetInstruction(key, out var meta, out _))
        {
            return "Unknown instruction";
        }
        
        StringBuilder pattern = new($"{meta.Name} ");
        for (int i = 0; i < meta.ArgumentPattern.Length; i++)
        {
            pattern.Append(meta.ArgumentPattern[i] switch
            {
                Argument.RS => RegistersTable.GetRegisterString(instruction.RS),
                Argument.RT => RegistersTable.GetRegisterString(instruction.RT),
                Argument.RD => RegistersTable.GetRegisterString(instruction.RD),
                Argument.Shift => instruction.ShiftAmount,
                Argument.Immediate => instruction.ImmediateValue,
                Argument.Offset => instruction.Offset,
                Argument.Address => instruction.Address,
                Argument.AddressOffset => $"{instruction.Offset}({RegistersTable.GetRegisterString(instruction.RS)})",
                Argument.FullImmediate => 0, // Won't happen until pseudo-instruction disassembly
                Argument.FS => RegistersTable.GetRegisterString((Register)((FloatInstruction)instruction).FS, RegisterSet.FloatingPoints),
                Argument.FT => RegistersTable.GetRegisterString((Register)((FloatInstruction)instruction).FT, RegisterSet.FloatingPoints),
                Argument.FD => RegistersTable.GetRegisterString((Register)((FloatInstruction)instruction).FD, RegisterSet.FloatingPoints),
                _ => "unknown",
            });

            if (i < meta.ArgumentPattern.Length - 1)
            {
                pattern.Append(", ");
            }
        }

        return $"{pattern}";
    }
}
