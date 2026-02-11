// Avishai Dernis 2025

using CommunityToolkit.Diagnostics;
using Zarem.MIPS.Models.Instructions.Enums;

namespace Zarem.Assembler.MIPS.Helpers.Tables;

/// <summary>
/// A class containing methods for floating-point formats lookups.
/// </summary>
public static class FloatFormatTable
{
    /// <summary>
    /// Attempts to get a float format by name.
    /// </summary>
    public static bool TryGetFloatFormat(string name, out FloatFormat format, out string lookupName)
    {
        format = 0;
        lookupName = string.Empty;

        // Split the text at the last period.
        // If there is no period, then the string does not contain a float format.
        var split = name.LastIndexOf('.');
        if (split is -1)
            return false;

        // Generate a lookup name by replacing the last part of the name with ".fmt"
        // and determine the float format based on the last part of the name.
        lookupName = name[..split] + ".fmt";
        format = name[(split+1)..].ToLower() switch
        {
            "s" => FloatFormat.Single,
            "d" => FloatFormat.Double,
            "l" => FloatFormat.Long,
            "w" => FloatFormat.Word,
            "ps" => FloatFormat.PairedSingle,
            _ => 0
        };

        return format is not 0;
    }
    
    /// <summary>
    /// Attempts to get a float-format string by value.
    /// </summary>
    public static string GetFloatFormatString(FloatFormat format)
    {
        return format switch
        {
            FloatFormat.Single => "S",
            FloatFormat.Double => "D",
            FloatFormat.Word => "W",
            FloatFormat.Long => "L",
            FloatFormat.PairedSingle => "PS",
            _ => ThrowHelper.ThrowArgumentOutOfRangeException<string>("Invalid float format"),
        };
    }

    /// <summary>
    /// Replaces the ".fmt" in a name with the appropriate float format string.
    /// </summary>
    public static string ApplyFormat(string name, FloatFormat format)
    {
        return name.Replace(".fmt", $".{GetFloatFormatString(format)}");
    }
}
