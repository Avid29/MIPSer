// Avishai Dernis 2026

using System.CommandLine;
using Zarem.Console.Commands.Interfaces;

namespace Zarem.Console.Commands;

/// <summary>
/// A class that handles the new command.
/// </summary>
public class Create : CommandBase<Create>, ICommand
{
    /// <inheritdoc/>
    public static string NameKey => "Create";

    /// <summary>
    /// Gets the architecture option for the create command.
    /// </summary>
    public static Option<string> ArchOption { get; } = CreateArchOption();

    /// <inheritdoc/>
    public static void Action(ParseResult result)
    {

    }

    /// <inheritdoc/>
    public static Command Register()
    {
        var command = Create();
        command.Options.Add(ArchOption);
        return command;
    }

    private static Option<string> CreateArchOption()
    {
        var option = CreateOption<string>("Arch", "1");
        option.Required = true;
        return option;
    }
}

