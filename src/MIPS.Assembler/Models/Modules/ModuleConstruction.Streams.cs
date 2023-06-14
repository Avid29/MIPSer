// Adam Dernis 2023

using CommunityToolkit.Diagnostics;
using MIPS.Models.Addressing.Enums;
using System;
using System.IO;

namespace MIPS.Assembler.Models.Modules;

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
    /// <param name="section">The segment to append to</param>
    /// <param name="bytes">The bytes to append to the end of the buffer.</param>
    /// <exception cref="ArgumentException"/>
    public void Append(Section section, params byte[] bytes)
    {
        // Select buffer and write bytes
        Stream buffer = GetSegmentStream(section);
        buffer.Write(bytes);
    }

    /// <summary>
    /// Aligns a segment to an n-size boundary.
    /// </summary>
    /// <param name="section">The segment to align.</param>
    /// <param name="boundary">The alignment boundary.</param>
    public void Align(Section section, int boundary)
    {
        // Scale boundary by power of 2.
        boundary = 1 << boundary;

        // Select buffer and get alignment offset
        Stream stream = GetSegmentStream(section);
        int offset = (int)stream.Length % boundary;

        // Already aligned
        if (offset <= 0)
            return;

        // Append offset bytes
        var append =  new byte[offset];
        Append(section, append);
    }

    /// <summary>
    /// Seeks to the start of all segments.
    /// </summary>
    public void ResetStreamPositions()
    {
        _text.Position = 0;
        _data.Position = 0;
    }

    private Stream GetSegmentStream(Section section)
    {
        return section switch
        {
            Section.Text => _text,
            Section.Data => _data,
            _ => ThrowHelper.ThrowArgumentException<Stream>(nameof(section), $"{nameof(section)} must be either {Section.Text} or {Section.Data}.")
        };
    }
}
