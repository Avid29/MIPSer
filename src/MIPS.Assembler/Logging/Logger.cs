// Adam Dernis 2024

using MIPS.Assembler.Helpers;
using MIPS.Assembler.Logging.Enum;
using MIPS.Assembler.Tokenization.Models;
using System;
using System.Collections.Generic;

namespace MIPS.Assembler.Logging;

/// <summary>
/// An <see cref="ILogger"/> for assembly/linker errors, warnings, and messages.
/// </summary>
public class Logger : ILogger
{
    private readonly List<AssemblerLog> _logs;
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
    public bool Log(Severity severity, LogCode code, string? file, string messageKey, params object[] args)
    {
        throw new NotImplementedException();
    }

    /// <inheritdoc/>
    public bool Log(Severity severity, LogCode code, Token token, string messageKey, params object?[] args)
        => Log(severity, code, [token], messageKey, args);
    
    /// <inheritdoc/>
    public bool Log(Severity severity, LogCode code, ReadOnlySpan<Token> tokens, string messageKey, params object?[] args)
    {
        var localizedMessage = _localizer[messageKey];
        var formattedMessage = string.Format(localizedMessage, args);

        var log = new AssemblerLog(severity, code, formattedMessage, tokens.ToArray());
        _logs.Add(log);

        if (severity is Severity.Error)
            Failed = true;

        return severity is not Severity.Error;
    }

    /// <summary>
    /// Gets a readonly list of logs.
    /// </summary>
    public IReadOnlyList<ILog> Logs => _logs;
}
