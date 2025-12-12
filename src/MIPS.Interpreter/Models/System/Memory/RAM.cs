// Avishai Dernis 2025

using MIPS.Interpreter.Helpers;
using System.Collections.Generic;
using System.IO;

namespace MIPS.Interpreter.Models.System.Memory;

/// <summary>
/// Represents the RAM (Random Access Memory) in a MIPS interpreter.
/// </summary>
public class RAM
{
    private PagedMemoryStream _memoryStream;

    /// <summary>
    /// Initializes a new instance of the <see cref="RAM"/> class.
    /// </summary>
    public RAM()
    {
        _memoryStream = new PagedMemoryStream(4096);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="address"></param>
    /// <returns></returns>
    public uint this[uint address]
    {
        get
        {
            _memoryStream.Position = address;
            if (_memoryStream.TryRead(out uint value))
                return value;

            return 0;
        }
        set => _memoryStream.TryWrite(value);
    }

    /// <summary>
    /// Gets the RAM memory as a <see cref="Stream"/>.
    /// </summary>
    public Stream AsStream() => _memoryStream;
}
