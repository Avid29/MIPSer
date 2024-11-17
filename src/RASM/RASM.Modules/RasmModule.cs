// Adam Dernis 2024

using CommunityToolkit.Diagnostics;
using MIPS.Assembler.Models;
using MIPS.Assembler.Models.Modules;
using MIPS.Assembler.Models.Modules.Interfaces;
using RASM.Modules.Config;
using System.Text;

using RasmRelocation = RASM.Modules.Tables.RelocationEntry;
using RasmReference = RASM.Modules.Tables.ReferenceEntry;
using RasmSymbol = RASM.Modules.Tables.SymbolEntry;

namespace RASM.Modules;

/// <summary>
/// A fully assembled object module.
/// </summary>
public class RasmModule : IModule<RasmModule>
{
    private Stream _source;

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
    public static RasmModule? Create(Stream stream, ModuleConstructor constructor, AssemblerConfig config)
    {
        if (config is not RasmConfig rconfig)
        {
            ThrowHelper.ThrowArgumentException(nameof(config), $"{config} must be a {nameof(RasmConfig)}.");
            return null;
        }

        // TODO: Flags and entry point properly
        // TODO: Construct string list.

        // Allocate space for header
        var header = new Header(rconfig.MagicNumber, rconfig.VersionNumber, 0, 0, new uint[10]); // We'll update the strings length later.
        header.TryWriteHeader(stream);

        // Append segments to stream
        constructor.ResetStreamPositions();
        foreach(var section in constructor.Sections)
            section.CopyTo(stream);

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
        var relCount = 0;
        var refCount = 0;
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
            newSymbol.SymbolIndex = (uint)GetStringPosition(symbol.Symbol);
            newSymbol.Write(stream);
        }

        // Write strings to the stream
        strings.Position = 0;
        strings.CopyTo(stream);

        // Mark the end of the module
        stream.SetLength(stream.Position);
        
        // Rewrite the header
        stream.Position = 0;
        header.UpdateSizes(
            (uint)constructor.Text.Length,
            (uint)constructor.ReadOnlyData.Length,
            (uint)constructor.Data.Length,
            (uint)constructor.SmallInitializedData.Length,
            (uint)constructor.SmallUninitializedData.Length,
            (uint)constructor.UninitializedData.Length,
            (uint)relCount, (uint)refCount,
            (uint)constructor.Symbols.Count,
            (uint)strings.Length);
        header.TryWriteHeader(stream);

        // Reset position, flush, and load
        stream.Position = 0;
        stream.Flush();
        return Load(stream);
    }

    /// <inheritdoc/>
    public static RasmModule? Load(Stream stream)
    {
        if(!Header.TryReadHeader(stream, out var header))
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
        if(ValidateHeader(rconfig))
            return null;

        return null;
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
}
