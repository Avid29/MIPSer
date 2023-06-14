// Adam Dernis 2023

using MIPS.Models;
using MIPS.Models.Addressing;
using MIPS.Models.Modules;
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

    private readonly Dictionary<string, Address> _definitions;
    private readonly Dictionary<Address, string> _references;

    private readonly Stream _text;
    private readonly Stream _data;
    //private readonly Stream _rodata;

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

        _text = text;
        _data = data;

        _definitions = new Dictionary<string, Address>();
        _references = new Dictionary<Address, string>();
    }

    /// <summary>
    /// Gets the fully assembled object module.
    /// </summary>
    public Module Finish(Stream stream)
    {
        // TODO: Flags and entry point properly

        var header = new Header(MAGIC, VERSION, _flags, _entryPoint, (uint)_text.Length, _rdataSize, (uint)_data.Length, _sdataSize, _sbssSize, _bssSize, 0, 0, 0, 0);
        header.WriteHeader(stream);

        // Append segments to stream
        _text.CopyTo(stream);
        _data.CopyTo(stream);

        stream.Flush();

        return new Module(stream);
    }
}
