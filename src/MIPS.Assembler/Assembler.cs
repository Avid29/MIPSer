// Adam Dernis 2024

using CommunityToolkit.Diagnostics;
using MIPS.Assembler.Logging;
using MIPS.Assembler.Logging.Enum;
using MIPS.Assembler.Models;
using MIPS.Assembler.Models.Modules;
using MIPS.Assembler.Models.Modules.Interfaces;
using MIPS.Assembler.Tokenization;
using MIPS.Assembler.Tokenization.Models;
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
    /// Gets the assembler's configuration.
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
    public static async Task<Assembler> AssembleAsync(string str, string? filename, AssemblerConfig config)
    {
        using var reader = new StringReader(str);
        return await AssembleAsync(reader, filename, config);
    }
    
    /// <summary>
    /// Assembles an object module from a stream of assembly.
    /// </summary>
    public static async Task<Assembler> AssembleAsync(Stream stream, string? filename, AssemblerConfig config)
    {
        using var reader = new StreamReader(stream);
        return await AssembleAsync(reader, filename, config);
    }

    /// <summary>
    /// Assembles an object module from a stream of assembly.
    /// </summary>
    public static async Task<Assembler> AssembleAsync(TextReader reader, string? filename, AssemblerConfig config)
    {
        var assembler = new Assembler(config);
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

    /// <summary>
    /// Writes the assembled module to a module stream of the provided format.
    /// </summary>
    /// <typeparam name="T">The format of module to write.</typeparam>
    /// <param name="stream">The stream to write the module to.</param>
    /// <returns>The module object.</returns>
    public T? CompleteModule<T>(Stream stream)
        where T : IBuildModule<T> => T.Create(_module, Config, stream);
}
