// Avishai Dernis 2025

using CommunityToolkit.Diagnostics;
using MIPS.Assembler.Logging;
using MIPS.Assembler.Logging.Enum;
using MIPS.Assembler.Models;
using MIPS.Assembler.Parsers.Expressions;
using MIPS.Assembler.Parsers.Expressions.Abstract;
using MIPS.Assembler.Parsers.Expressions.Enums;
using MIPS.Assembler.Parsers.Expressions.Evaluator;
using MIPS.Assembler.Tokenization.Models;
using MIPS.Assembler.Tokenization.Models.Enums;
using MIPS.Models.Addressing;
using MIPS.Models.Modules.Tables;
using System;
using System.Buffers;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace MIPS.Assembler.Parsers;

/// <summary>
/// Parses expressions
/// </summary>
public ref struct ExpressionParser
{
    private readonly ILogger? _logger;
    private readonly AssemblerContext? _context;
    private readonly ReadOnlySpan<Token> _tokens;

    private ExpressionParser(ReadOnlySpan<Token> tokens, AssemblerContext? context, ILogger? logger)
    {
        _tokens = tokens;
        _logger = logger;
        _context = context;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="expression"></param>
    /// <param name="result"></param>
    /// <param name="relSymbol"></param>
    /// <param name="context"></param>
    /// <param name="logger"></param>
    /// <returns></returns>
    public static bool TryParse(ReadOnlySpan<Token> expression, out Address result, out SymbolEntry? relSymbol, AssemblerContext? context = null, ILogger? logger = null)
    {
        result = default;
        relSymbol = null;

        // Parse expression tree
        var parser = new ExpressionParser(expression, context, logger);
        var node = parser.ParsePrecedence(ref expression, 0);

        if (node is null)
        {
            // TODO: Log
            return false;
        }

        if (!expression.IsEmpty)
        {
            // TODO: Log
            return false;
        }

        // Evaluate the address
        var eval = new AddressEvaluator(logger);
        if (!node.TryEvaluate(eval, out result))
            return false;

        return true;
    }

    private ExpNode? ParsePrecedence(ref ReadOnlySpan<Token> tokens, int minBindingPower)
    {
        // NOTE: "token" is consumed as a side effect of Next
        var token = tokens.Next();
        if (token is null)
            return null;

        var leftNode = NullDenotation(ref tokens, token);
        if (leftNode is null)
        {
            // TODO: Log
            return null;
        }

        while(!tokens.IsEmpty &&
            TryGetBinaryOperator(tokens[0].Source, out var op) &&
            TryGetBindingPowers(op, out var leftBindingPower, out var rightBindingPower) &&
            leftBindingPower >= minBindingPower)
        {
            var opToken = tokens.Next();
            if (opToken is null)
            {
                // TODO: Log
                return null;
            }

            leftNode = LeftDenotation(ref tokens, leftNode, opToken, rightBindingPower);
            if (leftNode is null)
            {
                // TODO: Log
                return null;
            }
        }

        return leftNode;
    }

    private ExpNode? NullDenotation(ref ReadOnlySpan<Token> tokens, Token token)
    {
        ExpNode? result = null;

        bool success = token.Type switch
        {
            TokenType.Immediate => TryParseImmediate(token, out result),
            TokenType.Reference => TryParseReference(token, out result),
            TokenType.Operator when TryGetUnaryOperator(token.Source, out var op)
                => TryParseUnaryOperator(ref tokens, token, op, out result),
            // TODO: Handle parenthesis
            _ => false,
        };

        if (!success)
        {
            // TODO: Log
            return null;
        }

        return result;
    }

    private bool TryParseImmediate(Token token, [NotNullWhen(true)] out ExpNode? result)
    {
        result = null;

        long value;
        if (token.Source.Length > 0 && token.Source[0] is '\'')
        {
            // Character literal
            if (!StringParser.TryParseChar(token, out char c, _logger))
                return false;

            value = c;
        }
        else if (token.Source.Length >= 3 && !char.IsDigit(token.Source[1]))
        {
            // Binary, Oct, or Hex
            int @base = token.Source[1] switch
            {
                'b' => 2,
                'o' => 8,
                'x' => 16,
                _ => ThrowHelper.ThrowArgumentException<int>($"{token.Source[1]} is not a valid special immediate mode."),
            };

            // The tokenizer will allow bad immediates to be created. This is handled here when the convert throws an exception.
            try
            {
                value = Convert.ToInt64(token.Source[2..], @base);
            }
            catch
            {
                return false;
            }
        }
        else if (!long.TryParse(token.Source, out value))
        {
            return false;
        }

        result = new AddressNode(token, value);
        return true;
    }

    private bool TryParseReference(Token token, [NotNullWhen(true)] out ExpNode? result)
    {
        result = null;

        if (_context is null || !_context.TryGetSymbol(token.Source, out var symbol))
            return false;

        result = new SymbolNode(token, symbol);
        return true;
    }

    private bool TryParseUnaryOperator(ref ReadOnlySpan<Token> tokens, Token token, Operation op, out ExpNode? result)
    {
        result = null;

        if (!TryGetBindingPower(op, out var bindingPower))
            return false;

        var right = ParsePrecedence(ref tokens, bindingPower);
        if (right is null)
            return false;

        result = new UnaryOperNode(token, op)
        {
            Child = right
        };
        return true;
    }

    private BinaryOperNode? LeftDenotation(ref ReadOnlySpan<Token> tokens, ExpNode left, Token opToken, int rightBindingPower)
    {
        if (!TryGetBinaryOperator(opToken.Source, out Operation op))
        {
            // TOOD: Log
            return null;
        }

        var right = ParsePrecedence(ref tokens, rightBindingPower);
        if (right is null)
            return null;

        return new BinaryOperNode(opToken, op)
        {
            LeftChild = left,
            RightChild = right
        };
    }
    
    private static bool TryGetUnaryOperator(string s, out Operation op)
    {
        op = s switch
        {
            "+" => Operation.UnaryPlus,
            "-" => Operation.Negation,
            "~" => Operation.Not, // Accept "Binary not"
            _ => (Operation)(-1),
        };

        return op is not (Operation)(-1);
    }

    private static bool TryGetBinaryOperator(string s, out Operation op)
    {
        op = s switch
        {
            "+" => Operation.Addition,
            "-" => Operation.Subtraction,
            "*" => Operation.Multiplication,
            "/" => Operation.Division,
            "%" => Operation.Modulus,
            "&" => Operation.And,
            "|" => Operation.Or,
            "^" => Operation.Xor,
            "<<" => Operation.LeftShift,
            ">>" => Operation.RightShift,
            "~" => Operation.Not, // Accept "Binary not"
            _ => (Operation)(-1),
        };

        return op is not (Operation)(-1);
    }

    private static bool TryGetBindingPower(Operation op, out int bindingPower)
    {
        if (!TryGetBindingPowers(op, out bindingPower, out var right))
            return false;

        // Expected a unary operation
        if (right is not -1)
            return false;

        return true;
    }

    private static bool TryGetBindingPowers(Operation op, out int leftBindingPower, out int rightBindingPower)
    {
        (leftBindingPower, rightBindingPower) = op switch
        {
            // Binary operations
            Operation.Multiplication or 
            Operation.Division or
            Operation.Modulus => (60, 61),

            Operation.Addition or
            Operation.Subtraction => (50, 51),
            
            Operation.And => (40, 41),
            Operation.Xor => (35, 36),
            Operation.Or => (30, 31),
            
            Operation.LeftShift or
            Operation.RightShift => (45, 46),
            
            // TODO: "Binary not"

            // Unary operations
            Operation.UnaryPlus => (80, -1),
            Operation.Negation => (80, -1),
            Operation.Not => (80, -1),

            _ => (-1, -1),
        };

        if (leftBindingPower is -1)
            return false;

        return true;
    }
}
