// Adam Dernis 2024

using CommunityToolkit.Diagnostics;
using MIPS.Models.Addressing;
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

    private readonly List<RelocationEntry> _relocations;
    private readonly Dictionary<Address, string> _references;
    private readonly Dictionary<string, SymbolEntry> _definitions;

    private readonly Stream[] _sections;

    /// <summary>
    /// Initializes a new instance of the <see cref="ModuleConstructor"/> class.
    /// </summary>
    public ModuleConstructor(params Stream[] streams)
    {
        // NOTE: Is this safe? Can I just write to the back of a stream that may not be empty?
        // I think it's not only safe, but good design for linking. Investigate more though.
        
        // Allow no stream arguments
        if (streams.Length == 0)
        {
            streams = new Stream[SECTION_COUNT];
            for (int i = 0; i < streams.Length; i++)
                streams[i] = new MemoryStream();
        }
        
        if (streams.Length != SECTION_COUNT)
        {
            ThrowHelper.ThrowArgumentException(nameof(streams), $"{streams} must contain exactly {SECTION_COUNT} entries.");
        }

        _sections = streams;

        _relocations = [];
        _references = [];
        _definitions = [];
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
    /// Gets the relocations lists.
    /// </summary>
    public IReadOnlyList<RelocationEntry> Relocations => _relocations;

    /// <summary>
    /// Gets the references dictionary.
    /// </summary>
    public IReadOnlyDictionary<Address, string> References => _references;

    /// <summary>
    /// Gets the symbol dictionary.
    /// </summary>
    public IReadOnlyDictionary<string, SymbolEntry> Symbols => _definitions;

    /// <summary>
    /// Gets the module sections streams.
    /// </summary>
    public IEnumerable<Stream> Sections => _sections;
}
