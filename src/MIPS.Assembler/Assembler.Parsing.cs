// Adam Dernis 2024

using CommunityToolkit.Diagnostics;
using MIPS.Assembler.Logging.Enum;
using MIPS.Assembler.Models.Directives;
using MIPS.Assembler.Models.Directives.Abstract;
using MIPS.Assembler.Parsers;
using MIPS.Assembler.Tokenization;
using MIPS.Assembler.Tokenization.Enums;
using MIPS.Models.Instructions;
using System;
using System.Diagnostics.CodeAnalysis;

namespace MIPS.Assembler;

public unsafe partial class Assembler
{
    private void LinePass1(Span<Token> line)
    {
        // Parse as macro
        if (TokenizeMacro(line, out var macro, out var expression))
        {
            HandleMacro(macro, expression);
            return;
        }

        // Get the parts of the line
        TokenizeLine(line, out var label, out var instruction, out var directiveStr);

        // Create symbol if line is labeled
        if (label is not null)
            CreateSymbol(label.Source);

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
        // This line is a macro. Skip on pass2
        if (TokenizeMacro(line, out _, out _))
            return;

        // Get the parts of the line
        // Discard labels. They are already parsed
        TokenizeLine(line, out _, out var instructionTokens, out var directiveTokens);

        // Handle instructions
        if (!instructionTokens.IsEmpty)
            HandleInstruction(instructionTokens);

        // Make allocations if directive is present
        // NOTE: Directive allocations are made in both passes
        if (!directiveTokens.IsEmpty)
            HandleDirective(directiveTokens);
    }

    private void HandleMacro(Token name, Span<Token> expression)
    {
        var expressionParser = new ExpressionParser(_obj, _logger);

        if (expression.IsEmpty)
        {
            _logger.Log(Severity.Error, LogId.MacroMissingValue, $"Symbol '{name}' missing value.");
            return;
        }
        
        if (!expressionParser.TryParse(expression, out var address, out var _))
            return;
        
        if (address.IsRelocatable)
        {
            _logger.Log(Severity.Error, LogId.MacroCannotBeRelocatable, $"Macros may not be a relocatable expression.");
            return;
        }
        
        CreateSymbol(name.Source, address);
    }

    private void HandleInstruction(Span<Token> instructionTokens)
    {
        var parser = new InstructionParser(_obj, _logger);
        
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
        Append((uint)instruction);
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
    
    private static bool TokenizeMacro(Span<Token> line, [NotNullWhen(true)] out Token? macro, out Span<Token> expression)
    {
        macro = null;
        expression = [];

        if (line.IsEmpty)
            return false;

        if (line[0].Type is not TokenType.MacroDefinition)
            return false;

        if (line[1].Type is not TokenType.Assign)
            ThrowHelper.ThrowArgumentException("Marco definition must be followed by assignment token '='");

        macro = line[0];
        expression = line[2..];
        return true;
    }

    private static void TokenizeLine(Span<Token> line, out Token? label, out Span<Token> instruction, out Span<Token> directive)
    {
        instruction = null;
        directive = null;

        // Find and parse label if present
        line = line.TrimType(TokenType.LabelDeclaration, out label);

        // Line contains neither a directive nor an instruction
        if (line.Length == 0)
            return;

        // Assign directive or instruction as appropriate
        switch (line[0].Type)
        {
            case TokenType.Directive:
                directive = line;
                break;
            case TokenType.Instruction:
                instruction = line;
                break;
        }
    }
}
