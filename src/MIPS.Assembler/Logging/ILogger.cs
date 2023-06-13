// Adam Dernis 2023

using MIPS.Assembler.Logging.Enum;

namespace MIPS.Assembler.Logging;

/// <summary>
/// An interface for the <see cref="AssemblerLogger"/> that only creating logs, not reading or managing logs.
/// </summary>
public interface ILogger
{
    /// <summary>
    /// Creates a new log.
    /// </summary>
    /// <param name="severity">The severity of the consequences of the log.</param>
    /// <param name="id">The id of the log.</param>
    /// <param name="message">The log message.</param>
    void Log(Severity severity, LogId id, string message);
}
