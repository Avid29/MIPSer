// Adam Dernis 2023

using CommunityToolkit.Diagnostics;
using MIPS.Assembler.Logging.Enum;
using MIPS.Assembler.Models.Markers;
using MIPS.Assembler.Models.Markers.Abstract;
using MIPS.Assembler.Parsers;
using MIPS.Models.Instructions;

namespace MIPS.Assembler;

public unsafe partial class Assembler
{
    private readonly MarkerParser _markerParser;
    private ExpressionParser _expressionParser;
    private InstructionParser _instructionParser;

    private void LinePass1(string line)
    {
        // Parse as macro
        if (TokenizeMacro(line, out var macro, out var expression))
        {
            Guard.IsNotNull(macro);
            if (!ValidateSymbolName(macro))
                return;

            if (string.IsNullOrEmpty(expression))
            {
                _logger.Log(Severity.Error, LogId.MacroMissingValue, $"Symbol '{macro}' missing value.");
                return;
            }

            if (!_expressionParser.TryParse(expression, out var address))
                return;

            if (address.IsRelocatable)
            {
                _logger.Log(Severity.Error, LogId.MacroCannotBeRelocatable, $"Macros may not be a relocatable expression.");
                return;
            }

            CreateSymbol(macro, address);
            return;
        }

        // Get the parts of the line
        TokenizeLine(line, out var labelStr, out var instructionStr, out var markerStr);

        // Create symbol if line is labeled
        if (labelStr is not null)
        {
            CreateSymbol(labelStr);
        }
        
        // Pad instruction sized allocation if instruction is present
        if (instructionStr is not null)
        {
            // TODO: Pseudo instructions
            Append(sizeof(Instruction));
        }

        // Make allocations if marker is present
        // NOTE: Marker allocations are made in both passes
        if (markerStr is not null)
            HandleMarker(markerStr);
    }

    private void LinePass2(string line)
    {
        // This line is a macro. Skip on pass2
        if (TokenizeMacro(line, out _, out _))
            return;

        // Get the parts of the line
        // Discard labels. They are already parsed
        TokenizeLine(line, out _, out var instructionStr, out var markerStr);

        // Handle instructions
        if (instructionStr is not null)
        {
            // Get the parts of the instruction and parse
            if(!TokenizeInstruction(instructionStr, out var name, out var args))
                return;

            // Try to parse instruction from name and arguments
            if (!_instructionParser.TryParse(name, args, out var instruction))
            {
                // Explicitly replace invalid instruction with a nop
                instruction = Instruction.NOP;
            }

            // Append instruction to active segment
            Append(instruction);
        }
        
        // Make allocations if marker is present
        // NOTE: Marker allocations are made in both passes
        if (markerStr is not null)
            HandleMarker(markerStr);
    }

    private void HandleMarker(string markerStr)
    {
        TokenizeMarker(markerStr, out var name, out var args);
        if (!_markerParser.TryParseMarker(name, args, out var marker))
            return;

        Guard.IsNotNull(marker);
        ExecuteMarker(marker);
    }

    private void ExecuteMarker(Marker marker)
    {
        switch (marker)
        {
            case SectionMarker segment:
                SetActiveSegment(segment.ActiveSection);
                break;

            case AlignMarker align:
                Align(align.Boundary);
                break;

            case DataMarker data:
                Append(data.Data);
                break;
        }
    }
}
