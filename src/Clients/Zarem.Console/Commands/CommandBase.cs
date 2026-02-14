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

        return new Command(name, desc);
    }
}
