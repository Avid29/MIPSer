// Adam Dernis 2025

using MIPS.Assembler.Models;
using MIPS.Models.Instructions;
using MIPS.Services.Interfaces;

namespace MIPS.Disassembler.Services;

#if DEBUG

/// <summary>
/// An implementation of the <see cref="IDisassemblerService"/>.
/// </summary>
public class DisassemblerService : IDisassemblerService
{
    /// <inheritdoc/>
    public string Disassemble(Instruction instruction)
    {
        var disassmbler = new Disassmbler(new AssemblerConfig());
        return disassmbler.DisassmbleInstruction(instruction);
    }
}

#endif
