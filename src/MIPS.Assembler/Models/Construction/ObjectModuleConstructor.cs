// Adam Dernis 2023

using MIPS.Models.Addressing;
using System.Collections.Generic;
using System.IO;

namespace MIPS.Assembler.Models.Construction;

/// <summary>
/// An object module in construction.
/// </summary>
public partial class ObjectModuleConstructor
{
    private readonly Dictionary<string, SegmentAddress> _symbols;

    private readonly BinaryWriter _text;
    private readonly BinaryWriter _data;
    
    /// <summary>
    /// Initializes a new instance of the <see cref="ObjectModuleConstructor"/> class.
    /// </summary>
    public ObjectModuleConstructor() : this(new MemoryStream(), new MemoryStream())
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ObjectModuleConstructor"/> class.
    /// </summary>
    public ObjectModuleConstructor(Stream text, Stream data)
    {
        // NOTE: Is this safe? Can I just write to the back of a stream that may not be empty?
        // I think it's not only safe, but good design for linking. Investigate more though.

        _text = new BinaryWriter(text);
        _data = new BinaryWriter(data);

        _symbols = new Dictionary<string, SegmentAddress>();
    }
}
