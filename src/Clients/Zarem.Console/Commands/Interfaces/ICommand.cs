// Avishai Dernis 2026

using System.CommandLine;

namespace Zarem.Console.Commands.Interfaces;

/// <summary>
/// An interface for a console command.
/// </summary>
public interface ICommand
{
    /// <summary>
    /// Gets the name key of the command.
    /// </summary>
    static abstract string NameKey { get; }

    /// <summary>
    /// The action the command runs.
    /// </summary>
    /// <param name="parseResult">The parse command args.</param>
    static abstract void Action(ParseResult parseResult);

    /// <summary>
    /// Creates the <see cref="Command"/> for the 
    /// </summary>
    /// <returns></returns>
    static abstract Command Register();
}
