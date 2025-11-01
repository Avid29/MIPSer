// Adam Dernis 2024

using CommunityToolkit.Diagnostics;
using MIPS.Assembler.Models;
using MIPS.Assembler.Models.Modules;
using MIPS.Assembler.Models.Modules.Interfaces;
using RASM.Modules.Config;
using MIPS.Models.Modules.Tables;
using System.Text;

using RasmRelocation = RASM.Modules.Tables.RelocationEntry;
using RasmReference = RASM.Modules.Tables.ReferenceEntry;
using RasmSymbol = RASM.Modules.Tables.SymbolEntry;
using MIPS.Models.Addressing.Enums;

namespace RASM.Modules;

/// <summary>
/// A fully assembled object module in RASM format.
/// </summary>
public class RasmModule : IBuildModule<RasmModule>
{
    private const int SECTION_COUNT = 6;

    private readonly Stream _source;

    /// <summary>
    /// Initializes a new instance of the <see cref="RasmModule"/> class.
    /// </summary>
    public RasmModule(Header header, Stream source)
    {
        Header = header;
        _source = source;
    }

    /// <summary>
    /// Gets the module header info.
    /// </summary>
    public Header Header { get; private set; }

    /// <inheritdoc/>
    public static RasmModule? Create(ModuleConstructor constructor, AssemblerConfig config, Stream? stream = null)
    {
        stream ??= new MemoryStream();

        if (config is not RasmConfig rconfig)
        {
            ThrowHelper.ThrowArgumentException(nameof(config), $"{config} must be a {nameof(RasmConfig)}.");
            return null;
        }

        // TODO: Flags and entry point properly
        // TODO: Construct string list.

        // Allocate space for header
        var header = new Header(rconfig.MagicNumber, rconfig.VersionNumber, 0, 0,
            (uint)constructor.Text.Length,
            (uint)constructor.ReadOnlyData.Length,
            (uint)constructor.Data.Length,
            (uint)constructor.SmallInitializedData.Length,
            (uint)constructor.SmallUninitializedData.Length,
            (uint)constructor.UninitializedData.Length,
            0, 0, 0, 0); // We'll handle table sizes later
        header.TryWrite(stream);

        // Append segments to stream
        constructor.ResetStreamPositions();
        foreach(var section in constructor.Sections)
            section.Stream.CopyTo(stream);

        // Split string references to string table.
        var strings = new MemoryStream();
        var positions = new Dictionary<string, long>();

        long GetStringPosition(string? symbol)
        {
            // TODO: Better error handling
            if (symbol is null)
                return 0;

            // Get from table if already seen
            if (positions.TryGetValue(symbol, out long value))
                return value;
            
            // Append to table
            var pos = strings.Position;
            strings.Write(Encoding.UTF8.GetBytes(symbol));
            strings.WriteByte(0); // Null terminate

            // Log in table and return position
            positions.Add(symbol, pos);
            return pos;
        }
        
        // Create split streams for relocations and references
        uint relCount = 0;
        uint refCount = 0;
        Stream relStream = new MemoryStream();
        Stream refStream = new MemoryStream();

        // Write reference and relocation table to their streams
        foreach (var reference in constructor.References)
        {
            if (reference.IsRelocation)
            {
                // Convert to rasm relocation and write to relocation stream
                var newRelocation = RasmRelocation.Convert(reference);
                newRelocation.Write(relStream);
                relCount++;
            }
            else
            {
                // Convert to rasm relocation, extract string, and write to reference stream
                var newReference = RasmReference.Convert(reference);
                newReference.SymbolIndex = (uint)GetStringPosition(reference.Symbol);
                newReference.Write(refStream);
                refCount++;
            }
        }

        // Append rel and ref streams
        relStream.Position = 0;
        refStream.Position = 0;
        relStream.CopyTo(stream);
        refStream.CopyTo(stream);
        
        // Write symbol table to the stream
        foreach (var symbol in constructor.Symbols.Values)
        {
            // Convert to rasm, extract string, and write to stream
            var newSymbol = RasmSymbol.Convert(symbol);
            newSymbol.SymbolIndex = (uint)GetStringPosition(symbol.Name);
            newSymbol.Write(stream);
        }

        // Write strings to the stream
        strings.Position = 0;
        strings.CopyTo(stream);

        // Mark the end of the module
        stream.SetLength(stream.Position);
        
        // Rewrite the header
        stream.Position = 0;
        header.RelocationTableCount = relCount;
        header.ReferenceTableCount = refCount;
        header.DefinitionsTableCount = (uint)constructor.Symbols.Count;
        header.StringTableSize = (uint)strings.Length;
        header.TryWrite(stream);

        // Reset position, flush, and load
        stream.Position = 0;
        stream.Flush();
        return Load(stream);
    }

    /// <inheritdoc/>
    public static RasmModule? Load(Stream stream)
    {
        if(!Header.TryRead(stream, out var header))
            return null;

        return new RasmModule(header, stream);
    }
    
    /// <inheritdoc/>
    public ModuleConstructor? Abstract(AssemblerConfig config)
    {
        if (config is not RasmConfig rconfig)
        {
            ThrowHelper.ThrowArgumentException(nameof(config), $"{config} must be a {nameof(RasmConfig)}.");
            return null;
        }

        // Validate the header for this assembly
        if(!ValidateHeader(rconfig))
            return null;

        // Return to start of module
        ResetStream();

        // Copy sections
        uint[] sizes =
            [Header.TextSize, Header.ReadOnlyDataSize, Header.DataSize, Header.SmallDataSize,
            Header.SmallUninitializedDataSize, Header.UninitializedDataSize];
        var sections = new ModuleSection[SECTION_COUNT];
        for (int i = 0; i < sections.Length; i++)
        {
            var size = (int)sizes[i];
            sections[i] = new ModuleSection((Section)i);
            if (size is not 0)
            {
                sections[i].Stream.CopyFrom(_source, size);
            }
        }

        // Load table entries
        RasmRelocation[] relocations = new RasmRelocation[Header.RelocationTableCount];
        RasmReference[] references = new RasmReference[Header.ReferenceTableCount];
        RasmSymbol[] symbols = new RasmSymbol[Header.DefinitionsTableCount];
        for (int i = 0; i < relocations.Length; i++)
            relocations[i] = relocations[i].Read(_source);
        for (int i = 0; i < references.Length; i++)
            references[i] = references[i].Read(_source);
        for (int i = 0; i < symbols.Length; i++)
            symbols[i] = symbols[i].Read(_source);

        var strings = LoadStrings();

        var referenceList = new List<ReferenceEntry>();
        var symbolTable = new Dictionary<string, SymbolEntry>();
        foreach (var rel in relocations)
            referenceList.Add(rel.Convert());

        foreach (var @ref in references)
        {
            var reference = @ref.Convert();
            reference.Symbol = strings[(int)@ref.SymbolIndex];
            referenceList.Add(reference);
        }

        foreach (var sym in symbols)
        {
            var symbol = sym.Convert();
            symbol.Name = strings[(int)sym.SymbolIndex];
            symbolTable.Add(symbol.Name, symbol);
        }

        // Create constructor from the sections
        return new ModuleConstructor(sections, referenceList, symbolTable);
    }

    private unsafe void ResetStream(bool skipHeader = true)
    {
        _source.Position = skipHeader ? Header.HEADER_SIZE : 0;
    }

    private bool ValidateHeader(RasmConfig config)
    {
        // Ensure the magic number is correct
        if (Header.Magic != config.MagicNumber)
            return false;

        // Ensure the version number is correct
        if (Header.Version != config.VersionNumber)
            return false;

        // Ensure the module is the expected size
        if (Header.ExpectedModuleSize != _source.Length)
            return false;

        return true;
    }

    private Dictionary<int, string> LoadStrings()
    {
        var strings = new Dictionary<int, string>();
        var sb = new StringBuilder();
        int pos = 0;
        int c;

        while ((c = _source.ReadByte()) is not -1)
        {
            if (c is not 0)
            {
                sb.Append((char)c);
                continue;
            }
            
            var str = $"{sb}";
            strings.Add(pos, str);
            sb = new StringBuilder();
            pos += str.Length + 1;
        }
        return strings;
    }
}
