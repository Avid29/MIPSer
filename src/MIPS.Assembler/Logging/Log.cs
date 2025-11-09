// Adam Dernis 2024

using MIPS.Assembler.Logging.Enum;
using MIPS.Assembler.Tokenization.Models;
using System;
using System.IO;

namespace MIPS.Assembler.Logging;

/// <summary>
/// An error, warning, or message event to track in the <see cref="ILogger"/>.
/// </summary>
public class Log
{
    /// <summary>
    /// Initializes a new instance of the <see cref="Log"/> class.
    /// </summary>
    internal Log(LogId id, string message, Severity severity, Token[] tokens)
    {
        Id = id;
        Message = message;
        Severity = severity;
        Tokens = tokens;
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
    /// Gets the tokens that caused the log.
    /// </summary>
    public Token[] Tokens { get; }

    /// <summary>
    /// Gets the name or path of the file where the log occurred.
    /// </summary>
    public string? File => Tokens[0].FilePath;

    /// <summary>
    /// Gets the name of the file where the log occurred.
    /// </summary>
    public string? FileName => Path.GetFileName(File);

    /// <summary>
    /// Gets the where the log occurred.
    /// </summary>
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
}
