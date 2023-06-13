// Adam Dernis 2023

using MIPS.Models;
using MIPS.Models.Addressing;
using System;
using System.Collections.Generic;
using System.IO;

namespace MIPS.Assembler.Models.Construction;

/// <summary>
/// An object module in construction.
/// </summary>
public partial class ModuleConstruction
{
    private const ushort MAGIC = 0xFA_CE;
    private const ushort VERSION = 0x2C_CE;

    private readonly Dictionary<string, SegmentAddress> _symbols;

    private readonly BinaryWriter _text;
    private readonly BinaryWriter _data;
    //private readonly BinaryWriter _rodata;

    private uint _flags = 0;
    private uint _entryPoint = 0;

    // TODO: Handle all sections property
    private uint _rdataSize = 0;
    private uint _sdataSize = 0;
    private uint _sbssSize = 0;
    private uint _bssSize = 0;
    
    /// <summary>
    /// Initializes a new instance of the <see cref="ModuleConstruction"/> class.
    /// </summary>
    public ModuleConstruction() : this(new MemoryStream(), new MemoryStream())
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ModuleConstruction"/> class.
    /// </summary>
    public ModuleConstruction(Stream text, Stream data)
    {
        // NOTE: Is this safe? Can I just write to the back of a stream that may not be empty?
        // I think it's not only safe, but good design for linking. Investigate more though.

        _text = new BinaryWriter(text);
        _data = new BinaryWriter(data);

        _symbols = new Dictionary<string, SegmentAddress>();
    }

    /// <summary>
    /// Gets the fully assembled object module.
    /// </summary>
    public Module Finish()
    {
        // TODO: Flags and entry point properly

        var header = new Header(MAGIC, VERSION, _flags, _entryPoint, (uint)TextPosition, _rdataSize, (uint)DataPosition, _sdataSize, _sbssSize, _bssSize, 0, 0, 0, 0);
        throw new NotImplementedException();
    }
}
