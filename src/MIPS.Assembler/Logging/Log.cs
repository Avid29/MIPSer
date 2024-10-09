// Adam Dernis 2024

using MIPS.Assembler.Logging.Enum;

namespace MIPS.Assembler.Logging;

/// <summary>
/// An error, warning, or message event to track in the <see cref="ILogger"/>.
/// </summary>
public class Log
{
    /// <summary>
    /// Initializes a new instance of the <see cref="Log"/> class.
    /// </summary>
    internal Log(LogId id, string message, Severity severity, int lineNumber)
    {
        Id = id;
        Message = message;
        Severity = severity;
        LineNumber = lineNumber;
    }

    /// <summary>
    /// Get the log's id.
    /// </summary>
    /// <remarks>
    /// Log id's are not unique to log instances.
    /// </remarks>
    public LogId Id { get; }

    /// <summary>
    /// Gets the log's message.
    /// </summary>
    public string Message { get; }

    /// <summary>
    /// Gets the log's severity.
    /// </summary>
    public Severity Severity { get; }

    /// <summary>
    /// Gets the line where the even was logged.
    /// </summary>
    public int LineNumber { get; }
}
