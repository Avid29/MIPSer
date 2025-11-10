// Avishai Dernis 2025

using MIPS.Assembler.Logging.Enum;

namespace MIPS.Assembler.Logging;

/// <summary>
/// An <see langword="interface"/> for an entry in the <see cref="ILogger"/>.
/// </summary>
public interface ILog
{
    /// <summary>
    /// Get the log's code.
    /// </summary>
    public LogCode Code { get; }
    
    /// <summary>
    /// Gets the log's severity.
    /// </summary>
    public Severity Severity { get; }
    
    /// <summary>
    /// Gets the log's message.
    /// </summary>
    public string Message { get; }
    
    /// <summary>
    /// Gets the name of the file where the log occurred.
    /// </summary>
    public string? FileName { get; }

    /// <summary>
    /// Gets the path of the file where the log occurred.
    /// </summary>
    public string? FilePath { get; }

    /// <summary>
    /// Gets the line where in the file where the log occurred.
    /// </summary>
    public int? Line { get; }
}
