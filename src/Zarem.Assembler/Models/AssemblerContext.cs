// Adam Dernis 2024

using System.Diagnostics.CodeAnalysis;
using Zarem.Assembler.Config;
using Zarem.Models.Addressing;
using Zarem.Models.Modules;
using Zarem.Models.Modules.Tables;

namespace Zarem.Assembler.Models;

/// <summary>
/// This class provides an interface for assembler context to the parsers.
/// </summary>
/// <remarks>
/// The assembler is structured so the parsers create as densely represented objects as possible,
/// however these objects are not appended to the module until being passed back to the assembler.
/// This class provides readonly access to a handful of contextual information the parsers depend on.
/// </remarks>
public abstract class AssemblerContext
{
    /// <summary>
    /// Initializes a new instance of the <see cref="AssemblerContext"/>
    /// </summary>
    /// <param name="module"></param>
    protected AssemblerContext(Module module)
    {
        Module = module;
    }

    /// <summary>
    /// Gets the module being assembled.
    /// </summary>
    protected Module Module { get; }

    /// <inheritdoc cref="Assembler.CurrentAddress"/>
    public abstract Address CurrentAddress { get; }

    /// <inheritdoc cref="Module.TryGetSymbol(string, out SymbolEntry)"/>
    public bool TryGetSymbol(string name, [NotNullWhen(true)] out SymbolEntry? value) => Module.TryGetSymbol(name, out value);
}
