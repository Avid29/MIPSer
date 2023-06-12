// Adam Dernis 2023

using MIPS.Models.Instructions;

namespace MIPS.Assembler;

public unsafe partial class Assembler
{
    private void LinePass1(string line)
    {
        // Get the parts of the line
        ParseLine(line, out var labelStr, out var instructionStr, out var markerStr);

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
            // TODO: Finish marker parsing
            ParseMarker(markerStr);
        }
    }

    private void LinePass2(string line)
    {
        // Get the parts of the line
        ParseLine(line, out _, out var instructionStr, out _);

        // Only need to handle instructions
        if (instructionStr is null)
            return;


        // Find instruction name
        int instrNameEnd = line.IndexOf(' ');
        if (instrNameEnd == -1)
        {
            // TODO: Log error
        }

        // Parse instruction
        string name = line[..instrNameEnd];
        string[] args = line[instrNameEnd..].Trim().Split(',');
        var instruction = _instructionParser.Parse(name, args);

        // Append instruction to active segment
        Append(instruction);
    }
}
