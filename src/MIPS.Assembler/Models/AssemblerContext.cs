// Adam Dernis 2024

using MIPS.Assembler.Models.Instructions;
using MIPS.Assembler.Models.Modules;
using MIPS.Models.Addressing;
using MIPS.Models.Modules.Tables;
using System.Diagnostics.CodeAnalysis;

namespace MIPS.Assembler.Models;

/// <summary>
/// This class provides an interface for assembler context to the parsers.
/// </summary>
/// <remarks>
/// The assembler is structured so the parsers create as densely represented objects as possible,
/// however these objects are no appended to the module until being passed back to the assembler.
/// This class provides readonly access to a handful of contextual information the parsers depend on.
/// </remarks>
public class AssemblerContext
{
    private readonly Assembler _assembler;
    private readonly ModuleConstructor _module;

    /// <summary>
    /// Initializes a new instance of the <see cref="AssemblerContext"/> class.
    /// </summary>
    public AssemblerContext(Assembler assembler, ModuleConstructor module)
    {
        _assembler = assembler;
        _module = module;
        InstructionTable = new InstructionTable(assembler.Config.MipsVersion);
    }

    /// <remarks>
    /// This is for testing purposes. Don't adjust behavior arround ensuring it works.
    /// </remarks>
    internal AssemblerContext(ModuleConstructor module)
    {
        _module = module;

        _assembler = null!;
        InstructionTable = null!;
    }
    
    /// <inheritdoc cref="Assembler.Config"/>
    public AssemblerConfig Config => _assembler.Config;

    /// <inheritdoc cref="Assembler.CurrentAddress"/>
    public Address CurrentAddress => _assembler.CurrentAddress;

    /// <summary>
    /// Gets a table of instructions for the assemlber.
    /// </summary>
    public InstructionTable InstructionTable { get; }

    /// <inheritdoc cref="ModuleConstructor.TryGetSymbol(string, out SymbolEntry)"/>
    public bool TryGetSymbol(string name, [NotNullWhen(true)] out SymbolEntry? value) => _module.TryGetSymbol(name, out value);
}
