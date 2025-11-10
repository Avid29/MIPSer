// Adam Dernis 2024

using CommunityToolkit.Diagnostics;
using MIPS.Assembler.Logging;
using MIPS.Assembler.Logging.Enum;
using MIPS.Assembler.Models;
using MIPS.Assembler.Models.Directives;
using MIPS.Assembler.Models.Directives.Abstract;
using MIPS.Assembler.Tokenization.Models;
using MIPS.Assembler.Tokenization.Models.Enums;
using MIPS.Models.Addressing;
using MIPS.Models.Addressing.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Numerics;

namespace MIPS.Assembler.Parsers;

// TODO: Allow repeat with <express> [: <count>] format

/// <summary>
/// A struct for parsing directives.
/// </summary>
public readonly struct DirectiveParser
{
    private readonly AssemblerContext? _context;
    private readonly ILogger? _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="DirectiveParser"/> struct.
    /// </summary>
    public DirectiveParser(AssemblerContext context, ILogger? logger = null)
    {
        _context = context;
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
        var token = line.Directive;

        return token.Source switch
        {
            // Section directives
            ".text" => TryParseSection(token, line.Args, out directive),
            ".data" => TryParseSection(token, line.Args, out directive),

            // Global References
            ".globl" => TryParseGlobal(token, line.Args, out directive),

            // Align or Space
            ".align" => TryParseAlignOrSpace(token, line.Args, out directive, true),
            ".space" => TryParseAlignOrSpace(token, line.Args, out directive, false),

            // Data
            ".word" => TryParseData<int>(token, line.Args, out directive),
            ".half" => TryParseData<short>(token, line.Args, out directive),
            ".byte" => TryParseData<byte>(token, line.Args, out directive),
            ".ascii" => TryParseAscii(line.Args, false, out directive),
            ".asciiz" => TryParseAscii(line.Args, true, out directive),

            // Invalid directive
            _ => false
        };
    }

    private bool TryParseSection(Token name, AssemblyLineArgs args, out Directive? directive)
    {
        directive = null;

        string sectionName = name.Source;
        if (args.Count is not 0)
        {
            _logger?.Log(Severity.Error, LogCode.InvalidDirectiveArgCount, name, "DirectiveTakesNoArguments", sectionName);
            return false;
        }

        Section section = sectionName switch
        {
            ".text" => Section.Text,
            ".data" => Section.Data,
            _ => ThrowHelper.ThrowArgumentException<Section>($"'{sectionName}' cannot be parsed as a section directive."),
        };

        directive = new SectionDirective(section);
        return true;
    }

    private bool TryParseGlobal(Token token, AssemblyLineArgs args, out Directive? directive)
    {
        // TODO: Can you declare multiple globals on one line?
        directive = null;

        // Global requires an argument
        if (args.Count is 0)
        {
            _logger?.Log(Severity.Error, LogCode.InvalidDirectiveArgCount, token, "DirectiveRequiresAnArgument", ".globl");
            return false;
        }

        // Global takes only one argument
        if (args.Count is > 1)
        {
            // TODO: Improve token range message
            _logger?.Log(Severity.Error, LogCode.InvalidDirectiveArgCount, args[1], "DirectiveTakesOneArgument", ".globl");
            return false;
        }

        if (args[0].Length is not 1)
        {
            // TODO: Improve message
            _logger?.Log(Severity.Error, LogCode.InvalidDirectiveArg, args[0], "DirectiveNonSymbolArgumentSmall", ".globl");
            return false;
        }

        // Get argument
        var arg = args[0][0];

        // Global only takes references as an argument
        if (arg.Type is not TokenType.Reference)
        {
            _logger?.Log(Severity.Error, LogCode.InvalidDirectiveArg, arg, "DirectiveNonSymbolArgument", ".globl", arg.Source);
        }

        directive = new GlobalDirective(arg.Source);
        return true;
    }

    private bool TryParseAlignOrSpace(Token token, AssemblyLineArgs args, out Directive? directive, bool align)
    {
        directive = null;
        string directiveName = align ? ".align" : ".space";

        // Space and Align require an argument
        if (args.Count is 0)
        {
            _logger?.Log(Severity.Error, LogCode.InvalidDirectiveArgCount, token, "DirectiveRequiresAnArgument", directiveName);
            return false;
        }
        
        // Space and Align take only one argument
        if (args.Count is > 1)
        {
            _logger?.Log(Severity.Error, LogCode.InvalidDirectiveArgCount, args[1], "DirectiveTakesOneArgument", directiveName);
            return false;
        }

        // Parse argument
        var parser = new ExpressionParser(_context, _logger);
        if (!parser.TryParse(args[0], out Address result, out _))
            return false;

        // Argument must not be relocatable
        if (result.IsRelocatable)
        {
            _logger?.Log(Severity.Error, LogCode.InvalidDirectiveArg, args[0], "DirectiveNoRelocatableArguments", directiveName);
            return false;
        }

        var value = result.Value;

        if (align)
        {
            // Kinda unique behavior here warrents a comment.
            // Any int? comparison operator involving a null returns false, so
            // if there's no context this never executes.
            var alignWarningThreshold = _context?.Config.AlignWarningThreshold;
            var alignMessageThreshold = _context?.Config.AlignMessageThreshold;
            if (value >= alignWarningThreshold)
            {
                _logger?.Log(Severity.Warning, LogCode.LargeAlignment, args[0], "DirectiveLargeAlignWarning", value, alignWarningThreshold);
            }
            else if (value >= alignMessageThreshold)
            {
                _logger?.Log(Severity.Message, LogCode.LargeAlignment, args[0], "DirectiveLargeAlignMessage", value, alignMessageThreshold);
            }

            directive = new AlignDirective((int)result.Value);
        }
        else
        {
            var spaceMessageThreshold = _context?.Config.SpaceMessageThreshold;
            if (value >= spaceMessageThreshold)
            {
                _logger?.Log(Severity.Message, LogCode.LargeSpacing, args[0], "DirectiveLargeAlignMessage", value, spaceMessageThreshold);
            }

            directive = new DataDirective(new byte[value]);
        }
        return true;
    }

    private bool TryParseData<T>(Token name, AssemblyLineArgs args, out Directive? directive)
        where T : unmanaged, IBinaryInteger<T>
    {
        directive = null;

        T value = default;
        int argSize = value.GetByteCount();

        int pos = 0;

        // Allocate space
        var bytes = new byte[args.Count * argSize];

        for (int i = 0; i < args.Count; i++)
        {
            var arg = args[i];

            var parser = new ExpressionParser(_context, _logger);
            if (!parser.TryParse(arg, out var result, out _))
                return false;

            if (result.IsRelocatable)
            {
                // TODO: Can data be a reference to a relocatable address?
                _logger?.Log(Severity.Error, LogCode.InvalidDirectiveDataArg, args[0], "DirectiveAllocationNoRelocatableArguments", name);
                return false;
            }
            
            // TODO: Double check the logic here. Does this always detect the error?
            value = T.CreateTruncating(result.Value);
            if (value != T.CreateSaturating(result.Value))
            {
                _logger?.Log(Severity.Warning, LogCode.IntegerTruncated, arg, "DirectiveAllocationTruncated",  arg.Print(), result.Value, value);
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

        var bytes = new List<byte>();

        for (int i =  0; i < args.Count; i++)
        {
            ReadOnlySpan<Token> arg = args[i];

            // TODO: Evaluate expressions
            // Parse string statement to string literal
            if (!StringParser.TryParseString(arg[0], out var value))
                return false;

            // Copy to byte list
            bytes.Capacity += value.Length;
            bytes.AddRange(value.Select(x => (byte)x));

            // Null terminate string conditionally
            if (terminate)
                bytes.Add(0);
        }

        directive = new DataDirective([..bytes]);
        return true;
    }
}
