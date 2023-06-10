// Adam Dernis 2023

using MIPS.Models.Addressing.Enums;
using System;

namespace MIPS.Assembler;

public partial class Assembler
{
    private bool ParseMarker(string line, int lineNum)
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
                return ParseAlign(line, lineNum);

            // Data
            case "ascii":
            case "asciiz":
                throw new NotImplementedException();
        }

        // Invalid marker
        return false;
    }

    private bool ParseAlign(string line, int lineNum)
    {
        // Trim marker and whitespace
        line = line[".align".Length..].Trim();

        if(!int.TryParse(line, out var boundary))
            // TODO: Log error
            return false;

        _obj.Align(_activeSegment, boundary);
        return true;
    }
}
