// Adam Dernis 2023

using CommunityToolkit.Diagnostics;
using MIPS.Assembler.Models.Markers;
using MIPS.Assembler.Models.Markers.Abstract;
using MIPS.Models.Addressing.Enums;

namespace MIPS.Assembler.Parsers;

// TODO: Handle expressions

/// <summary>
/// A struct for parsing markers.
/// </summary>
public struct MarkerParser
{
    /// <summary>
    /// Initializes a new instance of the <see cref="MarkerParser"/> struct.
    /// </summary>
    public MarkerParser()
    {
    }

    /// <summary>
    /// Parses a marker from a name and a list of arguments.
    /// </summary>
    /// <param name="name">The marker name.</param>
    /// <param name="args">The marker arguments.</param>
    /// <returns>A <see cref="Marker"/>.</returns>
    public Marker? ParseMarker(string name, params string[] args)
    {
        switch (name)
        {
            // Segment marker
            case "text":
            case "data":
                return ParseSegmentMarker(name);

            // Align
            case "align":
                return ParseAlign(args);

            // TODO: Parse data
            // Data
            case "space":
            case "word":
            case "ascii":
            case "asciiz":
                break;
        }

        // Invalid marker
        return null;
    }

    private static Marker ParseSegmentMarker(string marker)
    {
        Segment segment = marker switch
        {
            "text" => Segment.Text,
            "data" => Segment.Data,
            _ => ThrowHelper.ThrowArgumentException<Segment>($"'{marker}' cannot be parsed as a segment marker."),
        };

        return new SegmentMarker(segment);
    }

    private static Marker? ParseAlign(string[] args)
    {
        // Align only takes one argument
        if (args.Length != 1)
            return null;

        var arg = args[0].Trim();

        if(!int.TryParse(arg, out var boundary))
            return null;

        return new AlignMarker(boundary);
    }
}
