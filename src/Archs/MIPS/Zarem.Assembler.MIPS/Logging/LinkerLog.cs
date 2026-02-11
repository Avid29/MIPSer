// Avishai Dernis 2025

using Zarem.Assembler.MIPS.Logging.Enum;
using Zarem.Assembler.MIPS.Tokenization.Models;
using System.IO;

namespace Zarem.Assembler.MIPS.Logging;

/// <summary>
/// An <see cref="ILog"/> that occurred in the linker stage.
/// </summary>
public class LinkerLog : ILog
{
    /// <summary>
    /// Initializes a new instance of the <see cref="LinkerLog"/> class.
    /// </summary>
    internal LinkerLog(Severity severity, LogCode code, string message, string file)
    {
        Code = code;
        Severity = severity;
        Message = message;
        FilePath = file;
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
    public string? FilePath { get; }
    
    /// <inheritdoc/>
    public SourceLocation? Location => null;
}
