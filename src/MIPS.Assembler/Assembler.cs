// Adam Dernis 2024

using CommunityToolkit.Diagnostics;
using MIPS.Assembler.Logging;
using MIPS.Assembler.Models;
using MIPS.Assembler.Models.Config;
using MIPS.Assembler.Models.Modules;
using MIPS.Assembler.Tokenization;
using MIPS.Models.Addressing;
using MIPS.Models.Addressing.Enums;
using MIPS.Models.Modules.Tables;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace MIPS.Assembler;

//                                          Overview
// ------------------------------------------------------------------------------------------------
//     This assembler works in two passes.
//
//     Pass 1 - Alignment Pass:
//      - Track all labels and macros
//      - Assess instruction size
//        - Real instructions are 4 bytes
//        - Pseudo instructions have a real instruction count
//      - Allocate memory
//        - Note: Memory will be assigned as well where possible,
//          but all memory will be overwritten on the second pass.
//
//     Pass 2 - Realization Pass:
//      - Assemble instructions
//      - Initialize allocated memory
//

/// <summary>
/// A MIPS assembler.
/// </summary>
public partial class Assembler
{
    private readonly Logger _logger;
    private readonly Module _module;
    private Section _activeSection;

    /// <summary>
    /// Initializes a new instance of the <see cref="Assembler"/> class.
    /// </summary>
    private Assembler(AssemblerConfig config, Logger? logger = null)
    {
        _logger = logger ?? new Logger();

        _module = new Module();
        _activeSection = Section.Text;

        Config = config;
        Context = new(this, _module);
    }

    /// <summary>
    /// Gets the assembler context for this assembler instance.
    /// </summary>
    internal AssemblerContext Context { get; }

    /// <summary>
    /// Gets the assembler's configuration.
    /// </summary>
    internal AssemblerConfig Config { get; }

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
    public IReadOnlyList<AssemblerLogEntry> Logs => [.._logger.CurrentLog.OfType<AssemblerLogEntry>()];

    /// <summary>
    /// Gets the symbols found by the assembler.
    /// </summary>
    public IReadOnlyList<SymbolEntry> Symbols => [.._module.Symbols.Values];

    /// <summary>
    /// Gets whether or not the assembler failed to assemble a valid module.
    /// </summary>
    public bool Failed => _logger.CurrentFailed;

    /// <summary>
    /// Assembles an object module from a stream of assembly.
    /// </summary>
    private static async Task<Assembler> AssembleAsync(TextReader reader, string? filename, AssemblerConfig config, Logger? logger = null)
    {
        logger?.Flush();

        var assembler = new Assembler(config, logger);
        var tokens = await Tokenizer.TokenizeAsync(reader, filename);

        // Run the alignment pass on each line
        for (int i = 1; i <= tokens.LineCount; i++)
            assembler.AlignmentPass(tokens[i]);

        // Reset all streams to start
        assembler._activeSection = Section.Text;
        assembler._module.ResetStreamPositions();

        // Run the realization pass on each line
        for (int i = 1; i <= tokens.LineCount; i++)
            assembler.RealizationPass(tokens[i]);

        return assembler;
    }
}
