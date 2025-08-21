// Avishai Dernis 2025

using System.Collections.Generic;
using System.IO;

namespace MIPS.Interpreter.Models.System.Memory;

/// <summary>
/// Represents the RAM (Random Access Memory) in a MIPS interpreter.
/// </summary>
public class RAM
{
    private Dictionary<uint, Page> _pages;

    /// <summary>
    /// Initializes a new instance of the <see cref="RAM"/> class.
    /// </summary>
    public RAM()
    {
        _pages = [];
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
            GetPageAndOffset(address, out var page, out var offset);
            return page[offset];
        }
        set
        {
            GetPageAndOffset(address, out var page, out var offset);
            page[offset] = value;
        }
    }
    
    /// <summary>
    /// 
    /// </summary>
    /// <param name="address"></param>
    /// <returns></returns>
    public Stream AsStream(uint address)
    {
        GetPageAndOffset(address, out var page, out var offset);
        return page.AsStream(offset);
    }

    private void GetPageAndOffset(uint address, out Page page, out uint offset)
    {
        var pIndex = address & 0xFFFF_F000;
        offset = address & 0xFFF;
        if (!_pages.TryGetValue(pIndex, out var tpage))
        {
            page = new Page();
            _pages.Add(pIndex, page);
            return;
        }

        page = tpage;
    }
}
