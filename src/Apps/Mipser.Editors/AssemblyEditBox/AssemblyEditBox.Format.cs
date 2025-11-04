// Avishai Dernis 2025

using Microsoft.UI;
using Microsoft.UI.Text;
using MIPS.Assembler.Tokenization;
using MIPS.Assembler.Tokenization.Enums;
using System.IO;
using System.Threading.Tasks;

namespace Mipser.Editors.AssemblyEditBox;

public partial class AssemblyEditBox
{
    private bool @lock = false;

    private async Task UpdateSyntaxHighlighting()
    {
        if (@lock)
            return;

        @lock = true;
        Document.GetText(TextGetOptions.None, out string text);
        
        // Format line by line
        var reader = new StringReader(text);
        int pos = 0;
        while (true)
        {
            var line = await reader.ReadLineAsync();
            if (line is null)
                break;

            // TODO: Check if the line has been updated
            FormatLine(pos, line);
            pos += line.Length + 1;
        }
        @lock = false;
    }

    private void FormatLine(int lineStart, string line)
    {
        var tokenized = Tokenizer.TokenizeLine(line);
        
        // Clear the line to white
        var lineRange = Document.GetRange(lineStart, lineStart + line.Length);
        lineRange.CharacterFormat.ForegroundColor = Colors.White;

        foreach(var token in tokenized.Tokens)
        {
            var tokenStart = lineStart + token.Column;
            var tokenEnd = tokenStart + token.Source.Length;
            var tokenDocumentRange = Document.GetRange(tokenStart, tokenEnd);

            tokenDocumentRange.CharacterFormat.ForegroundColor = token.Type switch
            {
                TokenType.LabelDeclaration => Colors.Blue,
                TokenType.Directive => Colors.Purple,
                TokenType.Instruction => Colors.DarkCyan,
                TokenType.Register => Colors.Brown,
                TokenType.Immediate => Colors.LightYellow,
                TokenType.String => Colors.Maroon,
                _ => Colors.White,
            };
        }
    }
}
