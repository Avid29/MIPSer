// Avishai Dernis 2025

using MIPS.Assembler.Logging;
using System.Collections.Generic;

namespace Mipser.Messages.Build;

/// <summary>
/// A message sent when a file is assembled.
/// </summary>
public class FileAssembledMessage
{
    /// <summary>
    /// Initializes a new instance of the <see cref="FileAssembledMessage"/> class.
    /// </summary>
    public FileAssembledMessage(string assemblyFile, bool failed, IReadOnlyList<AssemblerLog>? logs)
    {
        AssemblyFile = assemblyFile;
        Failed = failed;
        Logs = logs;
    }

    /// <summary>
    /// Gets the source assembly file.
    /// </summary>
    public string AssemblyFile { get; }

    /// <summary>
    /// Gets whether or not the file assembly failed.
    /// </summary>
    public bool Failed { get; }

    /// <summary>
    /// Gets the log events from the file assembly.
    /// </summary>
    public IReadOnlyList<AssemblerLog>? Logs { get; }
}
