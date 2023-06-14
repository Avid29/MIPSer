// Adam Dernis 2023

using CommunityToolkit.Diagnostics;
using MIPS.Assembler.Logging;
using MIPS.Assembler.Logging.Enum;
using MIPS.Assembler.Models.Directives;
using MIPS.Assembler.Models.Directives.Abstract;
using MIPS.Models.Addressing.Enums;
using System.Globalization;
using System.Numerics;
using System.Runtime.InteropServices;

namespace MIPS.Assembler.Parsers;

// TODO: Use Logger to handle errors
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
    public bool TryParseDirective(string name, string[] args, out Directive? directive)
    {
        directive = null;

        switch (name)
        {
            // Section directives
            case "text":
            case "data":
                return TryParseSection(name, args, out directive);

            // Global References
            case "globl":
                return TryParseGlobal(args, out directive);

            // Align
            case "align":
                return TryParseAlign(args, out directive);

            // Data
            case "space":
                return TryParseSpace(args, out directive);

            case "word":
                return TryParseData<int>(name, args, out directive);
            case "half":
                return TryParseData<short>(name, args, out directive);
            case "byte":
                return TryParseData<byte>(name, args, out directive);

            // TODO: Parse ascii
            case "ascii":
            case "asciiz":
                break;
        }

        // Invalid directive
        return false;
    }

    private bool TryParseSection(string name, string[] args, out Directive? directive)
    {
        directive = null;

        if (args.Length != 0)
        {
            _logger?.Log(Severity.Error, LogId.InvalidDirectiveArgCount, "Section directives can not be parsed any arguments.");
            return false;
        }

        Section section = name switch
        {
            "text" => Section.Text,
            "data" => Section.Data,
            _ => ThrowHelper.ThrowArgumentException<Section>($"'{name}' cannot be parsed as a section directive."),
        };

        directive = new SectionDirective(section);
        return true;
    }

    private bool TryParseGlobal(string[] args, out Directive? directive)
    {
        directive = null;

        // Global only takes one argument
        if (args.Length != 1)
        {
            _logger?.Log(Severity.Error, LogId.InvalidDirectiveArgCount, $".globl only takes one argument. Cannot parse {args.Length} arguments.");
            return false;
        }

        var arg = args[0].Trim();

        var parser = new SymbolParser(_logger);
        if (!parser.ValidateSymbolName(arg))
            return false;

        directive = new GlobalDirective(arg);
        return true;
    }

    private bool TryParseAlign(string[] args, out Directive? directive)
    {
        directive = null;

        // Align only takes one argument
        if (args.Length != 1)
        {
            _logger?.Log(Severity.Error, LogId.InvalidDirectiveArgCount, $".align only takes one argument. Cannot parse {args.Length} arguments.");
            return false;
        }

        var arg = args[0].Trim();

        if(!int.TryParse(arg, out var boundary))
        {
            _logger?.Log(Severity.Error, LogId.InvalidDirectiveArg, $"'{arg}' is not a valid .align argument.");
            return false;
        }

        directive = new AlignDirective(boundary);
        return true;
    }

    private bool TryParseSpace(string[] args, out Directive? directive)
    {
        directive = null;

        // Space only takes one argument
        if (args.Length != 1)
        {
            _logger?.Log(Severity.Error, LogId.InvalidDirectiveArgCount, $".space only takes one argument. Cannot parse {args.Length} arguments.");
            return false;
        }
        
        var arg = args[0].Trim();

        if (!int.TryParse(arg, out var size))
        {
            _logger?.Log(Severity.Error, LogId.InvalidDirectiveArg, $"'{arg}' is not a valid .space argument.");
            return false;
        }

        directive = new DataDirective(new byte[size]);
        return true;
    }

    private bool TryParseData<T>(string name, string[] args, out Directive? directive)
        where T : unmanaged, IBinaryInteger<T>
    {
        directive = null;

        var format = CultureInfo.InvariantCulture.NumberFormat;
        T value = default;
        int argSize = value.GetByteCount();

        int pos = 0;

        // Allocate space
        var bytes = new byte[args.Length * argSize];
        foreach (var arg in args)
        {
            if (!T.TryParse(arg, format, out value))
            {
                _logger?.Log(Severity.Error, LogId.InvalidDirectiveDataArg, $"{arg} could not be parsed as a {name}");
                return false;
            }

            value.WriteBigEndian(bytes, pos);
            pos += argSize;
        }

        directive = new DataDirective(bytes);
        return true;
    }
}
