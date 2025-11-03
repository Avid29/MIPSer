// Adam Dernis 2024

using MIPS.Assembler.Helpers;
using MIPS.Assembler.Logging.Enum;
using MIPS.Assembler.Tokenization;
using System;
using System.Collections.Generic;

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

    /// <inheritdoc/>
    public void Log(Severity severity, LogId id, ReadOnlySpan<Token> token, string messageKey, params object?[] args)
        => Log(severity, id, token.Length > 0 ? token[0].LineNum : 0, messageKey, args);

    /// <inheritdoc/>
    public void Log(Severity severity, LogId id, Token token, string messageKey, params object?[] args)
        => Log(severity, id, [token], messageKey, args);
    
    /// <inheritdoc/>
    public void Log(Severity severity, LogId id, string messageKey, params object?[] args)
    {
        throw new NotImplementedException();
    }
    
    /// <inheritdoc/>
    public void Log(Severity severity, LogId id, int lineNum, string messageKey, params object?[] args)
    {
        var localizedMessage = _localizer[messageKey];
        var formattedMessage = string.Format(localizedMessage, args);

        var log = new Log(id, formattedMessage, severity, lineNum);
        _logs.Add(log);

        if (severity is Severity.Error)
            Failed = true;
    }

    /// <summary>
    /// Gets a readonly list of logs.
    /// </summary>
    public IReadOnlyList<Log> Logs => _logs;
}
