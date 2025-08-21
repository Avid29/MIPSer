// Avishai Dernis 2025

using System.IO;

namespace MIPS.Interpreter.Models.System.Memory;

/// <summary>
/// A class representing a page of memory.
/// </summary>
public class Page
{
    private MemoryStream _data;

    /// <summary>
    /// Initializes a new instance of the <see cref="Page"/> class.
    /// </summary>
    public Page()
    {
        _data = new MemoryStream(new byte[4096]);
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
            if(AsStream(address).TryRead(out uint value))
                return value;

            return 0;
        }
        set => AsStream(address).TryWrite(value);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public Stream AsStream(uint address = 0)
    {
        var stream = _data;
        stream.Seek(address & 0xFFF, SeekOrigin.Begin);
        return stream;
    }
}
