// Avishai Dernis 2025

using CommunityToolkit.Diagnostics;
using MIPS.Assembler.Tokenization.Models;
using MIPS.Assembler.Tokenization.Models.Enums;

namespace MIPS.Assembler.Tokenization;

public partial class Tokenizer
{
    private bool PreTokenizeLine(string line)
    {
        // Prepare for string tokenization
        _state = TokenizerState.TokenBegin;
        LineTokens = [];

        line += '\n';

        bool status = true;
        foreach (char c in line)
        {
            if (!ParseNextChar(c))
                status = false;

            _location.Index++;
            _location.Column++;
        }

        CompleteCacheToken();

        _location.Line++;
        _location.Column = 1;
        _cacheLocation = _location;
        return status;
    }

    private bool ParseNextChar(char c)
    {
        return _state switch
        {
            // LineBegin
            TokenizerState.TokenBegin when c is '#' => AppendCharacter(c, TokenizerState.Comment),                      // Begin a comment
            TokenizerState.TokenBegin when c is '"' => AppendCharacter(c, TokenizerState.StringLiteral),                // Begin a string
            TokenizerState.TokenBegin when c is '\'' => AppendCharacter(c, TokenizerState.CharLiteral),                 // Begin a char
            TokenizerState.TokenBegin when char.IsWhiteSpace(c) => AppendCharacter(c, TokenizerState.Whitespace),     // Begin whitespace
            TokenizerState.TokenBegin when char.IsLetterOrDigit(c) || c is '_'                                          // Begin an identifier
                => AppendCharacter(c, TokenizerState.TokenBody),                                                        // or numerical
            TokenizerState.TokenBegin => AppendAndComplete(c, TokenizerState.TokenBegin),                               // Begin an unknown token

            // Token Body
            TokenizerState.TokenBody when char.IsLetterOrDigit(c) || c is '_' => AppendCharacter(c),
            TokenizerState.TokenBody => CompleteAndContinue(c),

            // String Literal
            TokenizerState.StringLiteral when c is '"' => AppendAndComplete(c, TokenizerState.TokenBegin),
            TokenizerState.StringLiteral => AppendCharacter(c),

            // Char Literal
            TokenizerState.CharLiteral when c is '\'' => AppendAndComplete(c, TokenizerState.TokenBegin),
            TokenizerState.CharLiteral => AppendCharacter(c),

            // Comments
            TokenizerState.Comment => AppendCharacter(c),

            // Whitespace
            TokenizerState.Whitespace when char.IsWhiteSpace(c) => AppendCharacter(c),
            TokenizerState.Whitespace => CompleteAndContinue(c),

            _ => ThrowHelper.ThrowArgumentOutOfRangeException<bool>(nameof(_state)),
        };
    }

    private bool AppendCharacter(char c, TokenizerState? newState = null)
    {
        // Append the character to the cache
        _cache.Append(c);
        
        // Update the state if needed
        if (newState is not null)
            _state = newState.Value;

        return true;
    }

    private bool AppendAndComplete(char c, TokenizerState newState)
    {
        // Append the character and complete the token
        _cache.Append(c);
        var status = CompleteCacheToken();
        _state = newState;
        return status;
    }

    private bool CompleteAndContinue(char c)
    {
        // Complete the token
        var status = CompleteCacheToken();
        if (!status)
            return false;

        // Begin the next using this character
        _state = TokenizerState.TokenBegin;
        return ParseNextChar(c);
    }

    private bool CompleteCacheToken()
    {
        // Determine what type of token to create from the state
        var tokenType = _state switch
        {
            TokenizerState.TokenBegin or
            TokenizerState.TokenBody => TokenType.Unknown,
            TokenizerState.CharLiteral => TokenType.Char,
            TokenizerState.StringLiteral => TokenType.String,
            TokenizerState.Comment => TokenType.Comment,
            TokenizerState.Whitespace => TokenType.Whitespace,
            _ => ThrowHelper.ThrowArgumentOutOfRangeException<TokenType>(nameof(_state)),
        };

        // Create the token and add to list
        var token = new Token($"{_cache}")
        {
            Type = tokenType,
            Filename = _filename,
            Location = _cacheLocation,
        };
        LineTokens?.Add(token);

        // Reset cache
        _cache.Clear();
        _cacheLocation = _location;
        return true;
    }
}
