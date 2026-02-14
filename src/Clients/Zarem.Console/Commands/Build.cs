// Avishai Dernis 2026

using System.CommandLine;
using Zarem.Console.Commands.Interfaces;

namespace Zarem.Console.Commands;

/// <summary>
/// A class that handles the build command.
/// </summary>
public class Build : CommandBase<Build>, ICommand
{
    /// <inheritdoc/>
    public static string NameKey => "Build";

    /// <inheritdoc/>
    public static void Action(ParseResult result)
    {

    }

    /// <inheritdoc/>
    public static Command Register()
    {
        var command = Create();

        return command;
    }
}
