// Adam Dernis 2023

using MIPS.Assembler.Models.Markers;
using MIPS.Assembler.Models.Markers.Abstract;
using MIPS.Assembler.Parsers;
using MIPS.Models.Instructions;
using System.Reflection.Metadata.Ecma335;

namespace MIPS.Assembler;

public unsafe partial class Assembler
{
    private MarkerParser _markerParser;
    private InstructionParser _instructionParser;

    private void LinePass1(string line)
    {
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
        if (markerStr is not null)
        {
            TokenizeMarker(markerStr, out var name, out var args);
            var marker = _markerParser.ParseMarker(name, args);

            if (marker is null)
                return;

            ExecuteMarker(marker);
        }
    }

    private void LinePass2(string line)
    {
        // Get the parts of the line
        TokenizeLine(line, out _, out var instructionStr, out _);

        // Only need to handle instructions
        if (instructionStr is null)
            return;

        // Get the parts of the instruction and parse
        if(!TokenizeInstruction(instructionStr, out var name, out var args))
            return;

        var instruction = _instructionParser.Parse(name, args);

        // Append instruction to active segment
        Append(instruction);
    }

    private void ExecuteMarker(Marker marker)
    {
        switch (marker)
        {
            case SegmentMarker segment:
                SetActiveSegment(segment.ActiveSegment);
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
