// Adam Dernis 2025

using Zarem.Assembler.MIPS.Config;
using Zarem.MIPS.Models.Instructions;
using Zarem.MIPS.Services.Interfaces;

namespace Zarem.Disassembler.MIPS.Services;

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
