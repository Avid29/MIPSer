// Adam Dernis 2023

using CommunityToolkit.Diagnostics;
using MIPS.Models.Addressing.Enums;
using System;
using System.IO;

namespace MIPS.Assembler.Models.Construction;

public partial class ModuleConstruction
{
    /// <summary>
    /// Gets the current position in the text stream.
    /// </summary>
    public long TextPosition => _text.Position;

    /// <summary>
    /// Gets the current position in the data stream.
    /// </summary>
    public long DataPosition => _data.Position;

    /// <summary>
    /// Appends an array of bytes to the end of the specified segment.
    /// </summary>
    /// <param name="segment">The segment to append to</param>
    /// <param name="bytes">The bytes to append to the end of the buffer.</param>
    /// <exception cref="ArgumentException"/>
    public void Append(Segment segment, params byte[] bytes)
    {
        // Select buffer and write bytes
        Stream buffer = GetSegmentStream(segment);
        buffer.Write(bytes);
    }

    /// <summary>
    /// Aligns a segment to an n-size boundary.
    /// </summary>
    /// <param name="segment">The segment to align.</param>
    /// <param name="boundary">The alignment boundary.</param>
    public void Align(Segment segment, int boundary)
    {
        // Scale boundary by power of 2.
        boundary = 1 << boundary;

        // Select buffer and get alignment offset
        Stream stream = GetSegmentStream(segment);
        int offset = (int)stream.Length % boundary;

        // Already aligned
        if (offset <= 0)
            return;

        // Append offset bytes
        var append =  new byte[offset];
        Append(segment, append);
    }

    /// <summary>
    /// Seeks to the start of all segments.
    /// </summary>
    public void ResetStreamPositions()
    {
        _text.Position = 0;
        _data.Position = 0;
    }

    private Stream GetSegmentStream(Segment segment)
    {
        return segment switch
        {
            Segment.Text => _text,
            Segment.Data => _data,
            _ => ThrowHelper.ThrowArgumentException<Stream>(nameof(segment), $"{nameof(segment)} must be either {Segment.Text} or {Segment.Data}.")
        };
    }
}
