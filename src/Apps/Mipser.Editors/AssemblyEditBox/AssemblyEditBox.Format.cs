// Avishai Dernis 2025

using CommunityToolkit.WinUI.Helpers;
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

    private async Task UpdateSyntaxHighlightingAsync()
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
        // Batch the following display updates
        Document.BatchDisplayUpdates();

        // Clear the line to white
        var lineRange = Document.GetRange(lineStart, lineStart + line.Length);
        lineRange.CharacterFormat.ForegroundColor = Colors.White;

        // Tokenize the line
        var tokenized = Tokenizer.TokenizeLine(line, mode:TokenizerMode.IDE);
        foreach(var token in tokenized.Tokens)
        {
            var tokenStart = lineStart + token.Location.Column-1;
            var tokenEnd = tokenStart + token.Source.Length;
            var tokenDocumentRange = Document.GetRange(tokenStart, tokenEnd);

            tokenDocumentRange.CharacterFormat.ForegroundColor = token.Type switch
            {
                TokenType.Instruction => "#A7FA95".ToColor(),
                TokenType.Register => "#FE8482".ToColor(),
                TokenType.Immediate => "#F8FC8B".ToColor(),

                TokenType.Reference or 
                TokenType.LabelDeclaration => "#73EEFD".ToColor(),

                TokenType.Operator => "#77A7FD".ToColor(),

                TokenType.Directive => "#FA9EF6".ToColor(),
                TokenType.Comma => "#77A7FD".ToColor(),
                TokenType.String => "#FFC47A".ToColor(),
                TokenType.Comment => "#9B88FC".ToColor(),
                _ => Colors.White,
            };
        }

        // Apply display updates
        Document.ApplyDisplayUpdates();
    }
}
