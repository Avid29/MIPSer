﻿// Adam Dernis 2024

using MIPS.Assembler.Models.Instructions;
using MIPS.Assembler.Parsers;
using MIPS.Assembler.Tests.Live.Enums;
using MIPS.Assembler.Tokenization;
using MIPS.Assembler.Tokenization.Enums;
using MIPS.Disassembler.Services;
using MIPS.Models.Instructions;
using MIPS.Models.Instructions.Enums;
using MIPS.Services;
using RASM.Modules;
using RASM.Modules.Config;
using System.Text;

namespace MIPS.Assembler.Tests.Live;

public class Program()
{
    private TestMode _mode = TestMode.Line;

    static async Task Main()
    {
        ServiceCollection.DisassemblerService = new DisassemblerService();

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

        if (assembler.Failed)
        {
            Console.WriteLine("\nAssembly failed:");
            foreach (var error in assembler.Logs)
            {
                Console.WriteLine($"- {error.Message}");
            }
        }

        Console.Write("\nBinary: ");
        uint inst = 0;
        for (int i = 0; stream.Position != stream.Length; i++)
        {
            int x = stream.ReadByte();

            Console.Write($"{x:X2} ");
            if (i % 4 is 3)
                Console.Write("  ");
            if (i % 8 is 7)
                Console.WriteLine();

            inst = inst << 8;
            inst += (uint)x;
        }

        Console.Write("\n\nDisassembly: ");
        var disassembly = new Disassembler.Disassembler(new RasmConfig()).DisassembleInstruction((Instruction)inst);
        
        Console.Write(disassembly);

        Console.WriteLine("\n");
        return !assembler.Failed;
    }

    private bool TestExpression(string line)
    {
        var tokens = Tokenizer.TokenizeLine(line, mode: TokenizerMode.Expression);
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
                if (cmdArgs.Length is not (2 or 3))
                {
                    Console.WriteLine("Dump command requires 1 or 2 argument 'table' and 'MIPS Version'.");
                    return;
                }

                var version = MipsVersion.MipsII;
                if (cmdArgs.Length is 3)
                {
                    version = (MipsVersion)int.Parse(cmdArgs[2]);
                }

                Dump(cmdArgs[1], version);
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

    void Dump(string tableArg, MipsVersion version = MipsVersion.MipsII)
    {
        tableArg = tableArg.Trim().ToLower();
        switch (tableArg)
        {
            case "instructions":
                var instructions = new InstructionTable(version).GetInstructions().OrderBy(x => x.Name);
                foreach (var instr in instructions)
                {
                    if (instr.IsPseudoInstruction)
                        Console.Write("* ");

                    Console.WriteLine(instr.UsagePattern);
                }
                Console.WriteLine("* Pseudo Instruction");
                break;
        }
    }
}
