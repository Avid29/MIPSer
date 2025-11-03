// Adam Dernis 2024

using MIPS.Assembler.Helpers;
using MIPS.Assembler.Logging.Enum;
using System.Collections.Generic;
using System.Reflection;
using System.Resources;

namespace MIPS.Assembler.Logging;

/// <summary>
/// An <see cref="ILogger"/> for assembly/linker errors, warnings, and messages.
/// </summary>
public class Logger : ILogger
{
    private readonly List<Log> _logs;
    private readonly Localizer _localizer;

    /// <summary>
    /// Initializes a new instance of the <see cref="Logger"/> class.
    /// </summary>
    public Logger()
    {
        _localizer = new Localizer("MIPS.Assembler.Resources.Logger");
        _logs = [];
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
    public void Log(Severity severity, LogId id, string message, params object?[] args)
    {
        var localizedMessage = _localizer[message];
        var formattedMessage = string.Format(localizedMessage, args);

        var log = new Log(id, formattedMessage, severity, CurrentLine);
        _logs.Add(log);

        if (severity is Severity.Error)
            Failed = true;
    }

    /// <summary>
    /// Gets a readonly list of logs.
    /// </summary>
    public IReadOnlyList<Log> Logs => _logs;
}
