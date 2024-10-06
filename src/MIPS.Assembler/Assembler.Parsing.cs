// Adam Dernis 2023

using CommunityToolkit.Diagnostics;
using MIPS.Assembler.Tokenization;
using MIPS.Assembler.Tokenization.Enums;
using System;
using System.Diagnostics.CodeAnalysis;

namespace MIPS.Assembler;

public partial class Assembler
{
    private bool TokenizeMacro(Span<Token> line, [NotNullWhen(true)] out Token? macro, out Span<Token> expression)
    {
        macro = null;
        expression = [];

        if (line[0].Type is not TokenType.MacroDefinition)
            return false;

        if (line[1].Type is not TokenType.Assign)
            ThrowHelper.ThrowArgumentException("Marco definition must be followed by assignment token '='");

        macro = line[0];
        expression = line[2..];
        return true;
    }

    private void TokenizeLine(Span<Token> line, out Token? label, out Span<Token> instruction, out Span<Token> directive)
    {
        instruction = null;
        directive = null;
        
        // Find and parse label if present
        line = line.TrimType(TokenType.LabelDeclaration, out label);

        // Line contains neither a directive nor an instruction
        if (line.Length == 0)
            return;

        // Assign directive or instruction as appropriate
        switch (line[0].Type)
        {
            case TokenType.Directive:
                directive = line;
                break;
            case TokenType.Instruction:
                instruction = line;
                break;
        }
    }

    private static int FindNameEnd(string line)
    {
        // TODO: This is dirty. Clean it up.

        int firstSpace = line.IndexOf(' ');
        int firstTab = line.IndexOf("\t", StringComparison.Ordinal);

        return (firstSpace == -1, firstTab == -1) switch
        {
            (false, false) => int.Min(firstSpace, firstTab),
            (false, true) => firstSpace,
            (true, false) => firstTab,
            (true, true) => -1,
        };
    }
}
