// Adam Dernis 2024

using CommunityToolkit.Diagnostics;
using Microsoft.VisualBasic;
using MIPS.Assembler.Logging;
using MIPS.Assembler.Logging.Enum;
using MIPS.Assembler.Models;
using MIPS.Assembler.Parsers.Enums;
using MIPS.Assembler.Parsers.Expressions;
using MIPS.Assembler.Parsers.Expressions.Abstract;
using MIPS.Assembler.Parsers.Expressions.Enums;
using MIPS.Assembler.Parsers.Expressions.Evaluator;
using MIPS.Assembler.Tokenization.Models;
using MIPS.Assembler.Tokenization.Models.Enums;
using MIPS.Models.Addressing;
using MIPS.Models.Modules.Tables;
using System;

namespace MIPS.Assembler.Parsers;

// TODO: Marco support
// TODO: Parenthesis support

/// <summary>
/// A struct for parsing expressions.
/// </summary>
public struct ExpressionParser
{
    private readonly AssemblerContext? _context;
    private readonly ILogger? _logger;
    private readonly IEvaluator<Address> _evaluator;
    private ExpressionTree? _tree;
    private ExpressionParserState _state;
    private SymbolEntry? _refSymbol;

    /// <summary>
    /// Initializes a new instance of the <see cref="ExpressionParser"/> struct.
    /// </summary>
    public ExpressionParser() : this(null, null)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ExpressionParser"/> struct.
    /// </summary>
    public ExpressionParser(AssemblerContext? context, ILogger? logger = null)
    {
        _context = context;
        _logger = logger;
        _evaluator = new AddressEvaluator(logger);
        _tree = null;
        _state = ExpressionParserState.Start;
        _refSymbol = null;
    }

    /// <summary>
    /// Parses an expression as an <see cref="Address"/>.
    /// </summary>
    /// <param name="expression">The string expression to parse.</param>
    /// <param name="result">The expression parsed as a integer.</param>
    /// <param name="relSymbol">The symbol referenced if address expression is relocatable. Null otherwise.</param>
    /// <returns><see langword="true"/> if the expression was successfully parsed, <see langword="false"/> otherwise.</returns>
    public bool TryParse(ReadOnlySpan<Token> expression, out Address result, out SymbolEntry? relSymbol)
    {
        result = default;
        relSymbol = null;

        _tree = new ExpressionTree();
        _state = ExpressionParserState.Start;

        // Build tree from tokens expression
        foreach (var token in expression)
        {
            // Switch on the state, call the appropriate function, and track success
            bool success = _state switch
            {
                ExpressionParserState.Start or
                ExpressionParserState.Operator => TryParseFromStart(token),

                ExpressionParserState.Immediate or
                ExpressionParserState.Reference => TryParseAsOperator(token),
                _ => false,
            };

            // Parsing failed
            if (!success)
            {
                _logger?.Log(Severity.Error, LogId.UnparsableExpression, expression, "ExpressionParsingFailed", expression.Print());
                return false;
            }
        }

        // Evaluate tree
        if (!_tree.TryEvaluate(_evaluator, out result))
            return false;

        // Output symbol if result is relocatable.
        if (!result.IsFixed)
        {
            // NOTE: Relocatable values can only be in terms of one relocatable symbol
            // If they depend on more than one relocatable symbol, they will have
            // failed in evaluation.
            relSymbol = _refSymbol;
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
            t.Source is "+" or "-" or "~")
        {
            return TryParseAsOperator(t, true);
        }

        return false;
    }

    private bool TryParseAsOperator(Token token, bool unary = false)
    {
        if (token.Type is TokenType.Operator &&
            IsOperator(token, out var oper, unary))
        {
            TryParseOperator(token, oper, unary);
            return true;
        }

        return false;
    }

    private bool TryParseOperator(Token token, Operation oper, bool unary)
    {
        Guard.IsNotNull(_tree);

        OperNode node = unary ? new UnaryOperNode(token, oper) : new BinaryOperNode(token, oper);
        _tree.AddNode(node);

        _state = ExpressionParserState.Operator;
        return true;
    }

    private readonly bool AppendImmediate(Token token)
    {
        Guard.IsNotNull(_tree);

        long value;
        if (token.Source[0] is '\'')
        {
            // Character literal
            if (!StringParser.TryParseChar(token, out char c))
                return false;

            value = c;
        }
        else if (token.Source.Length > 2 && !char.IsDigit(token.Source[1]))
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
        
        // Construct node
        var node = new AddressNode(token, value);
        _tree.AddNode(node);

        return true;
    }

    private bool AppendReference(Token token)
    {
        Guard.IsNotNull(_tree);

        if (_context is null)
            return false;

        if (!_context.TryGetSymbol(token.Source, out var symbol))
            return false;

        // Cache non-fixed symbol
        if (!symbol.Address.IsFixed)
            _refSymbol = symbol;

        var node = new AddressNode(token, symbol.Address);
        _tree.AddNode(node);
        return true;
    }

    private static bool IsOperator(Token t, out Operation oper, bool unary = false)
    {
        if (unary)
        {
            oper = t.Source switch
            {
                "+" => Operation.UnaryPlus,
                "-" => Operation.Negation,
                "~" => Operation.Not,
                _ => (Operation)(-1),
            };
        }
        else
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
                "~" => Operation.Not,

                // Invalid
                _ => (Operation)(-1),
            };
        }

        return oper != (Operation)(-1);
    }
}
