// Adam Dernis 2023

using MIPS.Models.Instructions;
using System.Linq;

namespace MIPS.Assembler;

public partial class Assembler
{
    private InstructionParser _instructionParser;

    private void ParseLine(string line)
    {
        // Trim whitespace
        line = line.Trim();

        // Trim comment if present
        line = TrimComment(line);

        // Find and parse label if present
        int labelEnd = line.IndexOf(':');
        if (labelEnd != -1)
        {
            var label = line[..labelEnd].Trim();
            ParseLabel(label);

            // Trim label from line
            line = line[(labelEnd+1)..];
        }

        // Check for marker
        if (line[0] == '.')
        {
            ParseMarker(line);
        }
        // Parse as instruction
        else
        {
            ParseInstruction(line);
        }
    }

    private bool ParseInstruction(string line)
    {
        // Find instruction name
        int instrNameEnd = line.IndexOf(' ');
        if (instrNameEnd == -1)
        {
            // TODO: Log error
            return false;
        }

        // Parse instruction
        string name = line[..instrNameEnd];
        string[] args = line[instrNameEnd..].Trim().Split(',');
        Instruction instruction = _instructionParser.Parse(name, args);

        // Append instruction to active segment
        Append(instruction);
        return true;
    }

    private void ParseLabel(string label)
    {
        bool valid = ValidateLabel(label);

        if (valid)
        {
            CreateSymbolHere(label);
        }
        else
        {
            // TODO: Log exact error
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
        if (!char.IsDigit(label[0]))
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
