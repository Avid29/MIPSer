// Adam Dernis 2025

using MIPS.Assembler.Config;
using MIPS.Assembler.Helpers.Tables;
using MIPS.Disassembler.Models.Instructions;
using MIPS.Models.Instructions;
using MIPS.Models.Instructions.Enums;
using MIPS.Models.Instructions.Enums.Registers;
using System.Linq;
using System.Text;

namespace MIPS.Disassembler;

/// <summary>
/// A MIPS disassembler.
/// </summary>
public class Disassembler
{
    /// <summary>
    /// Initializes a new instance of the <see cref="Disassembler"/> class.
    /// </summary>
    public Disassembler(AssemblerConfig config)
    {
        Config = config;
        InstructionTable = new InstructionTable(config);
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
    /// Disassembles the <paramref name="instruction"/> into a string.
    /// </summary>
    /// <param name="instruction">The instruction to disassemble.</param>
    /// <returns>The instruction as a string.</returns>
    public string Disassemble(Instruction instruction)
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

            // TODO: Disassembling CoProc0 instructions
            //InstructionType.Coproc0 => (byte)((CoProc0Instruction)instruction).CoProc0RSCode,

            InstructionType.Coproc1 => (byte)((FloatInstruction)instruction).CoProc1RSCode,
            InstructionType.Float => (byte)((FloatInstruction)instruction).FloatFuncCode,

            _ => 255,
        };

        // If the instruction is a float instruction, retrieve the format.
        FloatFormat? format = null;
        if (instruction.Type is InstructionType.Float)
            format = ((FloatInstruction)instruction).Format;

        var key = ((byte)instruction.OpCode, funcCode, instruction.Type is InstructionType.Float);
        if (!InstructionTable.TryGetInstruction(key, out var metas, out _, out _))
        {
            return "Unknown instruction";
        }

        // Take the metadata with the most arguments
        var meta = metas.OrderByDescending(x => x.ArgumentPattern.Length).First();

        // Apply the format to the name if it exists
        var name = meta.Name;
        if (format is not null)
        {
            name = FloatFormatTable.ApplyFormat(name, format.Value);
        }

        StringBuilder pattern = new($"{name} ");
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
                Argument.AddressBase => $"{instruction.ImmediateValue}({RegistersTable.GetRegisterString(instruction.RS)})",
                Argument.FullImmediate => 0, // Won't happen until pseudo-instruction disassembly
                Argument.FS => RegistersTable.GetRegisterString((GPRegister)((FloatInstruction)instruction).FS, RegisterSet.FloatingPoints),
                Argument.FT => RegistersTable.GetRegisterString((GPRegister)((FloatInstruction)instruction).FT, RegisterSet.FloatingPoints),
                Argument.FD => RegistersTable.GetRegisterString((GPRegister)((FloatInstruction)instruction).FD, RegisterSet.FloatingPoints),
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
