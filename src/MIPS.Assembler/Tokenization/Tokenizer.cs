// Adam Dernis 2024

using CommunityToolkit.Diagnostics;
using MIPS.Assembler.Logging;
using MIPS.Assembler.Logging.Enum;
using MIPS.Assembler.Tokenization.Enums;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace MIPS.Assembler.Tokenization;

/// <summary>
/// A class for tokenizing an assembly file.
/// </summary>
public class Tokenizer
{
    private readonly ILogger? _logger;

    private TokenizerState _state;
    private string _cache;
    private TokenType? _tokenType;

    private readonly string? _filename;
    private int _line;
    private int _column;

    /// <summary>
    /// Initializes a new instance of the <see cref="Tokenizer"/> class.
    /// </summary>
    private Tokenizer(string? filename, ILogger? logger = null)
    {
        _logger = logger;

        TokenLines = [];
        Tokens = [];
        _state = TokenizerState.LineBegin;
        _cache = string.Empty;
        _tokenType = null;

        _filename = filename;
        _line = 1;
        _column = 0;
    }

    private List<AssemblyLine> TokenLines { get; }

    private List<Token> Tokens { get; set; }

    /// <summary>
    /// Tokenizes a stream of assembly code.
    /// </summary>
    /// <param name="stream">The stream of code.</param>
    /// <param name="filename">The filename of the stream.</param>
    /// <param name="logger">The logger to use when tracking errors.</param>
    /// <returns>A list of tokens.</returns>
    public static async Task<TokenizedAssmebly> TokenizeAsync(Stream stream, string? filename = null, ILogger? logger = null)
    {
        Tokenizer tokenizer = new(filename, logger);

        using var reader = new StreamReader(stream);
        while (!reader.EndOfStream)
        {
            var line = await reader.ReadLineAsync();
            if (line is null)
                ThrowHelper.ThrowArgumentNullException(nameof(line));

            tokenizer.ParseLine(line);
        }

        return new TokenizedAssmebly(tokenizer.TokenLines);
    }

    internal static AssemblyLine TokenizeLine(string line, string? filename = null, bool expression = false)
    {
        Tokenizer tokenizer = new(filename);

        if (line.Contains('\n'))
            ThrowHelper.ThrowArgumentException("Single line tokenizer cannot contain a new line.");

        // This is a debug tool.
        // We just want to tokenize an expression,
        // so we don't want directives, instructions, or labels
        if (expression)
        {
            tokenizer._state = TokenizerState.Begin;
        }

        tokenizer.ParseLine(line);
        return tokenizer.TokenLines[0];
    }

    private bool ParseLine(string line)
    {
        Tokens = [];

        line += '\n';
        foreach (char c in line)
        {
            bool status = ParseNextChar(c, line);

            // This line is trashed, but we'll add it and keep going to find any further errors
            if (!status)
            {
                TokenLines.Add(new([..Tokens]));
                return false;
            }

            _column++;
        }

        _line++;
        _column = 0;
        TokenLines.Add(new([..Tokens]));
        return true;
    }

    private bool ParseNextChar(char c, string line)
    {
        return _state switch
        {
            TokenizerState.Begin => ParseFromBegin(c, line, false),
            TokenizerState.LineBegin => ParseFromBegin(c, line, true),
            TokenizerState.NewLineText => ParseNewLineText(c, line, false),
            TokenizerState.NewLineTextWait => ParseNewLineText(c, line, true),
            TokenizerState.Register => ParseFromRegister(c, line),
            TokenizerState.Immediate => ParseFromImmediate(c, line),
            TokenizerState.SpecialImmediate => ParseFromImmediate(c, line, true),
            TokenizerState.HexImmediate => ParseFromImmediate(c, line, hex:true),
            TokenizerState.Character => ParseFromString(c, line, true),
            TokenizerState.String => ParseFromString(c, line, false),
            TokenizerState.Directive => ParseFromDirective(c, line),
            TokenizerState.Reference => ParseFromReference(c, line),
            TokenizerState.Comment => ParseFromComment(c, line),
            _ => ThrowHelper.ThrowArgumentOutOfRangeException<bool>(nameof(_state)),
        };
    }

    private bool ParseFromBegin(char c, string line, bool newLine)
    {
        // Handle new lines
        if (c == '\n')
        {
            _state = TokenizerState.LineBegin;
            return true;
        }

        // Ignore whitespace between tokens.
        if (char.IsWhiteSpace(c))
            return true;

        if (newLine && char.IsLetterOrDigit(c))
        {
            // Digits aren't actually valid here, but we're gonna defer that error for now
            // to get a better error message down the line.
            return HandleCharacter(c, TokenType.Reference, TokenizerState.NewLineText);
        }
        else
        {
            if (c is '0')
            {
                return HandleCharacter(c, TokenType.Immediate, TokenizerState.SpecialImmediate);
            }

            if (char.IsDigit(c))
            {
                return HandleCharacter(c, TokenType.Immediate, TokenizerState.Immediate);
            }

            if (char.IsLetter(c))
            {
                return HandleCharacter(c, TokenType.Reference, TokenizerState.Reference);
            }
        }

        return c switch
        {
            '+' or '-' or '*' or '/' or '%' or
            '|' or '&' or '^' => HandleCharacter(c, TokenType.Operator),

            '.' => newLine ? HandleCharacter(c, newState: TokenizerState.Directive) : false,
            '$' => HandleCharacter(c, newState: TokenizerState.Register),
            '\'' => HandleCharacter(c, newState: TokenizerState.Character),
            '"' => HandleCharacter(c, newState: TokenizerState.String),

            '(' => HandleCharacter(c, TokenType.OpenParenthesis),
            ')' => HandleCharacter(c, TokenType.CloseParenthesis),
            ',' => HandleCharacter(c, TokenType.Comma),
            '=' => HandleCharacter(c, TokenType.Assign),
            '#' => HandleCharacter(c, null, TokenizerState.Comment),
            _ => false,
        };
    }

    private bool ParseNewLineText(char c, string line, bool wait)
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
            return CompleteAndContinue(c, line);
        }

        // We're in waiting mode, we can't keep adding to the token.
        // It must be an instruction.
        if (wait || c is '\n')
        {
            _tokenType = TokenType.Instruction;
            return CompleteAndContinue(c, line);
        }

        // There's a lot of characters that aren't valid here,
        // but we're gonna let them be part of the token and just drop 
        // more understandable error later.
        return HandleCharacter(c, null, TokenizerState.NewLineText);
    }

    private bool ParseFromRegister(char c, string line)
    {
        if (char.IsLetterOrDigit(c))
        {
            return HandleCharacter(c, TokenType.Register, TokenizerState.Register);
        }

        return CompleteAndContinue(c, line);
    }

    private bool ParseFromImmediate(char c, string line, bool special = false, bool hex = false)
    {
        // We're in special state so the last value was a '0'.
        // Now if we see one of these characters we're handling a non-base 10 immediate.
        if (special && c is 'b' or 'o' or 'x')
        {
            return HandleCharacter(c, null, c is 'x' ? TokenizerState.HexImmediate : TokenizerState.Immediate);
        }

        var state = hex ? TokenizerState.HexImmediate : TokenizerState.Immediate;

        if (char.IsDigit(c))
        {
            return HandleCharacter(c, TokenType.Immediate, state);
        }

        if (hex && (char.IsBetween(c, 'a', 'f') || char.IsBetween(c, 'A', 'F')))
        {
            return HandleCharacter(c, TokenType.Immediate, state);
        }

        return CompleteAndContinue(c, line);
    }

    private bool ParseFromString(char c, string line, bool isChar)
    {
        if (c is '\n')
        {
            _logger?.Log(Severity.Error, LogId.MultiLineString, $"Strings may not wrap between lines.", _line);
            return false;
        }

        if (isChar)
        {
            if (_cache.Length is 1 && c is '\'')
            {
                _logger?.Log(Severity.Error, LogId.InvalidCharLiteral, $"Empty character literal.", _line);
                return false;
            }

            // TODO: Allow escaped characters.
            if (_cache.Length is >= 2 && c is not '\'')
                return false;

            if (_cache.Length is 2 && c is '\'')
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

    private bool ParseFromDirective(char c, string line)
    {
        if (char.IsLetterOrDigit(c))
        {
            return HandleCharacter(c, TokenType.Directive, TokenizerState.Directive);
        }

        return CompleteAndContinue(c, line);
    }

    private bool ParseFromReference(char c, string line)
    {
        if (char.IsLetterOrDigit(c))
        {
            return HandleCharacter(c, TokenType.Reference, TokenizerState.Reference);
        }

        return CompleteAndContinue(c, line);
    }

    private bool ParseFromComment(char c, string line)
    {
        if (c is '\n')
        {
            return CompleteCacheToken(TokenizerState.LineBegin);
        }

        return true;
    }

    private bool HandleCharacter(char c, TokenType? type = null, TokenizerState newState = TokenizerState.Begin)
    {
        _cache += c;
        _tokenType = type;

        if (newState is TokenizerState.Begin or TokenizerState.LineBegin)
        {
            if (type is null)
                ThrowHelper.ThrowArgumentException(nameof(type));

            return CompleteCacheToken(newState);
        }

        _state = newState;
        return true;
    }

    private bool CompleteAndContinue(char c, string line)
    {
        var status = CompleteCacheToken();
        if (!status)
            return false;

        return ParseNextChar(c, line);
    }

    private bool CompleteCacheToken(TokenizerState newState = TokenizerState.Begin)
    {
        // This method can be used to denote the completion of a comment block
        if (_state is not TokenizerState.Comment)
        {
            // Token must be created if cache is completed and not a comment
            if (_tokenType is null)
            {
                string message = _state switch
                {
                    TokenizerState.Reference => $"Incomplete symbol reference \"{_cache}\".",
                    TokenizerState.Immediate => $"Incomplete immediate value \"{_cache}\".",
                    TokenizerState.Register => $"Incomplete register name \"{_cache}\".",
                    _ => $"Incomplete token \"{_cache}\"",
                };

                _logger?.Log(Severity.Error, LogId.TokenizerError, message, _line);
                return false;
            }

            // Create the token and add to list
            Token token = new(_cache, _filename, _line, _column, _tokenType.Value);
            Tokens?.Add(token);
        }

        // Reset cache
        _cache = string.Empty;
        _tokenType = null;
        _state = newState;
        return true;
    }
}
