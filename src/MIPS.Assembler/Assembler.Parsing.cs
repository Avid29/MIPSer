// Adam Dernis 2023

using MIPS.Assembler.Logging.Enum;
using System;
using System.Linq;

namespace MIPS.Assembler;

public partial class Assembler
{
    private bool TokenizeMacro(string line, out string? macro, out string? expression)
    {
        macro = null;
        expression = null;

        int macroEnd = line.IndexOf('=');
        
        // Line does not contain a macro
        if (macroEnd == -1)
            return false;

        macro = line[..macroEnd];
        expression = line[(macroEnd+1)..];

        return true;
    }

    private void TokenizeLine(string line, out string? label, out string? instruction, out string? marker)
    {
        label = null;
        instruction = null;
        marker = null;
        // Find and parse label if present
        int labelEnd = line.IndexOf(':');
        if (labelEnd != -1)
        {
            label = line[..labelEnd].Trim();

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

    private static void CleanLine(ref string line)
    {
        // Trim zero width no-break space
        line = line.Trim('\uFEFF');
        
        // Trim whitespace
        line = line.Trim();

        TrimComment(ref line);
    }

    private static void TrimComment(ref string line)
    {
        // Find index of comment start
        int commentStart = line.IndexOf('#');
        
        // No comment found
        if (commentStart == -1)
            return;
        
        // Trim comment
        line = line[..commentStart];
    }

    private static int FindNameEnd(string line)
    {
        // TODO: This is dirty. Clean it up.

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
