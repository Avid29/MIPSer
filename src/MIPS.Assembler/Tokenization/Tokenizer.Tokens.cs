// Avishai Dernis 2025

using MIPS.Assembler.Tokenization.Models;
using MIPS.Assembler.Tokenization.Models.Enums;
using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.Marshalling;
using System.Text;

namespace MIPS.Assembler.Tokenization;

public partial class Tokenizer
{
    private bool ReTokenizeLine(List<Token> raw, out List<Token> classified)
    {
        // Prepare for token retokenization
        _state = TokenizerState.LineBegin;
        classified = [];

        bool start = true;
        if (_mode is TokenizerMode.BehaviorExpression or TokenizerMode.Expression)
            start = false;

        // Reclassify each token
        var span = CollectionsMarshal.AsSpan(raw);
        while (!span.IsEmpty)
        {
            var newToken = ReTokenizeSpan(start, span, out var advance);

            // TODO: Handle invalid state
            if (newToken is null)
                return false;

            // Track if the new token is meaningful
            bool meaningful = newToken.Type is not TokenType.Comment and not TokenType.Whitespace;

            if (meaningful || _mode is TokenizerMode.IDE or TokenizerMode.BehaviorExpression)
                classified.Add(newToken);

            // If a meaningful token was appended,
            // we are no longer at the start
            start = start && !meaningful;

            // Advance the appropriate number of tokens
            span = span[advance..];
        }

        return true;
    }

    private static Token? ReTokenizeSpan(bool start, ReadOnlySpan<Token> tokens, out int advance)
    {
        advance = 1;

        var current = tokens[0];

        // Classify operators or literals
        var @new = ReTokenizeAsOperator(current);
        @new ??= ReTokenizeLiterals(current);
        if (@new is not null)
            return @new;

        // Classify meaningless tokens (comments and whitespace)
        var meaningless = ReTokenizeMeaningless(current);
        if (meaningless is not null)
            return meaningless;

        // Classify numerical tokens
        if (current.IsNumeric())
            return ReClassify(current, TokenType.Immediate);

        var peek = PeekNext(tokens);

        // Registers
        if (!start && current.Source is "$" && peek.IsIdentifier())
        {
            advance = 2;
            return Merge(TokenType.Register, current, peek);
        }

        // Handle macros
        if (start && current.IsIdentifier() && peek?.Source is "=")
        {
            return ReClassify(current, TokenType.MacroDeclaration);
        }

        // Handle directives
        // TODO: Check if this is good in all conditions
        if (start && current.Source is ".")
        {
            advance = 2;    // null check peek?
            return Merge(TokenType.Directive, current, peek);
        }

        // Handle labels
        if (start && peek?.Source is ":")
        {
            advance = 2;
            return Merge(TokenType.LabelDeclaration, current, peek);
        }

        // Handle instructions
        if (start && current.IsIdentifier())
        {
            var merged = ReClassify(current, TokenType.Instruction);

            // Check for formatted instructions
            var span = tokens;
            do
            {
                var peek1 = PeekNext(span);
                var peek2 = PeekNext(span[1..]);
                if (peek2 is not null)
                    span = span[2..];

                if (peek?.Source is not "." || !peek2.IsIdentifier())
                    break;

                merged = Merge(TokenType.Instruction, merged, peek, peek2);
                advance += 2;
            } while (true);

            return merged;
        }

        if (current.IsIdentifier())
        {
            return ReClassify(current, TokenType.Reference);
        }

        // Keep unknown values as unknown
        return current;
    }

    private static Token? ReTokenizeAsOperator(Token token)
    {
        return token.Source switch
        {
            "(" => ReClassify(token, TokenType.OpenParenthesis),
            ")" => ReClassify(token, TokenType.CloseParenthesis),
            "[" => ReClassify(token, TokenType.OpenBracket),
            "]" => ReClassify(token, TokenType.CloseBracket),
            "," => ReClassify(token, TokenType.Comma),

            "+" or "-" or "*" or "/" or "%" or
            "|" or "&" or "^" or "~" => ReClassify(token, TokenType.Operator),

            // Behavior mode only?
            "!" or "<" or ">" => ReClassify(token, TokenType.Operator),
            _ => null,
        };
    }

    private static Token? ReTokenizeLiterals(Token token)
    {
        return token.Type switch
        {
            TokenType.String => token,
            TokenType.Char => ReClassify(token, TokenType.Immediate),
            _ => null
        };
    }

    private static Token? ReTokenizeMeaningless(Token token)
    {
        if (token.Type is not TokenType.Comment and not TokenType.Whitespace)
            return null;

        return token;
    }

    private static Token Merge(TokenType type, Token @base, params Token?[] tokens)
    {
        // Generate new text
        var source = new StringBuilder(@base.Source);
        foreach (var token in tokens)
        {
            source.Append(token?.Source);
        }

        return new Token($"{source}")
        {
            Type = type,
            Filename = @base.Filename,
            Location = @base.Location,
        };
    }

    private static Token? PeekNext(ReadOnlySpan<Token> tokens)
    {
        Token? token;
        do
        {
            // We've hit the end. None found
            if (tokens.Length is <= 1)
                return null;

            // Slice and grab the next token
            tokens = tokens[1..];

            token = tokens[0];

        } while (token.Type is TokenType.Whitespace);

        return token;
    }

    private static Token ReClassify(Token original, TokenType type)
    {
        return new Token(original.Source)
        {
            Filename = original.Filename,
            Location = original.Location,
            Type = type
        };
    }
}
