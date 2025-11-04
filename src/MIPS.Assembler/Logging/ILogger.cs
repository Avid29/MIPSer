// Adam Dernis 2024

using MIPS.Assembler.Logging.Enum;
using MIPS.Assembler.Tokenization;
using System;

namespace MIPS.Assembler.Logging;

/// <summary>
/// An interface for the <see cref="Logger"/> that only allows creating logs, not reading or managing logs.
/// </summary>
public interface ILogger
{
    /// <inheritdoc cref="Log(Severity, LogId, ReadOnlySpan{Token}, string, object?[])"/>
    void Log(Severity severity, LogId id, Token token, string messageKey, params object?[] args);

    /// <summary>
    /// Creates a new log.
    /// </summary>
    /// <param name="severity">The severity of the consequences of the log.</param>
    /// <param name="id">The id of the log.</param>
    /// <param name="tokens">The token(s) where the log occured.</param>
    /// <param name="messageKey">The log resource key for the log message .</param>
    /// <param name="args">The arguments to format the message with.</param>
    void Log(Severity severity, LogId id, ReadOnlySpan<Token> tokens, string messageKey, params object?[] args);
}
