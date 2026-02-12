// Adam Dernis 2024

using System.IO;
using Zarem.Assembler.Tokenization.Models;
using Zarem.Assembler.Logging.Enum;

namespace Zarem.Assembler.Logging;

/// <summary>
/// An <see cref="ILog"/> that occurred in the assembler stage.
/// </summary>
public class AssemblerLogEntry : ILog
{
    /// <summary>
    /// Initializes a new instance of the <see cref="AssemblerLogEntry"/> class.
    /// </summary>
    internal AssemblerLogEntry(Severity severity, LogCode code, string message, Token[] tokens)
    {
        Code = code;
        Severity = severity;
        Message = message;
        Tokens = tokens;
    }

    /// <inheritdoc/>
    public LogCode Code { get; }
    
    /// <inheritdoc/>
    public Severity Severity { get; }
    
    /// <inheritdoc/>
    public string Message { get; }
    
    /// <inheritdoc/>
    public string? FileName => Path.GetFileName(FilePath);
    
    /// <inheritdoc/>
    public string? FilePath => Tokens[0].FilePath;

    /// <summary>
    /// Gets the tokens that caused the log.
    /// </summary>
    public Token[] Tokens { get; }
    
    /// <inheritdoc/>
    public SourceLocation Location => Tokens[0].Location;
    
    /// <inheritdoc/>
    SourceLocation? ILog.Location => Location;
}
