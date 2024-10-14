// Adam Dernis 2024

using MIPS.Assembler.Parsers;
using MIPS.Assembler.Tests.Live.Enums;
using MIPS.Assembler.Tokenization;
using System.Text;

namespace MIPS.Assembler.Tests.Live;

public class Program()
{
    private TestMode _mode = TestMode.Line;

    static async Task Main()
    {
        var program = new Program();
        while (true)
        {
            await program.Loop();
        }
    }

    private async Task Loop()
    {
        var message = _mode switch
        {
            TestMode.Line => "Enter line of assembly:",
            TestMode.Expression => "Enter expression:",
            _ => throw new ArgumentOutOfRangeException(nameof(_mode)),
        };

        Console.WriteLine(message);
        var line = Console.ReadLine();

        if (line is null)
            return;

        if (line.StartsWith('>'))
        {
            HandleCommand(line);
            return;
        }

        _ = _mode switch
        {
            TestMode.Line => await TestLine(line),
            TestMode.Expression => TestExpression(line),
            _ => false,
        };
    }

    private async Task<bool> TestLine(string line)
    {
        var stream = new MemoryStream(Encoding.Default.GetBytes(line));
        var assembler = await Assembler.AssembleAsync(stream, null);

        stream = new MemoryStream();
        assembler.WriteModule(stream);
        
        Console.WriteLine("Binary:");;
        for (int i = 0; stream.Position != stream.Length; i++)
        {
            int x = stream.ReadByte();

            Console.Write($"{x:X2} ");
            if (i % 4 is 3)
                Console.Write("  ");
            if (i % 8 is 7)
                Console.WriteLine();
        }

        Console.WriteLine();
        return !assembler.Failed;
    }

    private bool TestExpression(string line)
    {
        var tokens = Tokenizer.TokenizeLine(line, expression:true);
        var parser = new ExpressionParser();
        bool success = parser.TryParse(tokens.Tokens, out var result, out _);

        var message = success ? $"{line} evaluated to {result}" :
                                $"{line} could not be evaluated";
        Console.WriteLine(message);

        return success;
    }

    void HandleCommand(string line)
    {
        line = line.Trim('>');
        line = line.Trim();
        var modeArgs = line.Split(' ', StringSplitOptions.RemoveEmptyEntries);

        switch (modeArgs[0])
        {
            case "mode":
                if (modeArgs.Length != 2)
                {
                    Console.WriteLine("Mode command does ");
                    return;
                }

                SwapMode(modeArgs[1]);
                break;
        }
    }

    void SwapMode(string mode)
    {
        mode = mode.ToLower().Trim();
        TestMode? newMode = mode switch
        {
            "line" => TestMode.Line,
            "expression" => TestMode.Expression,
            _ => null,
        };

        if (newMode is null)
        {
            Console.WriteLine($"Unrecognized mode {mode}. Mode not changed");
            return;
        }

        _mode = newMode.Value;
        Console.WriteLine($"Mode swapped to {_mode} mode");
    }
}
