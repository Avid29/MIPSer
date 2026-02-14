// Avishai Dernis 2026

using CommunityToolkit.Diagnostics;
using System.CommandLine;
using Zarem.Console.Commands.Interfaces;

namespace Zarem.Console.Commands;

/// <summary>
/// A base class for a console command.
/// </summary>
/// <typeparam name="TSelf">The concrete type of the command.</typeparam>
public abstract class CommandBase<TSelf>: Command
    where TSelf : ICommand
{
    private static string GetName() => Program.CommandLocalizer[$"{TSelf.NameKey}CommandName"]!;

    private static string GetDescription() => Program.CommandLocalizer[$"{TSelf.NameKey}CommandDescription"]!;

    /// <summary>
    /// 
    /// </summary>
    protected CommandBase() : base(GetName(), GetDescription())
    {
        SetAction(Method);
    }

    /// <summary>
    /// The action the command runs.
    /// </summary>
    /// <param name="parseResult">The parse command args.</param>
    public abstract void Method(ParseResult parseResult);

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
