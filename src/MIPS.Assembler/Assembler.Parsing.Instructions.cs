// Adam Dernis 2023

using CommunityToolkit.Diagnostics;
using MIPS.Models.Instructions;

namespace MIPS.Assembler;

public partial class Assembler
{
    private InstructionParser _instructionParser;

    private bool ParseInstruction(string line, int lineNum)
    {
        int instrNameEnd = line.IndexOf(' ');
        Guard.IsNotEqualTo(instrNameEnd, -1);
        string name = line[..instrNameEnd];
        string[] args = line[instrNameEnd..].Trim().Split(',');

        Instruction instruction = _instructionParser.Parse(name, args);

        Append(instruction);
        return true;
    }
}
