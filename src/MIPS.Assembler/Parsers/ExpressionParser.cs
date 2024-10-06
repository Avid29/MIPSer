// Adam Dernis 2023

using CommunityToolkit.Diagnostics;
using MIPS.Assembler.Logging;
using MIPS.Assembler.Logging.Enum;
using MIPS.Assembler.Models.Modules;
using MIPS.Assembler.Parsers.Enums;
using MIPS.Assembler.Parsers.Expressions;
using MIPS.Assembler.Parsers.Expressions.Enums;
using MIPS.Assembler.Parsers.Expressions.Evaluator;
using MIPS.Models.Addressing;

namespace MIPS.Assembler.Parsers;

// TODO: Marco support
// TODO: Unary operator support
// TODO: Parenthesis support
// TODO: Hex and binary support

/// <summary>
/// A struct for parsing expressions.
/// </summary>
public struct ExpressionParser
{
    private readonly ModuleConstruction? _obj;
    private readonly ILogger? _logger;
    private readonly IEvaluator<Address> _evaluator;
    private ExpressionTree? _tree;
    private ExpressionParserState _state;
    private string _cache;
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
        _obj = obj;
        _logger = logger;
        _evaluator = new AddressEvaluator(logger);
        _tree = null;
        _state = ExpressionParserState.Start;
        _cache = string.Empty;
        _relocatableSymbol = null;
    }

    /// <summary>
    /// Parses an expression as an <see cref="Address"/>.
    /// </summary>
    /// <param name="expression">The string expression to parse.</param>
    /// <param name="result">The expression parsed as a integer.</param>
    /// <param name="symbol">The symbol referenced if address expression is relocatable. Null otherwise.</param>
    /// <returns><see langword="true"/> if the expression was successfully parsed, <see langword="false"/> otherwise.</returns>
    public bool TryParse(string expression, out Address result, out string? symbol)
    {
        result = default;
        symbol = null;

        _tree = new ExpressionTree();
        _state = ExpressionParserState.Start;

        // Build tree from string expression
        foreach (var c in expression)
        {
            // Ignore whitespace
            // Continue with no side effects
            if (char.IsWhiteSpace(c))
                continue;

            // Switch on the state, call the appropriate function, and track success
            bool success;
            switch (_state)
            {
                case ExpressionParserState.Start:
                    success = TryParseFromStart(c);
                    break;
                case ExpressionParserState.Integer:
                    success = TryParseFromInteger(c);
                    break;
                case ExpressionParserState.Macro:
                    success = TryParseFromMacro(c);
                    break;
                case ExpressionParserState.Operator:
                    success = TryParseFromOperator(c);
                    break;
                case ExpressionParserState.Char:
                    success = TryParseFromChar(c);
                    break;
                case ExpressionParserState.CharClose:
                    success = TryParseFromCharClose(c);
                    break;
                default:
                    ThrowHelper.ThrowArgumentOutOfRangeException($"Expression parser in invalid state '{_state}'");
                    return false;
            }

            // Parsing failed
            if (!success)
            {
                _logger?.Log(Severity.Error, LogId.UnparsableExpression, $"Could not parse '{expression}' as an expression.");
                return false;
            }
        }

        if (!TryFinish())
        {
            _logger?.Log(Severity.Error, LogId.UnparsableExpression, $"Could not parse '{expression}' as an expression.");
            return false;
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
            symbol = _relocatableSymbol;
        }

        return true;
    }

    private bool TryParseFromStart(char c)
    {
        if (char.IsLetter(c))
        {
            _state = ExpressionParserState.Macro;
            _cache = $"{c}";
            return true;
        }

        // '-' at the start marks a unary '-', which can be parsed as part of the integer
        if (char.IsDigit(c) || c is '-')
        {
            // Begin parsing as integer and 
            _state = ExpressionParserState.Integer;
            _cache = $"{c}";

            return true;
        }

        if (c is '\'')
        {
            _state = ExpressionParserState.Char;
            return true;
        }

        return false;
    }

    private bool TryParseFromInteger(char c)
    {
        if (char.IsDigit(c))
        {
            // Append digit to the end of digit progress
            _cache += c;
            return true;
        }

        if (IsOperator(c, out var oper))
        {
            TryParseOperator(oper);
            return true;
        }

        return false;
    }

    private bool TryParseFromMacro(char c)
    {
        if (char.IsLetterOrDigit(c))
        {
            // Append digit to the end of digit progress
            _cache += c;
            return true;
        }

        if (IsOperator(c, out var oper))
        {
            TryParseOperator(oper);
            return true;
        }



        return false;
    }

    private bool TryParseFromOperator(char c) => TryParseFromStart(c);

    private bool TryParseFromChar(char c)
    {
        if (c is '\'')
        {
            _state = ExpressionParserState.CharClose;
            return true;
        }

        _cache += c;
        return true;
    }

    private bool TryParseFromCharClose(char c)
    {
        if (IsOperator(c, out var oper))
        {
            TryParseOperator(oper);
            return true;
        }

        return false;
    }

    private bool TryFinish()
    {
        return _state switch
        {
            ExpressionParserState.Integer => TryCompleteInteger(),
            ExpressionParserState.Macro => TryCompleteMacro(),
            ExpressionParserState.CharClose => TryCompleteChar(),
            ExpressionParserState.Start => false,
            _ => false
        };
    }

    private bool TryParseOperator(Operation oper)
    {
        Guard.IsNotNull(_tree);

        // Attempt complete current cache
        if (!TryFinish())
            return false;

        var node = new OperNode(oper);
        _tree.AddNode(node);

        _state = ExpressionParserState.Operator;
        return true;
    }

    private bool TryCompleteInteger()
    {
        Guard.IsNotNull(_tree);

        if (!long.TryParse(_cache, out var result))
            return false;

        // Construct node
        var node = new AddressNode(result);
        _tree.AddNode(node);

        return true;
    }

    private bool TryCompleteMacro()
    {
        Guard.IsNotNull(_obj);
        Guard.IsNotNull(_tree);

        var parser = new SymbolParser(_logger);
        if (!parser.ValidateSymbolName(_cache))
            return false;

        if (!_obj.TryGetSymbol(_cache, out var value))
            return false;

        // Cache relocatable symbol
        if (value.IsRelocatable)
        {
            _relocatableSymbol = _cache;
        }

        var node = new AddressNode(value);
        _tree.AddNode(node);
        return true;
    }

    private bool TryCompleteChar()
    {
        Guard.IsNotNull(_tree);

        // TODO: Handle char substitutes
        if (!char.TryParse(_cache, out var result))
            return false;

        // Construct node
        var node = new AddressNode(result);
        _tree.AddNode(node);

        return true;
    }

    private static bool IsOperator(char c, out Operation oper)
    {
        oper = c switch
        {
            // Arithmetic
            '+' => Operation.Addition,
            '-' => Operation.Subtraction,
            '*' => Operation.Multiplication,
            '/' => Operation.Division,
            '%' => Operation.Modulus,

            // Logical
            '&' => Operation.And,
            '|' => Operation.Or,
            '^' => Operation.Xor,

            // Invalid
            _ => (Operation)(-1),
        };

        return oper != (Operation)(-1);
    }
}
