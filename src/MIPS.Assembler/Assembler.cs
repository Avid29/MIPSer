﻿// Adam Dernis 2024

using CommunityToolkit.Diagnostics;
using MIPS.Assembler.Logging;
using MIPS.Assembler.Logging.Enum;
using MIPS.Assembler.Models;
using MIPS.Assembler.Models.Modules;
using MIPS.Assembler.Models.Modules.Interfaces;
using MIPS.Assembler.Tokenization;
using MIPS.Models.Addressing;
using MIPS.Models.Addressing.Enums;
using MIPS.Models.Modules.Tables.Enums;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace MIPS.Assembler;

//                                          Overview
// ------------------------------------------------------------------------------------------------
//     This assembler works in two passes.
//
//     Pass 1 - Alignment Pass:
//      - Track all labels and marcos
//      - Assess instruction size
//        - All acutal instructions are bytes
//        - Pseudo instructions have a real instruction count
//      - Allocate memory
//        - Note: Memory will be assigned as well where possible,
//          but all memory will be overwritten on the second pass.
//
//     Pass 2 - Realization Pass:
//      - Assemble instructions
//      - Assign memory
//

/// <summary>
/// A MIPS assembler.
/// </summary>
public partial class Assembler
{
    private readonly Logger _logger;
    private readonly ModuleConstructor _module;
    private Section _activeSection;

    /// <summary>
    /// Initializes a new instance of the <see cref="Assembler"/> class.
    /// </summary>
    private Assembler(AssemblerConfig config)
    {
        _logger = new Logger();
        _module = new ModuleConstructor();
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
    /// Assembles an object module from a stream of assembly.
    /// </summary>
    public static async Task<Assembler> AssembleAsync(Stream stream, string? filename, AssemblerConfig config)
    {
        var assembler = new Assembler(config);
        var tokens = await Tokenizer.TokenizeAsync(stream, filename, assembler._logger);

        // Run the alignment pass on each line
        for (int i = 1; i <= tokens.LineCount; i++)
        {
            assembler._logger.CurrentLine = i;
            assembler.AlignmentPass(tokens[i]);
        }

        // Reset all streams to start
        assembler._activeSection = Section.Text;
        assembler._module.ResetStreamPositions();

        // Run the realization pass on each line
        for (int i = 1; i <= tokens.LineCount; i++)
        {
            assembler._logger.CurrentLine = i;
            assembler.RealizationPass(tokens[i]);
        }

        return assembler;
    }

    /// <summary>
    /// Writes the assembled module to a module stream of the provided format.
    /// </summary>
    /// <typeparam name="T">The format of module to write.</typeparam>
    /// <param name="stream">The stream to write the module to.</param>
    /// <returns>The module object.</returns>
    public T? CompleteModule<T>(Stream stream)
        where T : IBuildModule<T> => T.Create(_module, Config, stream);

    /// <summary>
    /// Defines a label at the current address.
    /// </summary>
    /// <remarks>
    /// At this stage, the label is expected to be passed in with a tailing ':' that will be trimmed.
    /// The method will still work if the semicolon is pre-trimmed.
    /// </remarks>
    /// <param name="label">The name of the symbol.</param>
    private bool DefineLabel(string label) => DefineSymbol(label.TrimEnd(':'), CurrentAddress, SymbolType.Label);

    /// <summary>
    /// Defines a symbol.
    /// </summary>
    /// <param name="label">The name of the symbol.</param>
    /// <param name="address">The value of the symbol.</param>
    /// <param name="type">The symbol type.</param>
    /// <param name="flags">The flag info for the symbol.</param>
    /// <returns>True if successful, false on failure.</returns>
    private bool DefineSymbol(string label, Address address, SymbolType type, SymbolFlags flags = 0)
    {
        // Ensure the symbol has a valid name
        if (!ValidateSymbolName(label))
            return false;

        // Define the symbol or update by adding flags, address or type.
        // NOTE: The type can only be updated if it is currently unknown
        //       and the address can only be updated if it's undeclared/external.
        if (!_module.TryDefineOrUpdateSymbol(label, type, address))
        {
            _logger?.Log(Severity.Error, LogId.DuplicateSymbolDefinition, $"Symbol \"{label}\" is already defined.");
            return false;
        }

        return true;
    }
}
