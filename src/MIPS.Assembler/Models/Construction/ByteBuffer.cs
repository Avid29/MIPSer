// Adam Dernis 2023

using CommunityToolkit.Diagnostics;
using System;

namespace MIPS.Assembler.Models.Construction;

/// <summary>
/// A class containing a byte buffer that can be added to.
/// </summary>
public class ByteBuffer
{
    private byte[] _data;
    private int _pos;

    /// <summary>
    /// Initializes a new instance of the <see cref="ByteBuffer"/> class.
    /// </summary>
    /// <param name="size"></param>
    public ByteBuffer(int size = 20)
    {
        _data = new byte[size];
        _pos = 0;
    }

    /// <summary>
    /// Appends an array of bytes to the end of the array
    /// </summary>
    public unsafe void Append(params byte[] bytes)
    {
        fixed (byte* start = _data)
        {
            byte* p = start + _pos;

            // Expand if adding the values will overrun the buffer  
            if (bytes.Length + _pos > _data.Length)
                Expand(bytes.Length);

            // Add each byte to the back of the buffer
            foreach (var t in bytes)
            {
                *p = t;
                p++;
            }

            _pos += bytes.Length;
        }
    }

    /// <summary>
    /// Aligns the buffer to an n-byte boundary.
    /// </summary>
    /// <remarks>
    /// Boundary is Log2 of actual boundary
    /// </remarks>
    public void Align(int boundary)
    {
        Guard.IsGreaterThan(boundary, 0);

        // Scale boundary by power of 2.
        boundary = 1 << boundary;

        int offset = _pos % boundary;

        // Already aligned
        if (offset <= 0)
            return;

        var append =  new byte[offset];
        Append(append);
    }

    private void Expand(int expansion)
    {
        var newSize = _data.Length + expansion;
        Array.Resize(ref _data, newSize);
    }
}
