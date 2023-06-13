// Adam Dernis 2023

using CommunityToolkit.Diagnostics;
using MIPS.Assembler.Logging;
using MIPS.Assembler.Logging.Enum;
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
public readonly struct MarkerParser
{
    private readonly ILogger? _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="MarkerParser"/> struct.
    /// </summary>
    public MarkerParser() : this(null)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="MarkerParser"/> struct.
    /// </summary>
    public MarkerParser(ILogger? logger)
    {
        _logger = logger;
    }

    /// <summary>
    /// Parses a marker from a name and a list of arguments.
    /// </summary>
    /// <param name="name">The marker name.</param>
    /// <param name="args">The marker arguments.</param>
    /// <param name="marker">The <see cref="Marker"/>.</param>
    /// <returns>Whether or not an marker was parsed.</returns>
    public bool TryParseMarker(string name, string[] args, out Marker? marker)
    {
        marker = null;

        switch (name)
        {
            // Segment marker
            case "text":
            case "data":
                return TryParseSegmentMarker(name, out marker);

            // Align
            case "align":
                return TryParseAlignMarker(args, out marker);

            // Data
            case "space":
                return TryParseSpaceMarker(args, out marker);

            case "word":
                return TryParseData<int>(name, args, out marker);
            case "half":
                return TryParseData<short>(name, args, out marker);
            case "byte":
                return TryParseData<byte>(name, args, out marker);

            // TODO: Parse ascii
            case "ascii":
            case "asciiz":
                break;
        }

        // Invalid marker
        return false;
    }

    private static bool TryParseSegmentMarker(string name, out Marker marker)
    {
        Segment segment = name switch
        {
            "text" => Segment.Text,
            "data" => Segment.Data,
            _ => ThrowHelper.ThrowArgumentException<Segment>($"'{name}' cannot be parsed as a segment marker."),
        };

        marker = new SegmentMarker(segment);
        return true;
    }

    private bool TryParseAlignMarker(string[] args, out Marker? marker)
    {
        marker = null;

        // Align only takes one argument
        if (args.Length != 1)
        {
            _logger?.Log(Severity.Error, LogId.InvalidMarkerArgCount, $".align only takes one argument. Cannot parse {args.Length} arguments.");
            return false;
        }

        var arg = args[0].Trim();

        if(!int.TryParse(arg, out var boundary))
        {
            _logger?.Log(Severity.Error, LogId.InvalidMarkerArg, $"'{arg}' is not a valid .align argument.");
            return false;
        }

        marker = new AlignMarker(boundary);
        return true;
    }

    private bool TryParseSpaceMarker(string[] args, out Marker? marker)
    {
        marker = null;

        // Space only takes one argument
        if (args.Length != 1)
        {
            _logger?.Log(Severity.Error, LogId.InvalidMarkerArgCount, $".space only takes one argument. Cannot parse {args.Length} arguments.");
            return false;
        }
        
        var arg = args[0].Trim();

        if (!int.TryParse(arg, out var size))
        {
            _logger?.Log(Severity.Error, LogId.InvalidMarkerArg, $"'{arg}' is not a valid .space argument.");
            return false;
        }

        marker = new DataMarker(new byte[size]);
        return true;
    }

    private bool TryParseData<T>(string name, string[] args, out Marker? marker)
        where T : unmanaged, IBinaryInteger<T>
    {
        marker = null;

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
                _logger?.Log(Severity.Error, LogId.InvalidMarkerDataArg, $"{arg} could not be parsed as a {name}");
                return false;
            }

            value.WriteBigEndian(bytes, pos);
            pos += argSize;
        }

        marker = new DataMarker(bytes);
        return true;
    }
}
