// Adam Dernis 2023

using MIPS.Assembler.Logging;
using MIPS.Assembler.Logging.Enum;

namespace MIPS.Assembler.Parsers;

/// <summary>
/// Parses string statements into string literals.
/// </summary>
public struct StringParser
{
    private readonly ILogger? _logger;
    private bool _escapeState;

    /// <summary>
    /// Initializes a new instance of the <see cref="StringParser"/> struct.
    /// </summary>
    public StringParser(ILogger? logger)
    {
        _logger = logger;
        _escapeState = false;
    }

    /// <summary>
    /// Attempts to parse a literal string from a string statement.
    /// </summary>
    /// <param name="input">The string statement.</param>
    /// <param name="literal">The resulting string literal</param>
    /// <returns><see cref="true"/> if the string was successfully parsed. <see cref="false"/> otherwise.</returns>
    public bool TryParseString(string input, out string literal)
    {
        literal = string.Empty;

        // Trim whitespace
        input = input.Trim();

        if (string.IsNullOrWhiteSpace(input))
            return false;

        // Ensure string begins and ends with quotes
        if (input[0] != '"' || input[^1] != '"')
        {
            _logger?.Log(Severity.Error, LogId.NotAString, $"'{input}' is not a string.");
            return false;
        }

        foreach (char c in input[1..^1])
        {
            if (_escapeState)
            {
                if (!TryParseCharFromEscape(c, ref literal))
                    return false;
            }
            else
            {
                if (!TryParseChar(c, ref literal))
                    return false;
            }
        }

        return true;
    }

    private bool TryParseChar(char c, ref string literal)
    {
        switch (c)
        {
            case '"':
                _logger?.Log(Severity.Error, LogId.UnescapedQuoteInString, "Unespaced quote inside string.");
                return false;
            case '\\':
                _escapeState = true;
                break;
            default:
                literal += c;
                break;
        }
        return true;
    }

    private bool TryParseCharFromEscape(char c, ref string literal)
    {
        char e = c switch
        {
            'a' => '\a',
            'b' => '\b',
            'f' => '\f',
            'n' => '\n',
            'r' => '\r',
            't' => '\t',
            'v' => '\v',
            '\\' => '\\',
            '\'' => '\'',
            '"' => '\"',
            _ => (char)0,
        };

        if (e == (char)0)
        {
            _logger?.Log(Severity.Error, LogId.UnrecognizedEscapeSequence, $"'\\{c}' was not a recognized escape sequence.");
            return false;
        }

        literal += e;

        _escapeState = false;
        return true;
    }
}
