// Adam Dernis 2024

using MIPS.Assembler.Logging.Enum;
using MIPS.Assembler.Tokenization.Models;
using System.IO;

namespace MIPS.Assembler.Logging;

/// <summary>
/// An <see cref="ILog"/> that occurred in the assembler stage.
/// </summary>
public class AssemblerLog : ILog
{
    /// <summary>
    /// Initializes a new instance of the <see cref="AssemblerLog"/> class.
    /// </summary>
    internal AssemblerLog(Severity severity, LogCode code, string message, Token[] tokens)
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
    public int Line => Tokens[0].Location.Line;

    /// <summary>
    /// Gets the starting index of the log.
    /// </summary>
    public int Start => Tokens[0].Location.Index;
    
    /// <summary>
    /// Gets the ending index of the log.
    /// </summary>
    public int End
    {
        get
        {
            var last = Tokens[^1];
            return last.Location.Index + last.Source.Length;
        }
    }
    
    /// <inheritdoc/>
    int? ILog.Line => Line;
}
