// Avishai Dernis 2025

using CommunityToolkit.Diagnostics;
using MIPS.Assembler.Logging;
using MIPS.Assembler.Logging.Enum;
using MIPS.Assembler.Tokenization.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.IO;
using System.Threading.Tasks;

namespace MIPS.Assembler.Tokenization;

/// <summary>
/// A class for tokenizing an assembly file.
/// </summary>
public class Tokenizer
{
    private readonly ILogger? _logger;

    private TokenizerMode _mode;
    private TokenizerState _state;
    private string _cache;
    private TokenType? _tokenType;

    private readonly string? _filename;
    private int _line;
    private int _column;
    private int _cacheColumn;

    /// <summary>
    /// Initializes a new instance of the <see cref="Tokenizer"/> class.
    /// </summary>
    private Tokenizer(string? filename, ILogger? logger = null, TokenizerMode mode = TokenizerMode.Assembly)
    {
        _logger = logger;

        TokenLines = [];
        Tokens = [];
        _mode = mode;
        _state = TokenizerState.LineBegin;
        _cache = string.Empty;
        _tokenType = null;

        _filename = filename;
        Line = 1;
        _column = 0;
        _mode = mode;
    }

    private List<AssemblyLine> TokenLines { get; }

    private List<Token> Tokens { get; set; }

    private int Line
    {
        get => _line;
        set => _line = value;
    }

    /// <inheritdoc/>
    public static async Task<TokenizedAssmebly> TokenizeAsync(Stream stream, string? filename = null, ILogger? logger = null)
    {
        using var reader = new StreamReader(stream);
        return await TokenizeAsync(reader, filename, logger);
    }

    /// <summary>
    /// Tokenizes a stream of assembly code.
    /// </summary>
    /// <param name="reader">The stream of code.</param>
    /// <param name="filename">The filename of the stream.</param>
    /// <param name="logger">The logger to use when tracking errors.</param>
    /// <returns>A list of tokens.</returns>
    public static async Task<TokenizedAssmebly> TokenizeAsync(TextReader reader, string? filename = null, ILogger? logger = null)
    {
        // Create tokenizer
        Tokenizer tokenizer = new(filename, logger);

        // Parse line by line from stream
        while (true)
        {
            var line = await reader.ReadLineAsync();
            if (line is null)
                break;

            tokenizer.ParseLine(line);
        }

        return new TokenizedAssmebly(tokenizer.TokenLines);
    }

    /// <summary>
    /// Tokenizes a single line of assembly code.
    /// </summary>
    public static AssemblyLine TokenizeLine(string line, string? filename = null, TokenizerMode mode = TokenizerMode.Assembly)
    {
        Tokenizer tokenizer = new(filename, mode: mode);

        if (line.Contains('\n'))
            ThrowHelper.ThrowArgumentException("Single line tokenizer cannot contain a new line.");

        // This is a debug tool for tokenizing only an expression
        if (mode is TokenizerMode.BehaviorExpression or TokenizerMode.Expression)
        {
            tokenizer._state = TokenizerState.ArgBegin;
        }

        tokenizer.ParseLine(line);
        return tokenizer.TokenLines[0];
    }

    private bool ParseLine(string line)
    {
        Tokens = [];

        bool status = true;
        line += '\n';
        foreach (char c in line)
        {
            // This line is invalid and cannot be tokenized.
            // Regardless, we'll add what was parsable and keep going to find any further errors
            if (!ParseNextChar(c))
                status = false;

            _column++;
        }

        Line++;
        _column = 0;
        TokenLines.Add(new([..Tokens]));
        return status;
    }

    private bool ParseNextChar(char c)
    {
        return _state switch
        {
            // Beginning of a new argument or line
            TokenizerState.LineBegin => ParseFromBegin(c, true),
            TokenizerState.ArgBegin => ParseFromBegin(c, false),

            // Instructions, labels, macros, etc. at beginning of line
            TokenizerState.NewLineText => ParseNewLineText(c, false),
            TokenizerState.NewLineTextWait => ParseNewLineText(c, true),

            // Registers
            TokenizerState.Register => ParseFromRegister(c),

            // Immediate values
            TokenizerState.Immediate => ParseFromImmediate(c),
            TokenizerState.SpecialImmediate => ParseFromImmediate(c, true),
            TokenizerState.BinaryImmediate or TokenizerState.OctImmediate or
            TokenizerState.HexImmediate or TokenizerState.BadImmediate => ParseFromImmediate(c, state: _state),

            // String and character literals
            TokenizerState.Character => ParseFromString(c, true),
            TokenizerState.String => ParseFromString(c, false),

            TokenizerState.Complete => ParseFromComplete(c),

            // Directives and references
            TokenizerState.Directive => ParseFromDirective(c),
            TokenizerState.Reference => ParseFromReference(c),

            // Comments and whitespace
            TokenizerState.Comment => ParseFromComment(c),
            TokenizerState.Whitespace or TokenizerState.NewLineWhitespace => ParseFromWhitespace(c, state: _state),
            _ => ThrowHelper.ThrowArgumentOutOfRangeException<bool>(nameof(_state)),
        };
    }

    private bool ParseFromBegin(char c, bool newLine)
    {
        // Handle new lines
        if (c == '\n')
        {
            _state = TokenizerState.LineBegin;
            return true;
        }

        // We track whitespace between tokens
        // This will be discarded, unless in behavior mode
        if (char.IsWhiteSpace(c))
        {
            return HandleCharacter(c, TokenType.Whitespace, newLine ? TokenizerState.NewLineWhitespace : TokenizerState.Whitespace);
        }

        if (newLine && char.IsLetterOrDigit(c))
        {
            // Digits aren't actually valid here, but we're gonna defer that error for now
            // to get a better error message down the line.
            return HandleCharacter(c, TokenType.Reference, TokenizerState.NewLineText);
        }
        else
        {
            // Begin a special immediate, 
            if (c is '0')
            {
                return HandleCharacter(c, TokenType.Immediate, TokenizerState.SpecialImmediate);
            }

            if (char.IsDigit(c))
            {
                return HandleCharacter(c, TokenType.Immediate, TokenizerState.Immediate);
            }

            if (char.IsLetter(c) || c is '_')
            {
                return HandleCharacter(c, TokenType.Reference, TokenizerState.Reference);
            }
        }

        return c switch
        {
            // Operators
            '+' or '-' or '*' or '/' or '%' or
            '|' or '&' or '^' or '~' => HandleCharacter(c, TokenType.Operator),

            // Alt-Operators
            '(' => HandleCharacter(c, TokenType.OpenParenthesis),
            ')' => HandleCharacter(c, TokenType.CloseParenthesis),
            '[' => HandleCharacter(c, TokenType.OpenBracket),
            ']' => HandleCharacter(c, TokenType.CloseBracket),
            ',' => HandleCharacter(c, TokenType.Comma),
            '=' => HandleCharacter(c, TokenType.Assign),

            '.' => newLine && HandleCharacter(c, TokenType.Directive, TokenizerState.Directive),
            '$' => HandleCharacter(c, TokenType.Register, TokenizerState.Register),
            '\'' => HandleCharacter(c, newState: TokenizerState.Character),
            '"' => HandleCharacter(c, newState: TokenizerState.String),

            // Comments
            '#' => HandleCharacter(c, TokenType.Comment, TokenizerState.Comment),

            // Behavioral operators
            '!' => _mode is TokenizerMode.BehaviorExpression && HandleCharacter(c, TokenType.Operator),
            '<' => _mode is TokenizerMode.BehaviorExpression && HandleCharacter(c, TokenType.Operator),
            '>' => _mode is TokenizerMode.BehaviorExpression && HandleCharacter(c, TokenType.Operator),
            _ => false,
        };
    }

    private bool ParseNewLineText(char c, bool wait)
    {
        // Enter waiting mode
        if (char.IsWhiteSpace(c) && c is not '\n')
        {
            _state = TokenizerState.NewLineTextWait;
            return true;
        }

        // Text is a label declaration.
        if (c is ':')
        {
            return HandleCharacter(c, TokenType.LabelDeclaration, TokenizerState.LineBegin);
        }

        // Text is a marco declaration.
        if (c == '=')
        {
            _tokenType = TokenType.MacroDefinition;
            return CompleteAndContinue(c);
        }

        // We're in waiting mode, we can't keep adding to the token.
        // It must be an instruction.
        if (wait || c is '\n')
        {
            _tokenType = TokenType.Instruction;
            return CompleteAndContinue(c);
        }

        // There's a lot of characters that aren't valid here,
        // but we're gonna let them be part of the token and just drop 
        // more understandable error later.
        return HandleCharacter(c, null, TokenizerState.NewLineText);
    }

    private bool ParseFromRegister(char c)
    {
        if (char.IsLetterOrDigit(c))
        {
            return HandleCharacter(c, TokenType.Register, TokenizerState.Register);
        }

        return CompleteAndContinue(c);
    }

    private bool ParseFromImmediate(char c, bool special = false, TokenizerState state = TokenizerState.Immediate)
    {
        // We're in special state so the last value was a '0'.
        // Now if we see one of these characters we're handling a non-base 10 immediate.
        if (special && c is 'b' or 'o' or 'x')
        {
            var newState = c switch
            {
                'b' => TokenizerState.BinaryImmediate,
                'o' => TokenizerState.OctImmediate,
                'x' => TokenizerState.HexImmediate,
                _ => ThrowHelper.ThrowArgumentOutOfRangeException<TokenizerState>(nameof(c)),
            };

            return HandleCharacter(c, null, newState);
        }

        // Which characters are valid depends on the immediate state
        Func<char, bool> check = state switch
        {
            TokenizerState.BinaryImmediate => c => c is '0' or '1',
            TokenizerState.OctImmediate => c => char.IsBetween(c, '0', '7'),
            TokenizerState.HexImmediate => c => char.IsDigit(c) || char.IsBetween(char.ToLower(c), 'a', 'f'),
            TokenizerState.BadImmediate => _ => false,
            _ => char.IsDigit,
        };

        // Validate character for immediate state
        if (check(c))
        {
            return HandleCharacter(c, TokenType.Immediate, state);
        }

        // Track bad immediate creation, but log nothing.
        // This will be logged later during parsing.
        if (char.IsLetterOrDigit(c))
        {
            return HandleCharacter(c, TokenType.Immediate, TokenizerState.BadImmediate);
        }

        // Complete the current token and process the current character as the
        // start of the next.
        return CompleteAndContinue(c);
    }

    private bool ParseFromString(char c, bool isChar)
    {
        if (c is '\n')
        {
            var expected = isChar ? "Characters" : "Strings";
            _logger?.Log(Severity.Error, LogId.MultiLineString, Line, $"{expected}CantWrapLines.");
            return false;
        }

        // Only parsing a single character.
        // This is similar behavior to string parsing, so it's combined as a parameter
        // on the string parsing function.
        if (isChar)
        {
            // We notice the empty literal here, but defer reporting until the assembly step
            if (_cache.Length is 1 && c is '\'')
            {
                //_logger?.Log(Severity.Error, LogId.InvalidCharLiteral, Line, "EmptyCharacterLiteral");
                return HandleCharacter(c, TokenType.Immediate);
            }

            if (_cache.Length is >= 2 && c is '\'')
            {
                return HandleCharacter(c, TokenType.Immediate);
            }
        }

        if (!isChar && c is '"')
        {
            return HandleCharacter(c, TokenType.String);
        }

        return HandleCharacter(c, newState: isChar ? TokenizerState.Character : TokenizerState.String);
    }

    private bool ParseFromComplete(char c) => CompleteAndContinue(c);

    private bool ParseFromDirective(char c)
    {
        if (char.IsLetterOrDigit(c))
        {
            return HandleCharacter(c, TokenType.Directive, TokenizerState.Directive);
        }

        return CompleteAndContinue(c);
    }

    private bool ParseFromReference(char c)
    {
        if (char.IsLetterOrDigit(c) || c is '_')
        {
            return HandleCharacter(c, TokenType.Reference, TokenizerState.Reference);
        }

        return CompleteAndContinue(c);
    }

    private bool ParseFromComment(char c)
    {
        if (c is '\n')
        {
            return CompleteCacheToken(TokenizerState.LineBegin);
        }

        return HandleCharacter(c, TokenType.Comment, TokenizerState.Comment);
    }

    private bool ParseFromWhitespace(char c, TokenizerState state = TokenizerState.Whitespace)
    {
        if (char.IsWhiteSpace(c))
            return HandleCharacter(c, TokenType.Whitespace, state);

        var newState = state switch
        {
            TokenizerState.NewLineWhitespace => TokenizerState.LineBegin,
            _ => TokenizerState.ArgBegin,
        };
        return CompleteAndContinue(c, newState);
    }

    private bool HandleCharacter(char c, TokenType? type = null, TokenizerState newState = TokenizerState.Complete)
    {
        _cache += c;
        _tokenType = type;

        if (newState is TokenizerState.ArgBegin or TokenizerState.LineBegin)
        {
            if (type is null)
                ThrowHelper.ThrowArgumentException(nameof(type));

            return CompleteCacheToken(newState);
        }

        _state = newState;
        return true;
    }

    private bool CompleteAndContinue(char c, TokenizerState newState = TokenizerState.ArgBegin)
    {
        var status = CompleteCacheToken(newState);
        if (!status)
            return false;

        return ParseNextChar(c);
    }

    private bool CompleteCacheToken(TokenizerState newState = TokenizerState.ArgBegin)
    {
        // Token must be created if cache is completed and not a comment
        if (_tokenType is null)
        {
            string message = _state switch
            {
                TokenizerState.Reference => $"IncompleteReference",
                TokenizerState.Immediate => $"IncompleteImmediate",
                TokenizerState.Register => $"IncompleteRegister",
                _ => $"IncompleteToken",
            };

            _logger?.Log(Severity.Error, LogId.TokenizerError, Line, message, _cache);
            return false;
        }

        // Add the token if not whitespace. Unless in behavior mode, then add the whitespace token
        if (_tokenType is not TokenType.Whitespace and not TokenType.Comment || _mode is TokenizerMode.BehaviorExpression or TokenizerMode.IDE)
        {
            // Create the token and add to list
            Token token = new(_cache, _filename, Line, _cacheColumn, _tokenType.Value);
            Tokens?.Add(token);
        }

        // Reset cache
        _cache = string.Empty;
        _cacheColumn = _column;
        _tokenType = null;
        _state = newState;
        return true;
    }
}
