// Adam Dernis 2024

using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Zarem.Assembler.Config;
using Zarem.Assembler.Localization;
using Zarem.Assembler.Logging;
using Zarem.Assembler.MIPS.Tokenization;
using Zarem.Assembler.Models;
using Zarem.Models.Addressing;
using Zarem.Models.Modules.Tables;
using Module = Zarem.Models.Modules.Module;


namespace Zarem.Assembler;

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
public partial class MIPSAssembler : IAssembler<MIPSAssemblerConfig>
{
    private readonly Logger _logger;
    private readonly Module _module;
    private ModuleSection _activeSection;

    /// <summary>
    /// Initializes a new instance of the <see cref="MIPSAssembler"/> class.
    /// </summary>
    private MIPSAssembler(MIPSAssemblerConfig config, Logger? logger = null)
    {
        _logger = logger ?? new Logger();
        _logger.Register(new SetLocalizer("Zarem.Assembler.Resources.Logger", typeof(MIPSAssembler).Assembly));

        _module = new Module();
        _module.AddSection(".text");
        _module.AddSection(".data");
        _activeSection = _module.Sections[".text"];

        Config = config;
        Context = new(this, _module);
    }

    /// <inheritdoc/>
    public MIPSAssemblerConfig Config { get; }

    /// <inheritdoc/>
    public Address CurrentAddress => new(_activeSection.Stream.Position, _activeSection.Name);

    /// <summary>
    /// Gets the assembler context for this assembler instance.
    /// </summary>
    internal MIPSAssemblerContext Context { get; }

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
    private static async Task<MIPSAssembler> AssembleAsync(TextReader reader, string? filename, MIPSAssemblerConfig config, Logger? logger = null)
    {
        logger?.Flush();

        var assembler = new MIPSAssembler(config, logger);
        var tokens = await Tokenizer.TokenizeAsync(reader, filename);

        // Run the alignment pass on each line
        for (int i = 1; i <= tokens.LineCount; i++)
            assembler.AlignmentPass(tokens[i]);

        // Reset all streams to start
        assembler._activeSection = assembler._module.Sections[".text"];
        assembler._module.ResetStreamPositions();

        // Run the realization pass on each line
        for (int i = 1; i <= tokens.LineCount; i++)
            assembler.RealizationPass(tokens[i]);

        return assembler;
    }
}
