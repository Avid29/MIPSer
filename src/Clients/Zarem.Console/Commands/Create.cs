// Avishai Dernis 2026

using System.CommandLine;
using Zarem.Console.Commands.Interfaces;

namespace Zarem.Console.Commands;

/// <summary>
/// A class that handles the new command.
/// </summary>
public class Create : CommandBase<Create>, ICommand
{
    /// <summary>
    /// Initializes a new instance of the <see cref="Create"/> class.
    /// </summary>
    public Create()
    {
        Options.Add(ArchOption);
    }

    /// <inheritdoc/>
    public static string NameKey => "Create";

    /// <summary>
    /// Gets the architecture option for the create command.
    /// </summary>
    public Option<string> ArchOption { get; } = CreateArchOption();

    /// <inheritdoc/>
    public override void Method(ParseResult result)
    {
    }

    private static Option<string> CreateArchOption()
    {
        var option = CreateOption<string>("Arch", "1");
        option.Required = true;
        return option;
    }
}

