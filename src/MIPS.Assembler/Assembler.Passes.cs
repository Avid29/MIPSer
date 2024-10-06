// Adam Dernis 2023

using CommunityToolkit.Diagnostics;
using MIPS.Assembler.Logging.Enum;
using MIPS.Assembler.Models.Directives;
using MIPS.Assembler.Models.Directives.Abstract;
using MIPS.Assembler.Parsers;
using MIPS.Assembler.Tokenization;
using MIPS.Assembler.Tokenization.Enums;
using MIPS.Models.Addressing;
using MIPS.Models.Instructions;
using System;

namespace MIPS.Assembler;

public unsafe partial class Assembler
{
    private void LinePass1(Span<Token> line)
    {
        var expressionParser = new ExpressionParser(_obj, _logger);
        var symbolParser = new SymbolParser(_logger);

        // Parse as macro
        if (TokenizeMacro(line, out var macro, out var expression))
        {
            if (expression.IsEmpty)
            {
                _logger.Log(Severity.Error, LogId.MacroMissingValue, $"Symbol '{macro}' missing value.");
                return;
            }

            //if (!expressionParser.TryParse(expression, out var address, out var _))
            //    return;

            // TODO: Real macro implementations
            var address = new Address();

            if (address.IsRelocatable)
            {
                _logger.Log(Severity.Error, LogId.MacroCannotBeRelocatable, $"Macros may not be a relocatable expression.");
                return;
            }

            CreateSymbol(macro.Value, address);
            return;
        }

        // Get the parts of the line
        TokenizeLine(line, out var label, out var instruction, out var directiveStr);

        // Create symbol if line is labeled
        if (label is not null)
            CreateSymbol(label.Value);

        // Pad instruction sized allocation if instruction is present
        if (!instruction.IsEmpty)
        {
            // TODO: Pseudo instructions
            Append(sizeof(Instruction));
        }

        // Make allocations if directive is present
        // NOTE: Directive allocations are made in both passes
        if (!directiveStr.IsEmpty)
            HandleDirective(directiveStr);
    }

    private void LinePass2(Span<Token> line)
    {
        var parser = new InstructionParser(_obj, _logger);

        // This line is a macro. Skip on pass2
        if (TokenizeMacro(line, out _, out _))
            return;

        // Get the parts of the line
        // Discard labels. They are already parsed
        TokenizeLine(line, out _, out var instructionTokens, out var directiveStr);

        // Handle instructions
        if (!instructionTokens.IsEmpty)
        {
            // Get the parts of the instruction and parse
            var args = instructionTokens.TrimType(TokenType.Instruction, out var name);
            if (name is null)
                return;

            // Try to parse instruction from name and arguments
            if (!parser.TryParse(name, args, out var instruction, out var symbol))
            {
                // Explicitly replace invalid instruction with a nop
                instruction = Instruction.NOP;
            }

            // Track relocatable reference
            if (symbol is not null)
            {
                _obj.TryTrackRelocation(CurrentAddress, symbol);
            }

            // Append instruction to active segment
            Append(instruction);
        }

        // Make allocations if directive is present
        // NOTE: Directive allocations are made in both passes
        if (!directiveStr.IsEmpty)
            HandleDirective(directiveStr);
    }

    private void HandleDirective(Span<Token> directiveTokens)
    {
        var parser = new DirectiveParser();

        var args = directiveTokens.TrimType(TokenType.Directive, out var name);

        if (name is null || !parser.TryParseDirective(name, args, out var directive))
            return;

        Guard.IsNotNull(directive);
        ExecuteDirective(directive);
    }

    private void ExecuteDirective(Directive directive)
    {
        switch (directive)
        {
            case SectionDirective segment:
                SetActiveSection(segment.ActiveSection);
                break;
            case AlignDirective align:
                Align(align.Boundary);
                break;
            case DataDirective data:
                Append(data.Data);
                break;
        }
    }
}
