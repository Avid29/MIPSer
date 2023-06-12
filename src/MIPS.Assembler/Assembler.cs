// Adam Dernis 2023

using CommunityToolkit.Diagnostics;
using MIPS.Assembler.Models.Construction;
using MIPS.Assembler.Parsers;
using MIPS.Models;
using MIPS.Models.Addressing;
using MIPS.Models.Addressing.Enums;
using MIPS.Models.Instructions;
using System;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;

namespace MIPS.Assembler;

//                                          Overview
// ------------------------------------------------------------------------------------------------
//     This assembler works in two passes. The first pass will track all labels and macros. The 
// first pass will also allocate all static memory, replacing instructions with blank allocations.
// The second pass will then assemble all instructions and overwrite the blank allocations.
//

// TODO: Handle pseudo instructions

/// <summary>
/// A MIPS assembler.
/// </summary>
public partial class Assembler
{
    private readonly ObjectModuleConstructor _obj;
    private Segment _activeSegment;
    private int _currentLine;

    /// <summary>
    /// Initializes a new instance of the <see cref="Assembler"/> class.
    /// </summary>
    private Assembler()
    {
        _obj = new ObjectModuleConstructor();
        _activeSegment = Segment.Text;

        _instructionParser = new InstructionParser(_obj);
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

        // Create stream reader and run first pass
        using var reader = new StreamReader(stream);
        await assembler.MakePass(reader, assembler.LinePass1);

        // Seek to start and run second pass
        reader.BaseStream.Position = 0;
        await assembler.MakePass(reader, assembler.LinePass2);

        return assembler.Finish();
    }

    private async Task MakePass(StreamReader reader, Action<string> pass)
    {
        // Iterate until end of stream, tracking line number
        for (int lineNum = 0; !reader.EndOfStream; lineNum++)
        {
            var line = await reader.ReadLineAsync();
            Guard.IsNotNull(line, nameof(line));
            
            _currentLine = lineNum;
            pass(line);
        }
    }

    /// <summary>
    /// Creates a symbol at the current address.
    /// </summary>
    /// <param name="label">The name of the symbol</param>
    private void CreateSymbol(string label)
    {
        var address = CurrentAddress;
        if (!_obj.TryDefineSymbol(label, address))
        {
            // TODO: Log error
        }
    }

    private void Append(params byte[] bytes)
    {
        // Append data
        _obj.Append(_activeSegment, bytes);
    }

    private void Append(Instruction instruction)
        => Append(BitConverter.GetBytes((uint)instruction));

    private void Append(int byteCount)
    {
        Guard.IsGreaterThanOrEqualTo(byteCount, 0);

        Append(new byte[byteCount]);
    }

    private void SetActiveSegment(Segment segment)
    {
        _activeSegment = segment;
    }

    private ObjectModule Finish()
    {
        return _obj.Finish();
    }
}
