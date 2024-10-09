// Adam Dernis 2023

using CommunityToolkit.Diagnostics;
using MIPS.Assembler.Logging;
using MIPS.Assembler.Logging.Enum;
using MIPS.Assembler.Models.Modules;
using MIPS.Assembler.Parsers.Enums;
using MIPS.Assembler.Parsers.Expressions;
using MIPS.Assembler.Parsers.Expressions.Enums;
using MIPS.Assembler.Parsers.Expressions.Evaluator;
using MIPS.Assembler.Tokenization;
using MIPS.Assembler.Tokenization.Enums;
using MIPS.Models.Addressing;
using System;
using System.Globalization;

namespace MIPS.Assembler.Parsers;

// TODO: Marco support
// TODO: Parenthesis support

/// <summary>
/// A struct for parsing expressions.
/// </summary>
public struct ExpressionParser
{
    private readonly ModuleConstruction? _context;
    private readonly ILogger? _logger;
    private readonly IEvaluator<Address> _evaluator;
    private ExpressionTree? _tree;
    private ExpressionParserState _state;
    private string? _relocatableSymbol;

    /// <summary>
    /// Initializes a new instance of the <see cref="ExpressionParser"/> struct.
    /// </summary>
    public ExpressionParser() : this(null, null)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ExpressionParser"/> struct.
    /// </summary>
    public ExpressionParser(ModuleConstruction? obj, ILogger? logger = null)
    {
        _context = obj;
        _logger = logger;
        _evaluator = new AddressEvaluator(logger);
        _tree = null;
        _state = ExpressionParserState.Start;
        _relocatableSymbol = null;
    }

    /// <summary>
    /// Parses an expression as an <see cref="Address"/>.
    /// </summary>
    /// <param name="expression">The string expression to parse.</param>
    /// <param name="result">The expression parsed as a integer.</param>
    /// <param name="relSymbol">The symbol referenced if address expression is relocatable. Null otherwise.</param>
    /// <returns><see langword="true"/> if the expression was successfully parsed, <see langword="false"/> otherwise.</returns>
    public bool TryParse(Span<Token> expression, out Address result, out string? relSymbol)
    {
        result = default;
        relSymbol = null;

        _tree = new ExpressionTree();
        _state = ExpressionParserState.Start;

        // Build tree from string expression
        foreach (var token in expression)
        {
            // Switch on the state, call the appropriate function, and track success
            bool success = _state switch
            {
                ExpressionParserState.Start => TryParseFromStart(token),
                ExpressionParserState.Immediate => TryParseFromImmediate(token),
                ExpressionParserState.Reference => TryParseFromReference(token),
                ExpressionParserState.Operator => TryParseFromOperator(token),
                _ => false,
            };

            // Parsing failed
            if (!success)
            {
                // TODO: Convert token lines to string
                _logger?.Log(Severity.Error, LogId.UnparsableExpression, $"Could not parse '' as an expression.");
                return false;
            }
        }


        // Evaluate tree
        if (!_tree.TryEvaluate(_evaluator, out result))
            return false;

        // Output symbol if result is relocatable.
        if (result.IsRelocatable)
        {
            // NOTE: Relocatable values can only in terms of one relocatable symbol
            // If they depend on more than one relocatable symbol, they will have
            // failed in evaluation.
            relSymbol = _relocatableSymbol;
        }

        return true;
    }

    private bool TryParseFromStart(Token t)
    {
        if (t.Type is TokenType.Reference)
        {
            _state = ExpressionParserState.Reference;
            return AppendReference(t);
        }

        if (t.Type is TokenType.Immediate)
        {
            // Begin parsing as integer and 
            _state = ExpressionParserState.Immediate;
            return AppendImmediate(t);
        }

        // '-' at the start marks a unary '-', which we'll just handle by adding a 0 in front
        if (t.Type is TokenType.Operator &&
            t.Source is "+" or "-")
        {
            AppendImmediate(0);
            return TryParseFromImmediate(t);
        }

        // TODO: Char handling
        //if (t.Type is '\'')
        //{
        //    _state = ExpressionParserState.Char;
        //    return true;
        //}

        return false;
    }

    private bool TryParseFromImmediate(Token t)
    {
        if (t.Type is TokenType.Operator &&
            IsOperator(t, out var oper))
        {
            TryParseOperator(oper);
            return true;
        }

        return false;
    }

    private bool TryParseFromReference(Token t)
    {
        if (t.Type is TokenType.Operator &&
            IsOperator(t, out var oper))
        {
            TryParseOperator(oper);
            return true;
        }

        return false;
    }

    private bool TryParseFromOperator(Token t) => TryParseFromStart(t);

    private bool TryParseOperator(Operation oper)
    {
        Guard.IsNotNull(_tree);

        var node = new OperNode(oper);
        _tree.AddNode(node);

        _state = ExpressionParserState.Operator;
        return true;
    }

    private bool AppendImmediate(Token t)
    {
        long value;
        if (t.Source[0] is '\'')
        {
            // Character literal
            value = t.Source[1];
        }
        else if (t.Source.Length > 2 && !char.IsDigit(t.Source[1]))
        {
            // Binary, Oct, or Hex
            int @base = t.Source[1] switch
            {
                'b' => 2,
                'o' => 8,
                'x' => 16,
                _ => ThrowHelper.ThrowArgumentException<int>($"{t.Source[1]} is not a valid special immediate mode."),
            };
            
            // TODO: The tokenizer will currently allow base 10 digits in binary and oct immediates
            // This should be prevented or handled somewhere. For now it's "handled" here as an exception.
            value = Convert.ToInt64(t.Source[2..], @base);
        }
        else if (!long.TryParse(t.Source, out value))
        {
            return false;
        }

        return AppendImmediate(value);
    }

    private bool AppendImmediate(long value)
    {
        Guard.IsNotNull(_tree);

        // Construct node
        var node = new AddressNode(value);
        _tree.AddNode(node);

        return true;
    }

    private bool AppendReference(Token t)
    {
        Guard.IsNotNull(_context);
        Guard.IsNotNull(_tree);

        if (!_context.TryGetSymbol(t.Source, out var value))
            return false;

        // Cache relocatable symbol
        if (value.IsRelocatable)
        {
            _relocatableSymbol = t.Source;
        }

        var node = new AddressNode(value);
        _tree.AddNode(node);
        return true;
    }

    private static bool IsOperator(Token t, out Operation oper)
    {
        oper = t.Source switch
        {
            // Arithmetic
            "+" => Operation.Addition,
            "-" => Operation.Subtraction,
            "*" => Operation.Multiplication,
            "/" => Operation.Division,
            "%" => Operation.Modulus,

            // Logical
            "&" => Operation.And,
            "|" => Operation.Or,
            "^" => Operation.Xor,

            // Invalid
            _ => (Operation)(-1),
        };

        return oper != (Operation)(-1);
    }
}
