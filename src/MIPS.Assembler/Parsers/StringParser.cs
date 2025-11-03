// Adam Dernis 2024

using CommunityToolkit.Diagnostics;
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
        => TryParse(input, '"', out literal);

    /// <summary>
    /// Attempts to parse a single character from a char statement.
    /// </summary>
    /// <param name="input">The char statement.</param>
    /// <param name="c">The char literal.</param>
    /// <returns>Whether or not a character was successfully parsed.</returns>
    public bool TryParseChar(string input, out char c)
    {
        c = default;

        if (!TryParse(input, '\'', out string literal))
            return false;

        if (literal.Length != 1)
        {
            _logger?.Log(Severity.Error, LogId.InvalidCharLiteral, "MustBeSingleCharacter");
            return false;
        }

        c = literal[0];
        return true;
    }

    private bool TryParse(string input, char wrap, out string literal)
    {
        literal = string.Empty;
        _escapeState = false;

        // Trim whitespace
        input = input.Trim();

        if (string.IsNullOrWhiteSpace(input))
            return false;

        // Ensure string begins and ends with quotes
        if (input[0] != wrap || input[^1] != wrap)
        {
            string expected = wrap switch
            {
                '"' => "String",
                '\'' => "Char",
                _ => ThrowHelper.ThrowArgumentOutOfRangeException<string>($"{wrap} expected to be either a ''' or '\"' character."),
            };

            _logger?.Log(Severity.Error, LogId.NotAString, $"Expected{expected}");
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
                if (!TryParseByChar(c, ref literal))
                    return false;
            }
        }

        if (_escapeState)
        {
            _logger?.Log(Severity.Error, LogId.IncompleteEscapeSequence, "IncompleteEscapeSequence");
            return false;
        }

        return true;
    }

    private bool TryParseByChar(char c, ref string literal)
    {
        switch (c)
        {
            case '"':
                _logger?.Log(Severity.Error, LogId.UnescapedQuoteInString, "UnescapedQuoteInString");
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
            _logger?.Log(Severity.Error, LogId.UnrecognizedEscapeSequence, "UnrecognizedEscapeSequence", @$"\{c}");
            return false;
        }

        literal += e;

        _escapeState = false;
        return true;
    }
}
