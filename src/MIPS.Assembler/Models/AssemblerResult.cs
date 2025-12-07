// Avishai Dernis 2025

using MIPS.Assembler.Logging;
using MIPS.Models.Modules.Tables;
using System.Collections.Generic;

namespace MIPS.Assembler.Models;

/// <summary>
/// A class containing the results of an assembler run.
/// </summary>
public class AssemblerResult
{
    internal AssemblerResult(bool failed, IReadOnlyList<AssemblerLogEntry> logs, IReadOnlyList<SymbolEntry> symbols)
    {
        Failed = failed;
        Logs = logs;
        Symbols = symbols;
    }

    /// <summary>
    /// Gets whether or not the assembly failed.
    /// </summary>
    public bool Failed { get; }

    /// <summary>
    /// Gets the assmembly log.
    /// </summary>
    public IReadOnlyList<AssemblerLogEntry> Logs { get; }

    /// <summary>
    /// Gets the list of symbols
    /// </summary>
    public IReadOnlyList<SymbolEntry> Symbols { get;  }


}
