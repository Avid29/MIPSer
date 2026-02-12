// Adam Dernis 2024

using Zarem.Assembler.Config;
using Zarem.Assembler.Models.Instructions;
using Zarem.Models.Modules;

namespace Zarem.Assembler.Models;

/// <summary>
/// A class provides that provide interface for assembler context to the parsers.
/// </summary>
public class MIPSAssemblerContext : AssemblerContext<MIPSAssembler, MIPSAssemblerConfig>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="MIPSAssemblerContext"/> class.
    /// </summary>
    public MIPSAssemblerContext(MIPSAssembler assembler, Module module) : base(assembler, module)
    {
        InstructionTable = new InstructionTable(assembler.Config);
    }

    /// <remarks>
    /// This is for testing purposes. Don't adjust behavior arround ensuring it works.
    /// </remarks>
    internal MIPSAssemblerContext(Module module) : base(module)
    {
        InstructionTable = null!;
    }

    /// <summary>
    /// Gets a table of instructions for the assemlber.
    /// </summary>
    public InstructionTable InstructionTable { get; }
}
