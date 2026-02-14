// Adam Dernis 2024

using System.CommandLine;
using Zarem.Console.Commands;
using Zarem.Localization;

namespace Zarem.Console;

internal class Program
{
    public static Localizer CommandLocalizer { get; } = new("Zarem.Console.Resources.Commands", typeof(Program).Assembly);

    private static void Main(string[] args)
    {
        var rootCommand = new RootCommand
        {
            Assemble.Register(),
            Build.Register(),
            Create.Register(),
        };

        rootCommand.Parse(args).Invoke();
    }
}
