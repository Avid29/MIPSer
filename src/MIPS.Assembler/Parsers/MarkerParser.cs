// Adam Dernis 2023

using CommunityToolkit.Diagnostics;
using MIPS.Assembler.Models.Markers;
using MIPS.Assembler.Models.Markers.Abstract;
using MIPS.Models.Addressing.Enums;
using System.Globalization;
using System.Numerics;

namespace MIPS.Assembler.Parsers;

// TODO: Use Logger to handle errors
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
                return ParseAlignMarker(args);

            // Data
            case "space":
                return ParseSpaceMarker(args);

            case "word":
                return ParseData<int>(args);
            case "byte":
                return ParseData<byte>(args);

            // TODO: Parse ascii
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

    private static Marker? ParseAlignMarker(string[] args)
    {
        // Align only takes one argument
        if (args.Length != 1)
        {
            return ThrowHelper.ThrowArgumentException<Marker>($".align only takes one argument.");
        }

        var arg = args[0].Trim();

        if(!int.TryParse(arg, out var boundary))
        {
            return ThrowHelper.ThrowArgumentException<Marker>($"Invalid argument '{arg}'.");
        }

        return new AlignMarker(boundary);
    }

    private static Marker? ParseSpaceMarker(string[] args)
    {
        // Space only takes one argument
        if (args.Length != 1)
        {
            return ThrowHelper.ThrowArgumentException<Marker>($".space only takes one argument.");
        }
        
        var arg = args[0].Trim();

        if (!int.TryParse(arg, out var size))
        {
            return ThrowHelper.ThrowArgumentException<Marker>($"Invalid argument '{arg}'.");
        }

        return new DataMarker(new byte[size]);
    }

    private static Marker? ParseData<T>(string[] args)
        where T : unmanaged, IBinaryInteger<T>
    {
        var format = CultureInfo.InvariantCulture.NumberFormat;
        T value = default;
        int argSize = value.GetByteCount();

        int pos = 0;

        // Allocate space
        var bytes = new byte[args.Length * argSize];
        foreach (var arg in args)
        {
            if (!T.TryParse(arg, format, out value))
            {
                return ThrowHelper.ThrowArgumentException<Marker>($"Invalid argument '{arg}'.");
            }

            value.WriteBigEndian(bytes, pos);
            pos += argSize;
        }

        return new DataMarker(bytes);
    }
}
