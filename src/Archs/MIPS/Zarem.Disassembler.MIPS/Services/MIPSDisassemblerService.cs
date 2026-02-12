// Adam Dernis 2025

using Zarem.Assembler.Config;
using Zarem.Models.Instructions;
using Zarem.Services.Interfaces;

namespace Zarem.Disassembler.Services;

#if DEBUG

/// <summary>
/// An implementation of the <see cref="IDisassemblerService"/>.
/// </summary>
public class MIPSDisassemblerService : IDisassemblerService
{
    private MIPSDisassembler _disassembler;

    /// <summary>
    /// Initializes a new instance of the <see cref="MIPSDisassemblerService"/> class.
    /// </summary>
    public MIPSDisassemblerService(MIPSAssemblerConfig config)
    {
        _disassembler = new MIPSDisassembler(config);
    }

    /// <inheritdoc/>
    public string Disassemble(Instruction instruction)
        => _disassembler.Disassemble(instruction);
}

#endif
