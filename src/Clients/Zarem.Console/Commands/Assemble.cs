// Avishai Dernis 2026

using System.CommandLine;
using Zarem.Console.Commands.Interfaces;

namespace Zarem.Console.Commands;

/// <summary>
/// A class that handles the assemble command.
/// </summary>
public class Assemble : CommandBase<Assemble>, ICommand
{
    /// <summary>
    /// Initializes a new instance of the <see cref="Assemble"/> class.
    /// </summary>
    public Assemble()
    {
    }

    /// <inheritdoc/>
    public static string NameKey => "Assemble";

    /// <inheritdoc/>
    public override void Method(ParseResult result)
    {

    }
}
