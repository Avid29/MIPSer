// Adam Dernis 2023

using MIPS.Assembler.Logging.Enum;
using System.Collections.Generic;

namespace MIPS.Assembler.Logging;

/// <summary>
/// An <see cref="ILogger"/> for assembly errors, warnings, and messages.
/// </summary>
public class AssemblerLogger : ILogger
{
    private readonly List<Log> _logs;

    /// <summary>
    /// Initializes a new instance of the <see cref="AssemblerLogger"/> class.
    /// </summary>
    public AssemblerLogger()
    {
        _logs = new List<Log>();
    }

    /// <summary>
    /// Gets or sets the current line being assembled.
    /// </summary>
    public int CurrentLine { get; set; }

    /// <inheritdoc/>
    public void Log(Severity severity, LogId id, string message)
    {
        var log = new Log(id, message, severity, CurrentLine);
        _logs.Add(log);
    }

    /// <summary>
    /// Gets a readonly list of logs.
    /// </summary>
    public IReadOnlyList<Log> Logs => _logs;
}
