// Adam Dernis 2023

using MIPS.Models.Addressing.Enums;

namespace MIPS.Assembler;

public partial class Assembler
{
    private bool ParseMarker(string line)
    {
        var markerEnd = line.IndexOf(' ');
        var marker = line[1..markerEnd];
        switch (marker)
        {
            // Segment marker
            case "text":
                SetActiveSegment(Segment.Text);
                return true;
            case "data":
                SetActiveSegment(Segment.Data);
                return true;

            // Align
            case "align":
                return ParseAlign(line);

            // Data
            case "word":
                break;

            // Ascii
            case "ascii":
                ParseAscii(line);
                break;
            case "asciiz":
                ParseAscii(line, true);
                break;
        }

        // Invalid marker
        return false;
    }

    private bool ParseAlign(string line)
    {
        // Trim marker and whitespace
        line = line[".align".Length..].Trim();

        if(!int.TryParse(line, out var boundary))
            // TODO: Log error
            return false;

        _obj.Align(_activeSegment, boundary);
        return true;
    }

    private bool ParseAscii(string line, bool terminate = false)
    {
        // Null terminate conditionally
        if (terminate)
        {
            _obj.Append(_activeSegment, (byte)'\0');
        }

        return true;
    }
}
