// Adam Dernis 2023

using CommunityToolkit.Diagnostics;
using MIPS.Assembler.Models.Construction;
using MIPS.Models;
using MIPS.Models.Addressing;
using MIPS.Models.Addressing.Enums;
using MIPS.Models.Instructions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace MIPS.Assembler;

/// <summary>
/// A MIPS assembler.
/// </summary>
public partial class Assembler
{
    private ObjectModuleConstruction _obj;
    private readonly Dictionary<string, SegmentAddress> _symbols; // TODO: Move to object module construction

    private Segment _activeSegment;

    /// <summary>
    /// Initializes a new instance of the <see cref="Assembler"/> class.
    /// </summary>
    private Assembler()
    {
        _obj = new ObjectModuleConstruction();
        _symbols = new Dictionary<string, SegmentAddress>();

        _activeSegment = Segment.Text;
    }

    /// <summary>
    /// Gets the current segment address.
    /// </summary>
    public SegmentAddress CurrentAddress
    {
        get
        {
            return _activeSegment switch
            {
                Segment.Text => new SegmentAddress(_obj.TextPosition, Segment.Text),
                Segment.Data => new SegmentAddress(_obj.DataPosition, Segment.Data),
                _ => ThrowHelper.ThrowArgumentException<SegmentAddress>(nameof(_activeSegment)),
            };
        }
    }
    
    /// <summary>
    /// Assembles an object module from a stream of assembly
    /// </summary>
    public static async Task<ObjectModule?> AssembleAsync(Stream stream)
    {
        var assembler = new Assembler();

        using var reader = new StreamReader(stream);

        // Iterate until end of stream, tracking line number
        for (int lineNum = 0; !reader.EndOfStream; lineNum++)
        {
            var line = await reader.ReadLineAsync();
            Guard.IsNotNull(line, nameof(line));

            assembler.ParseLine(line);
        }

        return assembler.Finish();
    }

    /// <summary>
    /// Creates a symbol at the current address.
    /// </summary>
    /// <param name="label">The name of the symbol</param>
    private void CreateSymbol(string label)
    {
        if (_symbols.ContainsKey(label))
        {
            // TODO: Log error
        }

        var address = CurrentAddress;
        _symbols.Add(label, address);
    }

    private void Append(params byte[] bytes)
    {
        // Append data
        _obj.Append(_activeSegment, bytes);
    }

    private void Append(Instruction instruction)
        => Append(BitConverter.GetBytes(instruction));

    private void SetActiveSegment(Segment segment)
    {
        _activeSegment = segment;
    }

    private ObjectModule Finish()
    {
        return _obj.Finish();
    }
}
