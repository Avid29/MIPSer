// Adam Dernis 2024

using CommunityToolkit.Diagnostics;
using MIPS.Assembler.Helpers;
using MIPS.Assembler.Logging.Enum;
using MIPS.Assembler.Models.Directives;
using MIPS.Assembler.Models.Directives.Abstract;
using MIPS.Assembler.Parsers;
using MIPS.Assembler.Tokenization;
using MIPS.Assembler.Tokenization.Enums;
using MIPS.Models.Instructions;
using System;
using System.Runtime.CompilerServices;

namespace MIPS.Assembler;

public partial class Assembler
{
    private unsafe void AlignmentPass(AssemblyLine line)
    {
        // Parse as macro
        if (line.Type is LineType.Macro)
        {
            HandleMacro(line);
            return;
        }

        // Create symbol if line is labeled
        if (line.Label is not null)
            CreateSymbol(line.Label.Source);

        // Pad instruction sized allocation if instruction is present
        if (line.Type is LineType.Instruction)
        {
            Guard.IsNotNull(line.Instruction);

            // Get the number of real instructions to perform the instruction on this line.
            // We'll default to 1, because if the instruction fails to parse we'll replace it
            // with a NOP on the second pass.
            int realizedCount = 1;
            if(ConstantTables.TryGetInstruction(line.Instruction.Source, out var meta))
               realizedCount = meta.RealizedInstructionCount;

            // Multiply by realized instruction count to handle pseudo instructions
            Append(sizeof(Instruction) * realizedCount);
        }
        
        // Make allocations if directive is present
        // NOTE: Directive allocations are made in both passes
        if (line.Type is LineType.Directive)
            HandleDirective(line);
    }

    private void RealizationPass(AssemblyLine line)
    {
        switch (line.Type)
        {
            case LineType.Instruction:
                HandleInstruction(line);
                return;
                
            // Make allocations if directive is present
            // NOTE: Directive allocations are made in both passes
            case LineType.Directive:
                HandleDirective(line);
                return;

            // Macros can be skipped on realization pass
            case LineType.Macro:
                return;
        }
    }

    private void HandleMacro(AssemblyLine line)
    {
        var expressionParser = new ExpressionParser(Context, _logger);

        // Grab name and expression
        var name = line.Macro;
        var expression = line.MacroExpression;
        expression = expression.TrimType(TokenType.Assign, out var trimmed);

        // Ensure the name is not null, and that an assignment
        // token was trimmed from the expression.
        Guard.IsNotNull(name);
        Guard.IsNotNull(trimmed);

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

    private void HandleInstruction(AssemblyLine line)
    {
        // Try to parse the line
        var parser = new InstructionParser(Context, _logger);
        if (!parser.TryParse(line, out var instruction))
        {
            // Append a NOP in place of the unparsable instruction
            Append((uint)Instruction.NOP);
            return;
        }

        // Track relocatable reference
        if (instruction.SymbolReferenced is not null)
        {
            _module.TryTrackRelocation(CurrentAddress, instruction.SymbolReferenced);
        }

        // Append instruction to active segment
        Append(Unsafe.As<uint[]>(instruction.Realize()));
    }

    private void HandleDirective(AssemblyLine line)
    {
        var parser = new DirectiveParser();

        var name = line.Directive;
        var args = line.Args;

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
    
    private bool ValidateSymbolName(string symbol)
    {
        if (char.IsDigit(symbol[0]))
        {
            _logger?.Log(Severity.Error, LogId.IllegalSymbolName, $"{symbol} is not a valid symbol name. Symbol names cannot begin with a digit.");
            return false;
        }

        foreach (char c in symbol)
        {
            if (!char.IsLetterOrDigit(c))
            {
                _logger?.Log(Severity.Error, LogId.IllegalSymbolName, $"{symbol} is not a valid symbol name. Symbol names cannot contain the character {c}.");
                return false;
            }
        }

        return true;
    }
}
