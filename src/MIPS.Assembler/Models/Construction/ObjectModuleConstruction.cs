// Adam Dernis 2023

using CommunityToolkit.Diagnostics;
using MIPS.Models;
using MIPS.Models.Addressing.Enums;
using System;

namespace MIPS.Assembler.Models.Construction;

/// <summary>
/// An object module in construction.
/// </summary>
public class ObjectModuleConstruction
{
    private ByteBuffer _text;
    private ByteBuffer _data;

    /// <summary>
    /// Initializes a new instance of the <see cref="ObjectModuleConstruction"/> class.
    /// </summary>
    public ObjectModuleConstruction()
    {
        // TODO: Smart allocate buffers ahead of time
        _text = new ByteBuffer();
        _data = new ByteBuffer();
    }

    /// <summary>
    /// Appends an array of bytes to the end of the specified segment.
    /// </summary>
    /// <param name="segment">The segment to append to</param>
    /// <param name="bytes">The bytes to append to the end of the buffer.</param>
    /// <exception cref="ArgumentException"/>
    public void Append(Segment segment, params byte[] bytes)
    {
        ByteBuffer buffer = GetBuffer(segment);
        buffer.Append(bytes);
    }

    /// <summary>
    /// Aligns a segment to an n-size boundary.
    /// </summary>
    /// <param name="segment"></param>
    /// <param name="boundary"></param>
    public void Align(Segment segment, int boundary)
    {
        ByteBuffer buffer = GetBuffer(segment);
        buffer.Align(boundary);
    }

    private ByteBuffer GetBuffer(Segment segment)
    {
        return segment switch
        {
            Segment.Text => _text,
            Segment.Data => _data,
            _ => ThrowHelper.ThrowArgumentException<ByteBuffer>(nameof(segment), $"{nameof(segment)} must be either {Segment.Text} or {Segment.Data}.")
        };
    }

    /// <summary>
    /// Gets the fully assembled object module.
    /// </summary>
    public ObjectModule Finish()
    {
        throw new NotImplementedException();
    }
}
