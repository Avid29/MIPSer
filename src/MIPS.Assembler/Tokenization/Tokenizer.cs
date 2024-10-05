// Adam Dernis 2023

using CommunityToolkit.Diagnostics;
using MIPS.Assembler.Tokenization.Enums;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace MIPS.Assembler.Tokenization;

/// <summary>
/// A class for tokenizing an assembly file.
/// </summary>
public class Tokenizer
{
    private TokenizerState _state;
    private string _cache;
    private TokenType? _tokenType;

    private string? _filename;
    private int _line;
    private int _column;

    /// <summary>
    /// Initializes a new instance of the <see cref="Tokenizer"/> class.
    /// </summary>
    private Tokenizer(string? filename)
    {
        Tokens = [];
        _state = TokenizerState.LineBegin;
        _cache = string.Empty;
        _tokenType = null;

        _filename = filename;
        _line = 1;
        _column = 0;
    }

    private List<Token> Tokens { get; }

    /// <summary>
    /// Tokenizes a stream of assembly code.
    /// </summary>
    /// <param name="stream">The stream of code.</param>
    /// <param name="filename">The filename of the stream.</param>
    /// <returns>A list of tokens.</returns>
    public static async Task<IReadOnlyList<Token>> Tokenize(Stream stream, string? filename = null)
    {
        Tokenizer tokenizer = new (filename);

        using var reader = new StreamReader(stream);
        while (!reader.EndOfStream)
        {
            var line = await reader.ReadLineAsync();
            if (line is null)
                ThrowHelper.ThrowArgumentNullException(nameof(line));

            tokenizer.ParseLine(line + '\n');
        }

        return tokenizer.Tokens;
    }

    private bool ParseLine(string line)
    {
        foreach (char c in line)
        {
            bool status = ParseNextChar(c, line);
            if (!status)
                return false;

            _column++;
        }

        _line++;
        _column = 0;
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
            TokenizerState.Immediate => ParseFromImmediate(c, line, false),
            TokenizerState.SpecialImmediate => ParseFromImmediate(c, line, true),
            TokenizerState.Directive => ParseFromDirective(c, line),
            TokenizerState.Reference => ParseFromReference(c, line),
            TokenizerState.Comment => ParseFromComment(c, line),
            _ => false,
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
            if (newLine)
            {
                return HandleCharacter(c, null, TokenizerState.NewLineText);
            }
            else
            {
                return HandleCharacter(c, TokenType.Reference, TokenizerState.Reference);
            }
        }

        return c switch
        {
            '+' or '-' or '*' or '/' or '%' or
            '|' or '&' or '^' => HandleCharacter(c, TokenType.Operator),

            '.' => newLine ? HandleCharacter(c, newState:TokenizerState.Directive) : false,
            '$' => HandleCharacter(c, newState:TokenizerState.Register),

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
        if (wait)
        {
            _tokenType = TokenType.Instruction;
            return CompleteAndContinue(c, line);
        }

        // A label cannot begin with a number.
        if (char.IsDigit(c) && _cache.Length == 0)
            return false;

        if (char.IsLetterOrDigit(c))
        {
            return HandleCharacter(c, null, TokenizerState.NewLineText);
        }

        return false; // Nothing can come before a line's label
    }

    private bool ParseFromRegister(char c, string line)
    {
        if (char.IsLetterOrDigit(c))
        {
            return HandleCharacter(c, TokenType.Register, TokenizerState.Register);
        }

        return CompleteAndContinue(c, line);
    }

    private bool ParseFromImmediate(char c, string line, bool special)
    {
        // We're in special state so the last value was a '0'.
        // Now if we see one of these characters we're handling a non-base 10 immediate.
        if (special && c is 'b' or 'o' or 'x')
        {
            return HandleCharacter(c, null, TokenizerState.Immediate);
        }

        if (char.IsDigit(c))
        {
            return HandleCharacter(c, TokenType.Immediate, TokenizerState.Immediate);
        }

        return CompleteAndContinue(c, line);
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
                return false;
            
            // Create the token and add to list
            Token token = new(_cache, _filename, _line, _tokenType.Value);
            Tokens?.Add(token);
        }

        // Reset cache
        _cache = string.Empty;
        _tokenType = null;
        _state = newState;
        return true;
    }
}
