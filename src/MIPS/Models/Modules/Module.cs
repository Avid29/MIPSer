// Adam Dernis 2024

using MIPS.Models.Modules.Tables;

namespace MIPS.Assembler.Models.Modules;

/// <summary>
/// A modifiable object module.
/// </summary>
public partial class Module
{
    private readonly List<ReferenceEntry> _references;
    private readonly Dictionary<string, SymbolEntry> _definitions;
    private readonly Dictionary<string, ModuleSection> _sections;

    /// <summary>
    /// Initializes a new instance of the <see cref="Module"/> class.
    /// </summary>
    public Module()
    {
        _sections = [];
        _references = [];
        _definitions = [];
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Module"/> class.
    /// </summary>
    public Module(Dictionary<string, ModuleSection> sections, List<ReferenceEntry> references, Dictionary<string, SymbolEntry> definitions)
    {
        _sections = sections;
        _references = references;
        _definitions = definitions;
    }

    /// <summary>
    /// Gets the module name.
    /// </summary>
    public string? Name { get; } 

    /// <summary>
    /// Gets the references dictionary.
    /// </summary>
    public IReadOnlyList<ReferenceEntry> References => _references;

    /// <summary>
    /// Gets the symbol dictionary.
    /// </summary>
    public IReadOnlyDictionary<string, SymbolEntry> Symbols => _definitions;

    /// <summary>
    /// Gets the module sections streams.
    /// </summary>
    public IReadOnlyDictionary<string, ModuleSection> Sections => _sections;
}
