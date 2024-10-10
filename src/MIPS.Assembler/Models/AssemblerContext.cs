// Adam Dernis 2024

using MIPS.Assembler.Models.Modules;
using MIPS.Models.Addressing;

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
    private readonly ModuleConstruction _module;

    /// <summary>
    /// Initializes a new instance of the <see cref="AssemblerContext"/> class.
    /// </summary>
    public AssemblerContext(Assembler assembler, ModuleConstruction module)
    {
        _assembler = assembler;
        _module = module;
    }

    /// <inheritdoc cref="Assembler.CurrentAddress"/>
    public Address CurrentAddress => _assembler.CurrentAddress;

    /// <inheritdoc cref="ModuleConstruction.TryGetSymbol(string, out Address)"/>
    public bool TryGetSymbol(string name, out Address value) => _module.TryGetSymbol(name, out value);
}
