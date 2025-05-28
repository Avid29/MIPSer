// Adam Dernis 2024

using MIPS.Assembler.Models.Instructions;
using MIPS.Assembler.Parsers;
using MIPS.Assembler.Tests.Live.Enums;
using MIPS.Assembler.Tokenization;
using MIPS.Models.Instructions.Enums;
using RASM.Modules;
using RASM.Modules.Config;
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
        var assembler = await Assembler.AssembleAsync(stream, null, new RasmConfig());

        stream = new MemoryStream();
        assembler.CompleteModule<RasmModule>(stream);
        
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
        var cmdArgs = line.Split(' ', StringSplitOptions.RemoveEmptyEntries);

        switch (cmdArgs[0])
        {
            case "mode":
                if (cmdArgs.Length != 2)
                {
                    Console.WriteLine("Mode command requires exactly 1 argument 'mode'.");
                    return;
                }

                SwapMode(cmdArgs[1]);
                break;
            case "dump":
                if (cmdArgs.Length != 2)
                {
                    Console.WriteLine("Dump command requires exactly 1 argument 'table'.");
                    return;
                }

                Dump(cmdArgs[1]);
                break;
        }
    }

    void SwapMode(string mode)
    {
        mode = mode.Trim().ToLower();
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

    void Dump(string tableArg)
    {
        tableArg = tableArg.Trim().ToLower();
        switch (tableArg)
        {
            case "instructions":
                var instructions = new InstructionTable(MipsVersion.MipsIII).GetInstructions().OrderBy(x => x.Name);
                foreach (var instr in instructions)
                {
                    Console.WriteLine(instr.UsagePattern);
                }
                break;
        }
    }
}
