// Adam Dernis 2024

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
    /// Gets a value indicating whether or not assembly failed.
    /// </summary>
    public bool Failed { get; private set; }

    /// <summary>
    /// Gets or sets the current line being assembled.
    /// </summary>
    public int CurrentLine { get; set; }

    /// <inheritdoc/>
    public void Log(Severity severity, LogId id, string message, int? line = null)
    {
        var log = new Log(id, message, severity, line ?? CurrentLine);
        _logs.Add(log);

        if (severity is Severity.Error)
            Failed = true;
    }

    /// <summary>
    /// Gets a readonly list of logs.
    /// </summary>
    public IReadOnlyList<Log> Logs => _logs;
}
