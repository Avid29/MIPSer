// Adam Dernis 2024

using CommunityToolkit.Diagnostics;
using MIPS.Assembler.Logging;
using MIPS.Assembler.Logging.Enum;
using MIPS.Assembler.Models.Modules;
using MIPS.Assembler.Tokenization;
using MIPS.Extensions.MIPS.Models.Instructions;
using MIPS.Models.Addressing;
using MIPS.Models.Addressing.Enums;
using MIPS.Models.Instructions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Numerics;
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
    /// Gets the current address.
    /// </summary>
    internal Address CurrentAddress
    {
        get
        {
            if (_activeSection is < Section.Text or > Section.UninitializedData)
                ThrowHelper.ThrowArgumentException(nameof(_activeSection));

            return new Address(_obj.GetStreamPosition(_activeSection), _activeSection);
        }
    }

    /// <summary>
    /// Gets the assembler's logs.
    /// </summary>
    public IReadOnlyList<Log> Logs => _logger.Logs;

    /// <summary>
    /// Gets whether or not the assembler failed to assemble a valid module.
    /// </summary>
    public bool Failed => _logger.Failed;

    /// <summary>
    /// Assembles an object module from a stream of assembly
    /// </summary>
    public static async Task<Assembler> AssembleAsync(Stream stream, string? filename)
    {
        var assembler = new Assembler();
        var tokens = await Tokenizer.TokenizeAsync(stream, filename, assembler._logger);

        for (int i = 1; i <= tokens.LineCount; i++)
        {
            assembler._logger.CurrentLine = i;
            assembler.LinePass1(tokens[i]);
        }

        assembler._obj.ResetStreamPositions();

        for (int i = 1; i <= tokens.LineCount; i++)
        {
            assembler._logger.CurrentLine = i;
            assembler.LinePass2(tokens[i]);
        }

        return assembler;
    }

    /// <summary>
    /// Builds the object module from an assembler.
    /// </summary>
    /// <param name="stream">The stream to write the module to.</param>
    /// <returns>The assembled object module.</returns>
    public Module? WriteModule(Stream stream)
    {
        // Don't write a failed module
        if (_logger.Failed)
            return null;

        return _obj.Finish(stream);
    }

    /// <summary>
    /// Creates a symbol at the current address.
    /// </summary>
    /// <remarks>
    /// At this stage, the label is expected to be passed in with a tailing ':' that will be trimmed.
    /// The method will still work if the semicolon is pre-trimmed.
    /// </remarks>
    /// <param name="label">The name of the symbol.</param>
    private bool CreateSymbol(string label) => CreateSymbol(label, CurrentAddress);

    /// <summary>
    /// Creates a symbol.
    /// </summary>
    /// <param name="label">The name of the symbol.</param>
    /// <param name="address">The value of the symbol.</param>
    /// <returns>True if successful, false on failure.</returns>
    private bool CreateSymbol(string label, Address address)
    {
        label = label.TrimEnd(':');
        if (!_obj.TryDefineSymbol(label, address))
        {
            _logger?.Log(Severity.Error, LogId.DuplicateSymbolDefinition, $"Symbol \"{label}\" already exists.");
            return false;
        }

        return true;
    }
}
