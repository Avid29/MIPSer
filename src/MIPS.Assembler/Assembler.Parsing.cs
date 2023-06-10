// Adam Dernis 2023

using System.Linq;

namespace MIPS.Assembler;

public partial class Assembler
{
    private void ParseLine(string line, int lineNum)
    {
        // Trim whitespace
        line = line.Trim();

        // Trim comment if present
        int commentStart = line.IndexOf('#');
        if (commentStart != -1)
        {
            line = line[..commentStart];
        }

        // Find and parse label if present
        int labelEnd = line.IndexOf(':');
        if (labelEnd != -1)
        {
            var label = line[..labelEnd].Trim();
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

        // Check for marker
        if (line[0] == '.')
        {
            ParseMarker(line, lineNum);
        }
        // Parse as instruction
        else
        {
            ParseInstruction(line, lineNum);
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
}
