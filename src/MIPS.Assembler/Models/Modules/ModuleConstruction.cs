﻿// Adam Dernis 2024

using CommunityToolkit.Diagnostics;
using MIPS.Models.Addressing;
using MIPS.Models.Addressing.Enums;
using MIPS.Models.Modules;
using MIPS.Models.Modules.Tables;
using System.Collections.Generic;
using System.IO;

namespace MIPS.Assembler.Models.Modules;

/// <summary>
/// An object module in construction.
/// </summary>
public partial class ModuleConstruction
{
    private const ushort MAGIC = 0xFA_CE;
    private const ushort VERSION = 0x2C_C6;
    private const int SECTION_COUNT = 6;

    private readonly List<RelocationEntry> _relocations;
    private readonly Dictionary<Address, string> _references;
    private readonly Dictionary<string, SymbolEntry> _definitions;

    private readonly Stream[] _sections;

    private Stream Text => _sections[(int)Section.Text];
    private Stream ReadOnlyData => _sections[(int)Section.ReadOnlyData];
    private Stream Data => _sections[(int)Section.Data];
    private Stream SmallInitializedData => _sections[(int)Section.SmallInitializedData];
    private Stream SmallUninitializedData => _sections[(int)Section.SmallUninitializedData];
    private Stream UninitializedData => _sections[(int)Section.UninitializedData];
    private Stream Strings { get; } = new MemoryStream();

    private uint _flags = 0;
    private uint _entryPoint = 0;

    /// <summary>
    /// Initializes a new instance of the <see cref="ModuleConstruction"/> class.
    /// </summary>
    public ModuleConstruction(params Stream[] streams)
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
    /// Gets the fully assembled object module.
    /// </summary>
    public Module? Finish(Stream stream)
    {
        // TODO: Flags and entry point properly

        var header = new Header(
            MAGIC, VERSION,
            _flags, _entryPoint,
            (uint)Text.Length,
            (uint)ReadOnlyData.Length,
            (uint)Data.Length,
            (uint)SmallInitializedData.Length,
            (uint)SmallUninitializedData.Length,
            (uint)UninitializedData.Length,
            (uint)Strings.Length,
            (uint)_relocations.Count,
            (uint)_references.Count,
            (uint)_definitions.Count);

        // Try to write the header
        if(!header.TryWriteHeader(stream))
            return null;

        // Append segments to stream
        ResetStreamPositions();
        foreach(var section in _sections)
            section.CopyTo(stream);

        // Write tables to the stream
        foreach(var rel in _relocations)
        {
            rel.Write(stream);
        }

        // Write symbol name table
        Strings.Position = 0;
        Strings.CopyTo(stream);

        // Mark the end and flush
        stream.SetLength(stream.Position);
        stream.Flush();

        stream.Position = 0;
        return Module.Load(stream);
    }
}
