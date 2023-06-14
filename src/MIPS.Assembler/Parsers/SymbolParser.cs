// Adam Dernis 2023

using MIPS.Assembler.Logging.Enum;
using MIPS.Assembler.Logging;
using System.Linq;

namespace MIPS.Assembler.Parsers;

/// <summary>
/// A struct for parsing symbols.
/// </summary>
public readonly struct SymbolParser
{
    private readonly ILogger? _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="SymbolParser"/> struct.
    /// </summary>
    public SymbolParser(ILogger? logger)
    {
        _logger = logger;
    }

    /// <summary>
    /// Checks if <paramref name="symbol"/> is a legal symbol name.
    /// </summary>
    /// <param name="symbol">The name of the symbol.</param>
    /// <returns><see cref="true"/> if the symbol name is legal. <see cref="false"/> otherwise.</returns>
    public bool ValidateSymbolName(string symbol)
    {
        symbol = symbol.Trim();

        // No characters may be whitespace
        if (symbol.Any(char.IsWhiteSpace))
        {
            _logger?.Log(Severity.Error, LogId.IllegalSymbolName, $"'{symbol}' is not a legal symbol name. Symbols may not contain whitespace.");
            return false;
        }

        // Labels may not begin with a digit
        if (char.IsDigit(symbol[0]))
        {
            _logger?.Log(Severity.Error, LogId.IllegalSymbolName, $"'{symbol}' is not a legal symbol name. Symbols may not begin with a digit.");
            return false;
        }

        // All characters must be a letter or a digit
        if (!symbol.All(char.IsLetterOrDigit))
        {
            _logger?.Log(Severity.Error, LogId.IllegalSymbolName, $"'{symbol}' is not a legal symbol name.");
            return false;
        }

        return true;
    }
}
