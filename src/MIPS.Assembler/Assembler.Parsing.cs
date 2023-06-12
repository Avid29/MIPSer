// Adam Dernis 2023

using MIPS.Assembler.Parsers;
using System.Linq;

namespace MIPS.Assembler;

public partial class Assembler
{
    private InstructionParser _instructionParser;

    private void ParseLine(string line, out string? label, out string? instruction, out string? marker)
    {
        label = null;
        instruction = null;
        marker = null;

        // Trim whitespace
        line = line.Trim();

        // Trim comment if present
        line = TrimComment(line);

        // Find and parse label if present
        int labelEnd = line.IndexOf(':');
        if (labelEnd != -1)
        {
            label = line[..labelEnd].Trim();
            if (!ValidateLabel(label))
            {
                label = null;

                // TODO: Log error
            }

            // Trim label from line
            line = line[(labelEnd+1)..];
        }

        // Line is neither a marker nor an instruction
        if (line.Length == 0)
            return;

        if (line[0] == '.')
        {
            // Line is a marker
            marker = line[1..];
        }
        else
        {
            // Line is an instruction
            instruction = line;
        }
    }

    private bool ValidateLabel(string label)
    {
        // No characters may be whitespace
        if (label.Any(char.IsWhiteSpace))
        {
            return false;
        }

        // Labels may not begin with a digit
        if (char.IsDigit(label[0]))
        {
            return false;
        }

        // All characters must be a letter or a digit
        if (!label.All(char.IsLetterOrDigit))
        {
            return false;
        }

        return true;
    }

    private static string TrimComment(string line)
    {
        // Find index of comment start
        int commentStart = line.IndexOf('#');

        // Trim comment if present
        return commentStart != -1 ? line[..commentStart] : line;
    }
}
