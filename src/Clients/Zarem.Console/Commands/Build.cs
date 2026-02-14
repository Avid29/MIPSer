// Avishai Dernis 2026

using System.CommandLine;
using Zarem.Console.Commands.Interfaces;

namespace Zarem.Console.Commands;

/// <summary>
/// A class that handles the build command.
/// </summary>
public class Build : CommandBase<Build>, ICommand
{
    /// <summary>
    /// Initializes a new instance of the <see cref="Build"/> class.
    /// </summary>
    public Build()
    {

    }

    /// <inheritdoc/>
    public static string NameKey => "Build";

    /// <inheritdoc/>
    public override void Method(ParseResult result)
    {
    }
}
