// Adam Dernis 2023

using CommunityToolkit.Diagnostics;
using MIPS.Assembler.Parsers.Enums;
using MIPS.Assembler.Parsers.Expressions;
using MIPS.Assembler.Parsers.Expressions.Abstract;
using MIPS.Assembler.Parsers.Expressions.Evaluator;

namespace MIPS.Assembler.Parsers;

/// <summary>
/// A class for parsing expressions.
/// </summary>
public struct ExpressionParser
{
    // TODO: String support
    // TODO: Marco support
    // TODO: Parenthesis support
    // TODO: Hex and binary support

    private ExpressionParserState _state;
    private string _cache;

    private ExpNode? _root;
    //private OperNode? _activeNode; 

    /// <summary>
    /// Initializes a new instance of the <see cref="ExpressionParser"/> struct.
    /// </summary>
    public ExpressionParser()
    {
        _state = ExpressionParserState.Start;
        _cache = string.Empty;

        _root = null;
        //_activeNode = null;
    }
    
    /// <summary>
    /// Parses an expression as an integer.
    /// </summary>
    /// <remarks>
    /// Parsed as long integer.
    /// </remarks>
    /// <param name="expression">The string expression to parse.</param>
    /// <param name="result">The expression parsed as a integer.</param>
    /// <returns><see langword="true"/> if the expression was successfully parsed, <see langword="false"/> otherwise.</returns>
    public bool TryParseInteger(string expression, out long result)
    {
        var evaluator = new IntegerEvaluator();
        return TryParse(expression, evaluator, out result);
    }

    private bool TryParse<T>(string expression, IEvaluator<T> evaluator, out T? result)
    {
        result = default;
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
                    success = ParseFromStart(c);
                    break;
                case ExpressionParserState.Integer:
                    success = ParseFromInt(c);
                    break;
                default:
                    ThrowHelper.ThrowArgumentOutOfRangeException($"Expression parser in invalid state '{_state}'");
                    return false;
            }

            // Parsing failed
            if (!success)
                return false;
        }

        if (!Finish())
            return false;

        if (_root is not ExpNode<T> root)
            return false;

        // Evaluate tree and return result
        return root.TryEvaluate(evaluator, out result);
    }

    private bool ParseFromStart(char c)
    {
        if (char.IsLetter(c))
        {
            // TODO: Macros and symbols
            return false;
        }

        // '-' at the start marks a unary '-', which can be parsed as part of the integer
        if (char.IsDigit(c) || c is '-')
        {
            // Begin parsing as integer and 
            _state = ExpressionParserState.Integer;
            _cache = $"{c}";

            return true;
        }

        return false;
    }

    private bool ParseFromInt(char c)
    {
        if (char.IsDigit(c))
        {
            // Append digit to the end of digit progress
            _cache += c;
            return true;
        }

        return false;
    }

    private bool Finish()
    {
        switch (_state)
        {
            case ExpressionParserState.Integer:
                return CompleteInteger();

            case ExpressionParserState.Start:
            default:
                return false;
        }
    }

    private bool CompleteInteger()
    {
        if (!long.TryParse(_cache, out var result))
        {
            return false;
        }

        // Construct node
        var node = new IntegerNode(result);

        // Add node to tree
        // TODO: Properly add node to tree
        _root = node;

        return true;
    }
}
