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
    /// <param name="line">The assembly line.</param>
    /// <param name="directive">The <see cref="Directive"/>.</param>
    /// <returns>Whether or not an directive was parsed.</returns>
    public bool TryParseDirective(AssemblyLine line, out Directive? directive)
    {
        directive = null;

        Guard.IsNotNull(line.Directive);
        var name = line.Directive;

        return name.Source switch
        {
            // Section directives
            ".text" => TryParseSection(name, line.Args, out directive),
            ".data" => TryParseSection(name, line.Args, out directive),
            // Global References
            ".globl" => TryParseGlobal(line.Args, out directive),
            // Align or Space
            ".align" => TryParseAlignOrSpace(line.Args, out directive, true),
            ".space" => TryParseAlignOrSpace(line.Args, out directive, false),
            // Data
            ".word" => TryParseData<int>(name, line.Args, out directive),
            ".half" => TryParseData<short>(name, line.Args, out directive),
            ".byte" => TryParseData<byte>(name, line.Args, out directive),
            ".ascii" => TryParseAscii(line.Args, false, out directive),
            ".asciiz" => TryParseAscii(line.Args, true, out directive),

            // Invalid directive
            _ => false
        };
    }

    private bool TryParseSection(Token name, AssemblyLineArgs args, out Directive? directive)
    {
        directive = null;

        if (args.Count is not 0)
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

    private bool TryParseGlobal(AssemblyLineArgs args, out Directive? directive)
    {
        directive = null;

        // Global only takes one argument
        if (args.Count is not 1 && args[0].Length is not 1)
        {
            _logger?.Log(Severity.Error, LogId.InvalidDirectiveArgCount, $".globl only takes one argument. Cannot parse {args.Count} arguments.");
            return false;
        }

        var arg = args[0][0];

        // Global only takes references as an argument
        if (arg.Type is not TokenType.Reference)
        {
            _logger?.Log(Severity.Error, LogId.InvalidDirectiveArg, $".globl only takes a symbol for an argument. Cannot parse {arg.Source}.");
        }

        directive = new GlobalDirective(arg.Source);
        return true;
    }

    private bool TryParseAlignOrSpace(AssemblyLineArgs args, out Directive? directive, bool align)
    {
        directive = null;
        string directiveName = align ? ".align" : ".space";

        // Space only takes one argument
        if (args.Count is not 1 && args[0].Length is not 1)
        {
            _logger?.Log(Severity.Error, LogId.InvalidDirectiveArgCount, $"{directiveName} only takes one argument. Cannot parse {args.Count} arguments.");
            return false;
        }

        // Align only takes immediates as an argument
        // TODO: Macro support
        var arg = args[0][0];
        if (arg.Type is not TokenType.Immediate)
        {
            _logger?.Log(Severity.Error, LogId.InvalidDirectiveArg, $"{directiveName} only takes an immediate value for an argument. Cannot parse {arg.Source}.");
        }

        if (!int.TryParse(arg.Source, out var value))
        {
            _logger?.Log(Severity.Error, LogId.InvalidDirectiveArg, $"'{arg.Source}' is not a valid {directiveName} argument.");
            return false;
        }

        if (align)
        {
            directive = new AlignDirective(value);
        }
        else
        {
            directive = new DataDirective(new byte[value]);
        }
        return true;
    }

    private bool TryParseData<T>(Token name, AssemblyLineArgs args, out Directive? directive)
        where T : unmanaged, IBinaryInteger<T>
    {
        directive = null;

        var format = CultureInfo.InvariantCulture.NumberFormat;
        T value = default;
        int argSize = value.GetByteCount();

        int pos = 0;

        // Allocate space
        var bytes = new byte[args.Count * argSize];

        for (int i = 0; i < args.Count; i++)
        {
            ReadOnlySpan<Token> arg = args[i];

            // TODO: Evaluate expressions
            if (!T.TryParse(arg[0].Source, format, out value))
            {
                _logger?.Log(Severity.Error, LogId.InvalidDirectiveDataArg, $"{arg[0].Source} could not be parsed as a {name}");
                return false;
            }

            value.WriteBigEndian(bytes, pos);
            pos += argSize;
        }

        directive = new DataDirective(bytes);
        return true;
    }

    private bool TryParseAscii(AssemblyLineArgs args, bool terminate, out Directive? directive)
    {
        directive = null;

        var parser = new StringParser(_logger);

        var bytes = new List<byte>();

        for (int i =  0; i < args.Count; i++)
        {
            ReadOnlySpan<Token> arg = args[i];

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
        }

        directive = new DataDirective(bytes.ToArray());
        return true;
    }
}
