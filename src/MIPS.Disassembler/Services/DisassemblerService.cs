// Adam Dernis 2025

using MIPS.Assembler.Config;
using MIPS.Models.Instructions;
using MIPS.Services.Interfaces;

namespace MIPS.Disassembler.Services;

#if DEBUG

/// <summary>
/// An implementation of the <see cref="IDisassemblerService"/>.
/// </summary>
public class DisassemblerService : IDisassemblerService
{
    private Disassembler _disassembler;

    /// <summary>
    /// Initializes a new instance of the <see cref="DisassemblerService"/> class.
    /// </summary>
    public DisassemblerService(AssemblerConfig config)
    {
        _disassembler = new Disassembler(config);
    }

    /// <inheritdoc/>
    public string Disassemble(Instruction instruction)
        => _disassembler.Disassemble(instruction);
}

#endif
