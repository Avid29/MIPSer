// Adam Dernis 2023

using System;
using System.Linq;

namespace MIPS.Assembler;

public partial class Assembler
{
    private void TokenizeLine(string line, out string? label, out string? instruction, out string? marker)
    {
        // Trim zero width no-break space
        line = line.Trim('\uFEFF');

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

    private bool TokenizeInstruction(string instruction, out string name, out string[] args)
    {
        if (string.IsNullOrWhiteSpace(instruction))
        {
            name = string.Empty;
            args = Array.Empty<string>();
            return false;
        }

        // Find instruction name
        int nameEnd = FindNameEnd(instruction);
        if (nameEnd == -1)
        {
            nameEnd = instruction.Length;
        }


        // Parse instruction
        name = instruction[..nameEnd];
        args = instruction[nameEnd..].Trim().Split(',');

        // Clear args to 0 if there are none
        if (args.Length == 1 && string.IsNullOrWhiteSpace(args[0]))
        {
            args = Array.Empty<string>();
        }

        return true;
    }

    private bool TokenizeMarker(string marker, out string name, out string[] args) => TokenizeInstruction(marker, out name, out args);

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

    private static int FindNameEnd(string line)
    {
        int firstSpace = line.IndexOf(' ');
        int firstTab = line.IndexOf("\t", StringComparison.Ordinal);

        return (firstSpace == -1, firstTab == -1) switch
        {
            (false, false) => int.Min(firstSpace, firstTab),
            (false, true) => firstSpace,
            (true, false) => firstTab,
            (true, true) => -1,
        };
    }
}
