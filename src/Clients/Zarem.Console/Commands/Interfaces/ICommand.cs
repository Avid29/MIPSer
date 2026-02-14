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

    /// <inheritdoc/>
    static abstract Command Register();
}
