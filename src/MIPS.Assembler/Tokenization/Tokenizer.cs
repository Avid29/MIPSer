// Avishai Dernis 2025

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
        // Create tokenizer
        Tokenizer tokenizer = new(filename, logger);

        // Parse line by line from stream
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

        // This is a debug tool for tokenizing only an expression
        if (expression)
        {
            tokenizer._state = TokenizerState.ArgBegin;
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
            bool status = ParseNextChar(c);

            // This line is invalid and cannot be tokenized.
            // Regardless, we'll add what was parsable and keep going to find any further errors
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

    private bool ParseNextChar(char c)
    {
        return _state switch
        {
            TokenizerState.ArgBegin => ParseFromBegin(c, false),
            TokenizerState.LineBegin => ParseFromBegin(c, true),
            TokenizerState.NewLineText => ParseNewLineText(c, false),
            TokenizerState.NewLineTextWait => ParseNewLineText(c, true),
            TokenizerState.Register => ParseFromRegister(c),
            TokenizerState.Immediate => ParseFromImmediate(c),
            TokenizerState.SpecialImmediate => ParseFromImmediate(c, true),
            TokenizerState.HexImmediate => ParseFromImmediate(c, hex:true),
            TokenizerState.Character => ParseFromString(c, true),
            TokenizerState.String => ParseFromString(c, false),
            TokenizerState.Directive => ParseFromDirective(c),
            TokenizerState.Reference => ParseFromReference(c),
            TokenizerState.Comment => ParseFromComment(c),
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
            // Begin a special immediate, 
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

            '.' => newLine && HandleCharacter(c, newState: TokenizerState.Directive),
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

    private bool ParseFromImmediate(char c, bool special = false, bool hex = false)
    {
        // We're in special state so the last value was a '0'.
        // Now if we see one of these characters we're handling a non-base 10 immediate.
        if (special && c is 'b' or 'o' or 'x')
        {
            return HandleCharacter(c, null, c is 'x' ? TokenizerState.HexImmediate : TokenizerState.Immediate);
        }

        var state = hex ? TokenizerState.HexImmediate : TokenizerState.Immediate;

        // Continue parsing the current token if digit
        if (char.IsDigit(c))
        {
            return HandleCharacter(c, TokenType.Immediate, state);
        }

        // If the immediate is a hexidemical the characters 'A' through 'F',
        // in upper or lowercase, continue parsing the current token.
        if (hex && (char.IsBetween(char.ToLower(c), 'a', 'f')))
        {
            return HandleCharacter(c, TokenType.Immediate, state);
        }

        // Complete the current token and process the current character as the
        // start of the next.
        return CompleteAndContinue(c);
    }

    private bool ParseFromString(char c, bool isChar)
    {
        if (c is '\n')
        {
            _logger?.Log(Severity.Error, LogId.MultiLineString, $"Strings may not wrap between lines.", _line);
            return false;
        }

        // Only parsing a single character.
        // This is similar behavior to string parsing, so it's combined as a parameter
        // on the string parsing function.
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
        if (char.IsLetterOrDigit(c))
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

        return true;
    }

    private bool HandleCharacter(char c, TokenType? type = null, TokenizerState newState = TokenizerState.ArgBegin)
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

    private bool CompleteAndContinue(char c)
    {
        var status = CompleteCacheToken();
        if (!status)
            return false;

        return ParseNextChar(c);
    }

    private bool CompleteCacheToken(TokenizerState newState = TokenizerState.ArgBegin)
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
