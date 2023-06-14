// Adam Dernis 2023

using CommunityToolkit.Diagnostics;
using MIPS.Assembler.Logging;
using MIPS.Assembler.Models.Modules;
using MIPS.Models.Addressing;
using MIPS.Models.Addressing.Enums;
using MIPS.Models.Instructions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Module = MIPS.Models.Modules.Module;

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
    private readonly ModuleConstruction _obj;
    private readonly AssemblerLogger _logger;
    private Section _activeSection;

    /// <summary>
    /// Initializes a new instance of the <see cref="Assembler"/> class.
    /// </summary>
    private Assembler()
    {
        _obj = new ModuleConstruction();
        _logger = new AssemblerLogger();
        _activeSection = Section.Text;
    }

    /// <summary>
    /// Gets the current segment address.
    /// </summary>
    internal Address CurrentAddress
    {
        get
        {
            return _activeSection switch
            {
                Section.Text => new Address(_obj.TextPosition, Section.Text),
                Section.Data => new Address(_obj.DataPosition, Section.Data),
                _ => ThrowHelper.ThrowArgumentException<Address>(nameof(_activeSection)),
            };
        }
    }
    
    /// <summary>
    /// Gets the assembler's logs.
    /// </summary>
    public IReadOnlyList<Log> Logs => _logger.Logs;

    /// <summary>
    /// Assembles an object module from a stream of assembly
    /// </summary>
    public static async Task<Assembler> AssembleAsync(Stream stream)
    {
        var assembler = new Assembler();

        // Create stream reader and run first pass
        using var reader = new StreamReader(stream);
        await assembler.MakePass(reader, assembler.LinePass1);
        await assembler.MakePass(reader, assembler.LinePass2);

        return assembler;
    }

    /// <summary>
    /// Builds the object module from an assembler.
    /// </summary>
    /// <param name="stream">The stream to write the module to.</param>
    /// <returns>The assembled object module.</returns>
    public Module WriteModule(Stream stream) => _obj.Finish(stream);

    private async Task MakePass(StreamReader reader, Action<string> pass)
    {
        // Iterate until end of stream, tracking line number
        for (int lineNum = 1; !reader.EndOfStream; lineNum++)
        {
            // Track the line in the logger
            _logger.CurrentLine = lineNum;

            // Load line from stream and make pass
            var line = await reader.ReadLineAsync();
            Guard.IsNotNull(line, nameof(line));

            CleanLine(ref line);
            pass(line);
        }

        // Reset stream position
        reader.BaseStream.Position = 0;
        _obj.ResetStreamPositions();
    }

    /// <summary>
    /// Creates a symbol at the current address.
    /// </summary>
    /// <param name="label">The name of the symbol.</param>
    private void CreateSymbol(string label) => CreateSymbol(label, CurrentAddress);

    /// <summary>
    /// Creates a symbol.
    /// </summary>
    /// <param name="label">The name of the symbol.</param>
    /// <param name="address">The value of the symbol.</param>
    private void CreateSymbol(string label, Address address)
    {
        if (!_obj.TryDefineSymbol(label, address))
        {
            // TODO: Log error
        }
    }

    private void Append(params byte[] bytes)
    {
        // Append data
        _obj.Append(_activeSection, bytes);
    }

    private void Append(Instruction instruction)
        => Append(BitConverter.GetBytes((uint)instruction));

    private void Append(int byteCount)
    {
        Guard.IsGreaterThanOrEqualTo(byteCount, 0);

        Append(new byte[byteCount]);
    }

    private void Align(int boundary) => _obj.Align(_activeSection, boundary);

    private void SetActiveSegment(Section section)
    {
        _activeSection = section;
    }
}
