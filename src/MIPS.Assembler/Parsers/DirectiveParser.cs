// Adam Dernis 2024

using CommunityToolkit.Diagnostics;
using MIPS.Assembler.Logging;
using MIPS.Assembler.Logging.Enum;
using MIPS.Assembler.Models.Directives;
using MIPS.Assembler.Models.Directives.Abstract;
using MIPS.Assembler.Tokenization;
using MIPS.Assembler.Tokenization.Enums;
using MIPS.Models.Addressing.Enums;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Numerics;

namespace MIPS.Assembler.Parsers;

// TODO: Allow repeat with <express> [: <count>] format
// TODO: Handle expressions

/// <summary>
/// A struct for parsing directives.
/// </summary>
public readonly struct DirectiveParser
{
    private readonly ILogger? _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="DirectiveParser"/> struct.
    /// </summary>
    public DirectiveParser() : this(null)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="DirectiveParser"/> struct.
    /// </summary>
    public DirectiveParser(ILogger? logger)
    {
        _logger = logger;
    }

    /// <summary>
    /// Parses a directive from a name and a list of arguments.
    /// </summary>
    /// <param name="name">The directive name.</param>
    /// <param name="args">The directive arguments.</param>
    /// <param name="directive">The <see cref="Directive"/>.</param>
    /// <returns>Whether or not an directive was parsed.</returns>
    public bool TryParseDirective(Token name, Span<Token> args, out Directive? directive)
    {
        directive = null;

        return name.Source switch
        {
            // Section directives
            ".text" => TryParseSection(name, args, out directive),
            ".data" => TryParseSection(name, args, out directive),
            // Global References
            ".globl" => TryParseGlobal(args, out directive),
            // Align
            ".align" => TryParseAlign(args, out directive),
            // Data
            ".space" => TryParseSpace(args, out directive),
            ".word" => TryParseData<int>(name, args, out directive),
            ".half" => TryParseData<short>(name, args, out directive),
            ".byte" => TryParseData<byte>(name, args, out directive),
            ".ascii" => TryParseAscii(args, false, out directive),
            ".asciiz" => TryParseAscii(args, true, out directive),

            // Invalid directive
            _ => false
        };
    }

    private bool TryParseSection(Token name, Span<Token> args, out Directive? directive)
    {
        directive = null;

        if (args.Length != 0)
        {
            _logger?.Log(Severity.Error, LogId.InvalidDirectiveArgCount, "Section directives can not be parsed any arguments.");
            return false;
        }

        string sectionName = name.Source;
        Section section = sectionName switch
        {
            ".text" => Section.Text,
            ".data" => Section.Data,
            _ => ThrowHelper.ThrowArgumentException<Section>($"'{sectionName}' cannot be parsed as a section directive."),
        };

        directive = new SectionDirective(section);
        return true;
    }

    private bool TryParseGlobal(Span<Token> args, out Directive? directive)
    {
        directive = null;

        // Global only takes one argument
        if (args.Length is not 1)
        {
            _logger?.Log(Severity.Error, LogId.InvalidDirectiveArgCount, $".globl only takes one argument. Cannot parse {args.Length} arguments.");
            return false;
        }

        // Global only takes references as an argument
        if (args[0].Type is not TokenType.Reference)
        {
            _logger?.Log(Severity.Error, LogId.InvalidDirectiveArg, $".globl only takes a symbol for an argument. Cannot parse {args[0].Source}.");
        }

        directive = new GlobalDirective(args[0].Source);
        return true;
    }

    private bool TryParseAlign(Span<Token> args, out Directive? directive)
    {
        directive = null;

        // Align only takes one argument
        if (args.Length != 1)
        {
            _logger?.Log(Severity.Error, LogId.InvalidDirectiveArgCount, $".align only takes one argument. Cannot parse {args.Length} arguments.");
            return false;
        }

        // Align only takes immediates as an argument
        // TODO: Macro support
        if (args[0].Type is not TokenType.Immediate)
        {
            _logger?.Log(Severity.Error, LogId.InvalidDirectiveArg, $".align only takes an immediate value for an argument. Cannot parse {args[0].Source}.");
        }

        if (!int.TryParse(args[0].Source, out var boundary))
        {
            _logger?.Log(Severity.Error, LogId.InvalidDirectiveArg, $"'{args[0].Source}' is not a valid .align argument.");
            return false;
        }

        directive = new AlignDirective(boundary);
        return true;
    }

    private bool TryParseSpace(Span<Token> args, out Directive? directive)
    {
        directive = null;

        // Space only takes one argument
        if (args.Length != 1)
        {
            _logger?.Log(Severity.Error, LogId.InvalidDirectiveArgCount, $".space only takes one argument. Cannot parse {args.Length} arguments.");
            return false;
        }

        // Align only takes immediates as an argument
        // TODO: Macro support
        if (args[0].Type is not TokenType.Immediate)
        {
            _logger?.Log(Severity.Error, LogId.InvalidDirectiveArg, $".align only takes an immediate value for an argument. Cannot parse {args[0].Source}.");
        }

        if (!int.TryParse(args[0].Source, out var size))
        {
            _logger?.Log(Severity.Error, LogId.InvalidDirectiveArg, $"'{args[0].Source}' is not a valid .space argument.");
            return false;
        }

        directive = new DataDirective(new byte[size]);
        return true;
    }

    private bool TryParseData<T>(Token name, Span<Token> args, out Directive? directive)
        where T : unmanaged, IBinaryInteger<T>
    {
        directive = null;

        var format = CultureInfo.InvariantCulture.NumberFormat;
        T value = default;
        int argSize = value.GetByteCount();

        int pos = 0;

        // Allocate space
        var bytes = new byte[args.Length * argSize];

        Span<Token> arg;
        do
        {
            // Split the argument out of the span.
            args = args.SplitAtNext(TokenType.Comma, out arg, out _);

            // TODO: Evaluate expressions

            if (!T.TryParse(arg[0].Source, format, out value))
            {
                _logger?.Log(Severity.Error, LogId.InvalidDirectiveDataArg, $"{arg[0].Source} could not be parsed as a {name}");
                return false;
            }

            value.WriteBigEndian(bytes, pos);
            pos += argSize;
        } while (!args.IsEmpty);

        Array.Resize(ref bytes, pos);
        directive = new DataDirective(bytes);
        return true;
    }

    private bool TryParseAscii(Span<Token> args, bool terminate, out Directive? directive)
    {
        directive = null;

        var parser = new StringParser(_logger);

        var bytes = new List<byte>();

        Span<Token> arg;
        do
        {
            args = args.SplitAtNext(TokenType.Comma, out arg, out _);

            // TODO: Evaluate expressions

            // Parse string statement to string literal
            if (!parser.TryParseString(arg[0].Source, out var value))
                return false;

            // Copy to byte list
            bytes.Capacity += value.Length;
            bytes.AddRange(value.Select(x => (byte)x));

            // Null terminate string conditionally
            if (terminate)
                bytes.Add(0);
        } while (!args.IsEmpty);

        directive = new DataDirective(bytes.ToArray());
        return true;
    }
}
