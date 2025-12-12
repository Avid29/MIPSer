// Avishai Dernis 2025

using CommunityToolkit.Diagnostics;
using System;
using System.Collections.Generic;
using System.IO;

namespace MIPS.Interpreter.Helpers;

/// <summary>
/// A <see cref="MemoryStream"/> with a virtual paging TLB above it to allow  sparse .
/// </summary>
public class PagedMemoryStream : Stream
{
    private readonly int _pageSize;
    private Dictionary<long, byte[]> _pages;
    private long _length;
    private long _position;

    /// <summary>
    /// Initializes a new instance of the <see cref="PagedMemoryStream"/> class.
    /// </summary>
    /// <param name="pageSize"></param>
    public PagedMemoryStream(int pageSize = 4096)
    {
        Guard.IsGreaterThan(pageSize, 0);

        _pageSize = pageSize;
        _pages = [];
    }

    /// <inheritdoc/>
    public override bool CanRead => true;

    /// <inheritdoc/>
    public override bool CanSeek => true;

    /// <inheritdoc/>
    public override bool CanWrite => true;

    /// <inheritdoc/>
    public override long Length => _length;

    /// <inheritdoc/>
    public override long Position
    {
        get => _position;
        set
        {
            if (value < 0 || value > _length)
                throw new ArgumentOutOfRangeException(nameof(value));
            _position = value;
        }
    }

    /// <inheritdoc/>
    public override void Flush()
    {
        // No operation needed
    }

    /// <inheritdoc/>
    public override int Read(byte[] buffer, int offset, int count)
    {
        Guard.IsNotNull(buffer);
        if (offset < 0 || count < 0 || offset + count > buffer.Length)
            ThrowHelper.ThrowArgumentOutOfRangeException();

        if (_position >= _length)
            return 0;

        int bytesRead = 0;
        while (count > 0 && _position < _length)
        {
            long pageIndex = (_position / _pageSize);
            int pageOffset = (int)(_position % _pageSize);
            int bytesAvailable = (int)Math.Min(_pageSize - pageOffset, _length - _position);
            int bytesToCopy = Math.Min(bytesAvailable, count);

            Array.Copy(_pages[pageIndex], pageOffset, buffer, offset, bytesToCopy);

            offset += bytesToCopy;
            count -= bytesToCopy;
            _position += bytesToCopy;
            bytesRead += bytesToCopy;
        }

        return bytesRead;
    }

    /// <inheritdoc/>

    public override long Seek(long offset, SeekOrigin origin)
    {
        long newPos = origin switch
        {
            SeekOrigin.Begin => offset,
            SeekOrigin.Current => _position + offset,
            SeekOrigin.End => _length + offset,
            _ => throw new ArgumentException("Invalid SeekOrigin", nameof(origin))
        };

        if (newPos < 0) throw new IOException("Seek before beginning");
        _position = newPos;
        return _position;
    }

    /// <inheritdoc/>
    public override void SetLength(long value)
    {
        Guard.IsGreaterThanOrEqualTo(value, 0);
        _length = value;

        var newPages = new Dictionary<long, byte[]>();
        foreach (var page in _pages)
        {
            if (page.Key < value)
            {
                newPages.Add(page.Key, page.Value);
            }
        }

        _pages = newPages;
    }

    /// <inheritdoc/>
    public override void Write(byte[] buffer, int offset, int count)
    {
        if (buffer == null) throw new ArgumentNullException(nameof(buffer));
        if (offset < 0 || count < 0 || offset + count > buffer.Length)
            throw new ArgumentOutOfRangeException();

        while (count > 0)
        {
            int pageIndex = (int)(_position / _pageSize);
            int pageOffset = (int)(_position % _pageSize);

            // Ensure page exists
            if (!_pages.ContainsKey(pageIndex))
                _pages.Add(pageIndex, new byte[_pageSize]);

            int bytesToCopy = Math.Min(_pageSize - pageOffset, count);
            Array.Copy(buffer, offset, _pages[pageIndex], pageOffset, bytesToCopy);

            offset += bytesToCopy;
            count -= bytesToCopy;
            _position += bytesToCopy;
            _length = Math.Max(_length, _position);
        }
    }
}
