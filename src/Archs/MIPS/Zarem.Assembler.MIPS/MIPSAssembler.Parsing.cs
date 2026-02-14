// Adam Dernis 2024

using CommunityToolkit.Diagnostics;
using Zarem.Assembler.Parsers;
using System.Runtime.CompilerServices;
using Zarem.Assembler.Models.Directives;
using Zarem.Assembler.Tokenization.Models.Enums;
using Zarem.Assembler.Tokenization.Models;
using Zarem.Assembler.Extensions;
using Zarem.Assembler.Extensions.System;
using Zarem.Assembler.Models.Directives.Abstract;
using Zarem.Assembler.Logging.Enum;
using Zarem.Models.Modules.Tables.Enums;
using Zarem.Models.Instructions;

namespace Zarem.Assembler;

public partial class MIPSAssembler
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
            DefineLabel(line.Label);

        // Pad instruction sized allocation if instruction is present
        if (line.Type is LineType.Instruction)
        {
            Guard.IsNotNull(line.Instruction);

            // Get the number of real instructions to perform the instruction on this line.
            // We'll default to 1, because if the instruction fails to parse we'll replace it
            // with a NOP on the second pass.
            int realizedCount = 1;
            if(Context.InstructionTable.TryGetInstruction(line.Instruction.Source, line.Args.Count, out var meta, out _, out _))
               realizedCount = meta.RealizedInstructionCount ?? 1;

            // Multiply by realized instruction count to handle pseudo instructions
            Append(sizeof(MIPSInstruction) * realizedCount);
        }
        
        // Make allocations if directive is present
        // NOTE: Directive allocations are made in both passes
        // Issues are only logged on the second pass though
        if (line.Type is LineType.Directive)
            HandleDirective(line, false);

        // If args is non-zero while the line type is none,
        // random garbage is in the file.
        if (line.Type is LineType.None && line.Args.Count is not 0)
        {
            _logger.Log(Severity.Error, LogId.UnexpectedToken, line.Args[0].Tokens[0], "UnexpectedToken", line.Args[0].Tokens[0]);
        }
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
        // Grab name and expression
        var name = line.Macro;
        var expression = line.Args[0].Tokens;
        expression = expression.TrimType(TokenType.Assign, out var trimmed);

        // Ensure the name is not null, and that an assignment
        // token was trimmed from the expression.
        Guard.IsNotNull(name);
        Guard.IsNotNull(trimmed);

        if (expression.IsEmpty)
        {
            _logger.Log(Severity.Error, LogId.MacroMissingValue, expression[0], "SymbolMissingValue", name);
            return;
        }
        
        if (!ExpressionParser.TryParse(expression, out var result, Context, _logger))
            return;
        
        if (result.IsRelocatable)
        {
            _logger.Log(Severity.Error, LogId.MacroCannotBeRelocatable, expression[0], "NoRelocatableMacros");
            return;
        }
        
        // TODO: Macro flags
        DefineSymbol(name, result.Value, SymbolType.Macro);
    }

    private void HandleInstruction(AssemblyLine line)
    {
        // Try to parse the line
        var parser = new InstructionParser(Context, _logger);
        if (!parser.TryParse(line, out var instruction))
        {
            // Append a NOP in place of the unparsable instruction
            Append((uint)MIPSInstruction.NOP);
            return;
        }

        // Track relocatable reference
        if (instruction.Reference.HasValue)
        {
            _module.TryTrackReference(instruction.Reference.Value);
        }

        // Append instruction to active segment
        Append(Unsafe.As<uint[]>(instruction.Realize()));
    }

    private void HandleDirective(AssemblyLine line, bool log = true)
    {
        var parser = new DirectiveParser(Context, log ? _logger : null);

        var name = line.Directive;
        if (name is null || !parser.TryParseDirective(line, out var directive))
            return;

        Guard.IsNotNull(directive);
        ExecuteDirective(directive);
    }

    private void ExecuteDirective(Directive directive)
    {
        switch (directive)
        {
            case GlobalDirective global:
                _module.TryDefineOrUpdateSymbol(global.Symbol, binding: SymbolBinding.Global);
                break;
            case SectionDirective segment:
                SetActiveSection(segment.Section);
                break;
            case AlignDirective align:
                Align(align.Boundary);
                break;
            case DataDirective data:
                Append(data.Data);
                break;
        }
    }
    
    private bool ValidateSymbolName(Token symbol, out string name)
    {
        name = symbol.Source.TrimEnd(':');
        if (char.IsDigit(name[0]))
        {
            _logger?.Log(Severity.Error, LogId.IllegalSymbolName, symbol, "SymbolsCannotBeginWithDigits", name);
            return false;
        }

        foreach (char c in name)
        {
            if (!char.IsLetterOrDigit(c) && c is not '_')
            {
                _logger?.Log(Severity.Error, LogId.IllegalSymbolName, symbol, "SymbolCannotContain", name, c);
                return false;
            }
        }

        return true;
    }
}
