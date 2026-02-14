// Avishai Dernis 2026

using CommunityToolkit.Diagnostics;
using System.CommandLine;
using Zarem.Console.Commands.Interfaces;

namespace Zarem.Console.Commands;

/// <summary>
/// A base class for a console command.
/// </summary>
/// <typeparam name="TSelf">The concrete type of the command.</typeparam>
public class CommandBase<TSelf>
    where TSelf : ICommand
{
    /// <summary>
    /// Creates a <see cref="Command"/> for the command, automatically grabbing localized name and description.
    /// </summary>
    protected static Command Create()
    {
        var name = Program.CommandLocalizer[$"{TSelf.NameKey}CommandName"];
        var desc = Program.CommandLocalizer[$"{TSelf.NameKey}CommandDescription"];
        Guard.IsNotNull(name);

        var command =  new Command(name, desc);
        command.SetAction(TSelf.Action);
        return command;
    }

    /// <summary>
    /// Creates an option for the command.
    /// </summary>
    protected static Option<T> CreateOption<T>(string nameKey, params string[] aliasKeys)
    {
        var name = $"--{Program.CommandLocalizer[$"{nameKey}OptionName"]}";
        var aliases = aliasKeys.Select(x => $"-{Program.CommandLocalizer[$"{nameKey}OptionAlias{x}"]}"!).ToArray();
        Guard.IsNotNull(name);

        return new Option<T>(name, aliases);
    }
}
