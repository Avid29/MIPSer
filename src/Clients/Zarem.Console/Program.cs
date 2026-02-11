// Adam Dernis 2024

using  Zarem.ObjFormats.RASM;
using  Zarem.ObjFormats.RASM.Config;
using System;
using System.IO;
using System.Threading.Tasks;
using Zarem.Assembler.MIPS.Logging.Enum;

namespace Zarem;

/// <summary>
/// The class containing the program entry point.
/// </summary>
public class Program
{
    /// <summary>
    /// Entry point.
    /// </summary>
    public static async Task<int> Main(params string[] args)
    {
        if (args.Length == 0)
        {
            await RequestArgs();
            return 0;
        }

        if (args.Length != 1)
        {
            Console.WriteLine("Invalid number of arguments.");
            return -1;
        }

        await Run(args[0]);
        return 0;
    }

    private static async Task RequestArgs()
    {
        Console.WriteLine($"Current directory: {Path.GetFullPath(".")}");

        string? path = null;
        while (path is null)
        {
            Console.Write("Please enter asm file path: ");
            path = Console.ReadLine();
        }

        await Main(path);
    }

    private static async Task Run(string filePath)
    {
        var resultFile = Path.ChangeExtension(filePath, ".obj");

        var inFile = File.Open(filePath, FileMode.Open);
        var outFile = File.Open(resultFile, FileMode.Create);
        var assembler = await Assembler.MIPS.Assembler.AssembleAsync<RasmModule, RasmConfig>(inFile, filePath, new RasmConfig());
        assembler.Module?.Save(outFile);


        Console.WriteLine(!assembler.Failed
            ? $"Assembled with {assembler.Logs.Count} messages."
            : $"Failed to assemble with {assembler.Logs.Count} messages.");

        Console.WriteLine();

        if (assembler.Logs.Count > 0)
        {
            foreach (var log in assembler.Logs)
            {
                Console.ForegroundColor = log.Severity switch
                {
                    Severity.Message => ConsoleColor.Blue,
                    Severity.Warning => ConsoleColor.Yellow,
                    Severity.Error => ConsoleColor.Red,
                    _ => ConsoleColor.White,
                };


                // TODO: Improved message
                var message = $"{log.Severity} on line {log.Location.Line}: {log.Message}";
                Console.WriteLine(message);
            }

            Console.ResetColor();

        }
    }
}
