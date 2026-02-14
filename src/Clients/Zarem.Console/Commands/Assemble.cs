// Avishai Dernis 2026

using System.CommandLine;
using Zarem.Console.Commands.Interfaces;

namespace Zarem.Console.Commands;

/// <summary>
/// A handles the assemble command.
/// </summary>
public class Assemble : CommandBase<Assemble>, ICommand
{
    /// <inheritdoc/>
    public static string NameKey => "Assemble";

    /// <inheritdoc/>
    public static Command Register()
    {
        var command = Create();
        return command;
    }
}
