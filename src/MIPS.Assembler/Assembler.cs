﻿// Adam Dernis 2024

using CommunityToolkit.Diagnostics;
using MIPS.Assembler.Logging;
using MIPS.Assembler.Logging.Enum;
using MIPS.Assembler.Models;
using MIPS.Assembler.Models.Modules;
using MIPS.Assembler.Parsers;
using MIPS.Assembler.Tokenization;
using MIPS.Models.Addressing;
using MIPS.Models.Addressing.Enums;
using MIPS.Models.Modules.Tables.Enums;
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

/// <summary>
/// A MIPS assembler.
/// </summary>
public partial class Assembler
{
    private readonly AssemblerLogger _logger;
    private readonly ModuleConstruction _module;
    private Section _activeSection;

    /// <summary>
    /// Initializes a new instance of the <see cref="Assembler"/> class.
    /// </summary>
    private Assembler(AssemblerConfig config)
    {
        _logger = new AssemblerLogger();
        _module = new ModuleConstruction();
        _activeSection = Section.Text;

        Config = config;
        Context = new(this, _module);
    }

    /// <summary>
    /// Gets the assembler context for this assembler instance.
    /// </summary>
    public AssemblerContext Context { get; }

    /// <summary>
    /// Gets the assembler's configuation.
    /// </summary>
    public AssemblerConfig Config { get; }

    /// <summary>
    /// Gets the current address.
    /// </summary>
    internal Address CurrentAddress
    {
        get
        {
            if (_activeSection is < Section.Text or > Section.UninitializedData)
                ThrowHelper.ThrowArgumentException(nameof(_activeSection));

            return new Address(_module.GetStreamPosition(_activeSection), _activeSection);
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
    public static async Task<Assembler> AssembleAsync(Stream stream, string? filename, AssemblerConfig? config = null)
    {
        if (config is null)
        {
            config = AssemblerConfig.Default;
        }

        var assembler = new Assembler(config);
        var tokens = await Tokenizer.TokenizeAsync(stream, filename, assembler._logger);

        for (int i = 1; i <= tokens.LineCount; i++)
        {
            assembler._logger.CurrentLine = i;
            assembler.AlignmentPass(tokens[i]);
        }

        assembler._activeSection = Section.Text;
        assembler._module.ResetStreamPositions();

        for (int i = 1; i <= tokens.LineCount; i++)
        {
            assembler._logger.CurrentLine = i;
            assembler.RealizationPass(tokens[i]);
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

        return _module.Finish(stream);
    }

    /// <summary>
    /// Defines a label at the current address.
    /// </summary>
    /// <remarks>
    /// At this stage, the label is expected to be passed in with a tailing ':' that will be trimmed.
    /// The method will still work if the semicolon is pre-trimmed.
    /// </remarks>
    /// <param name="label">The name of the symbol.</param>
    private bool DefineLabel(string label) => DefineSymbol(label, CurrentAddress, SymbolFlags.Def_Label);

    /// <summary>
    /// Defines a symbol.
    /// </summary>
    /// <param name="label">The name of the symbol.</param>
    /// <param name="address">The value of the symbol.</param>
    /// <param name="flags">The flags of the symbol.</param>
    /// <returns>True if successful, false on failure.</returns>
    private bool DefineSymbol(string label, Address address, SymbolFlags flags)
    {
        label = label.TrimEnd(':');
        if (!ValidateSymbolName(label))
            return false;

        if (!_module.DefineOrUpdateSymbol(label, address, flags))
        {
            _logger?.Log(Severity.Error, LogId.DuplicateSymbolDefinition, $"Symbol \"{label}\" is already defined.");
            return false;
        }

        return true;
    }
}
