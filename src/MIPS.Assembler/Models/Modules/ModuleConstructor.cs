// Adam Dernis 2024

using CommunityToolkit.Diagnostics;
using MIPS.Models.Addressing.Enums;
using MIPS.Models.Modules.Tables;
using System.Collections.Generic;
using System.IO;

namespace MIPS.Assembler.Models.Modules;

/// <summary>
/// An object module in construction.
/// </summary>
public partial class ModuleConstructor
{
    private const int SECTION_COUNT = 6;

    private readonly List<ReferenceEntry> _references;
    private readonly Dictionary<string, SymbolEntry> _definitions;

    private readonly Stream[] _sections;

    /// <summary>
    /// Initializes a new instance of the <see cref="ModuleConstructor"/> class.
    /// </summary>
    public ModuleConstructor()
    {
        _sections = new Stream[SECTION_COUNT];
        for (int i = 0; i < _sections.Length; i++)
            _sections[i] = new MemoryStream();
        
        _references = [];
        _definitions = [];
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ModuleConstructor"/> class.
    /// </summary>
    public ModuleConstructor(Stream[] streams, List<ReferenceEntry> references, Dictionary<string, SymbolEntry> definitions)
    {
        // NOTE: Is this safe? Can I just write to the back of a stream that may not be empty?
        // I think it's not only safe, but good design for linking. Investigate more though.
        if (streams.Length != SECTION_COUNT)
        {
            ThrowHelper.ThrowArgumentException(nameof(streams), $"{streams} must contain exactly {SECTION_COUNT} entries.");
        }

        _sections = streams;
        _references = references;
        _definitions = definitions;
    }

    /// <summary>
    /// Gets the text (text) stream.
    /// </summary>
    public Stream Text => GetSectionStream(Section.Text);

    /// <summary>
    /// Gets the read only data (rdata) stream.
    /// </summary>
    public Stream ReadOnlyData => GetSectionStream(Section.ReadOnlyData);

    /// <summary>
    /// Gets the data (data) stream.
    /// </summary>
    public Stream Data => GetSectionStream(Section.Data);

    /// <summary>
    /// Gets the small initialized data (sdata) stream.
    /// </summary>
    public Stream SmallInitializedData => GetSectionStream(Section.SmallInitializedData);
    
    /// <summary>
    /// Gets the small uninitialized data (sbss) stream.
    /// </summary>
    public Stream SmallUninitializedData => GetSectionStream(Section.SmallUninitializedData);
    
    /// <summary>
    /// Gets the uninitialized data (bss) stream.
    /// </summary>
    public Stream UninitializedData => GetSectionStream(Section.UninitializedData);

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
    public IEnumerable<Stream> Sections => _sections;
}
