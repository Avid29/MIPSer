// Adam Dernis 2024

using CommunityToolkit.Diagnostics;
using MIPS.Assembler.Logging.Enum;
using MIPS.Assembler.Models.Directives;
using MIPS.Assembler.Models.Directives.Abstract;
using MIPS.Assembler.Parsers;
using MIPS.Assembler.Tokenization;
using MIPS.Assembler.Tokenization.Enums;
using MIPS.Models.Instructions;
using MIPS.Models.Modules.Tables.Enums;
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
            DefineLabel(line.Label);

        // Pad instruction sized allocation if instruction is present
        if (line.Type is LineType.Instruction)
        {
            Guard.IsNotNull(line.Instruction);

            // Get the number of real instructions to perform the instruction on this line.
            // We'll default to 1, because if the instruction fails to parse we'll replace it
            // with a NOP on the second pass.
            int realizedCount = 1;
            if(Context.InstructionTable.TryGetInstruction(line.Instruction.Source, line.Args.Count, out var meta, out _))
               realizedCount = meta.RealizedInstructionCount ?? 1;

            // Multiply by realized instruction count to handle pseudo instructions
            Append(sizeof(Instruction) * realizedCount);
        }
        
        // Make allocations if directive is present
        // NOTE: Directive allocations are made in both passes
        // Issues are only logged on the second pass though
        if (line.Type is LineType.Directive)
            HandleDirective(line, false);
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
        var expression = line.Args[0];
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
        
        if (!expressionParser.TryParse(expression, out var address, out var _))
            return;
        
        if (address.IsRelocatable)
        {
            _logger.Log(Severity.Error, LogId.MacroCannotBeRelocatable, expression[0], "NoRelocatableMacros");
            return;
        }
        
        // TODO: Macro flags
        DefineSymbol(name, address, SymbolType.Macro);
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

        // Check if pseudo instructions are allowed
        if (instruction.IsPseduoInstruction && !Config.AllowPseudos)
        {
            _logger?.Log(Severity.Error, LogId.DisabledFeatureInUse, line.Tokens[0], "PseudoInstructionsDisabled");
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
                _module.TryDefineOrUpdateSymbol(global.Symbol, flags: SymbolFlags.Global);
                break;
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
            if (!char.IsLetterOrDigit(c))
            {
                _logger?.Log(Severity.Error, LogId.IllegalSymbolName, symbol, "SymbolCannotContain", name, c);
                return false;
            }
        }

        return true;
    }
}
