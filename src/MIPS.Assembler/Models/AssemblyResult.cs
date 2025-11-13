// Avishai Dernis 2025

using MIPS.Assembler.Logging;
using System.Collections.Generic;

namespace MIPS.Assembler.Models;

/// <summary>
/// A class containing the results of an assembler run.
/// </summary>
public class AssemblyResult
{
    internal AssemblyResult(bool failed, IReadOnlyList<AssemblerLog> logs)
    {
        Failed = failed;
        Logs = logs;
    }

    /// <summary>
    /// Gets whether or not the assembly failed.
    /// </summary>
    public bool Failed { get; }

    /// <summary>
    /// Gets the assmembly log.
    /// </summary>
    public IReadOnlyList<AssemblerLog> Logs { get; }
}
