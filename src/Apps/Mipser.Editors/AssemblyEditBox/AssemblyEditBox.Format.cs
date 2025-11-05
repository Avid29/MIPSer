// Avishai Dernis 2025

using CommunityToolkit.Diagnostics;
using CommunityToolkit.WinUI.Helpers;
using MIPS.Assembler.Tokenization;
using MIPS.Assembler.Tokenization.Enums;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Windows.UI;

namespace Mipser.Editors.AssemblyEditBox;

public partial class AssemblyEditBox
{
    private bool @lock = false;

    /// <summary>
    /// Applies formatting based on a log messages.
    /// </summary>
    //public void ApplyLogHighlights(IReadOnlyList<Log> logs)
    //{
    //    // Clear underlines
    //    Document.GetText(TextGetOptions.None, out var temp);
    //    var range = Document.GetRange(0, temp.Length - 1);
    //    range.CharacterFormat.Underline = UnderlineType.None;

    //    foreach (var log in logs)
    //    {
    //        // Get log range
    //        range = Document.GetRange(log.Start, log.End);

    //        // Underline range
    //        range.CharacterFormat.Underline = log.Severity switch
    //        {
    //            Severity.Message => UnderlineType.ThickDotted,
    //            Severity.Warning => UnderlineType.Wave,
    //            Severity.Error => UnderlineType.Wave,
    //            _ => UnderlineType.Undefined,
    //        };
    //    }
    //}

    private void UpdateSyntaxHighlighting()
    {
        if (@lock || _codeEditor is null)
            return;

        @lock = true;
        var editor = _codeEditor.Editor;

        // Clear the style
        editor.StartStyling(0, 0);
        editor.SetStyling(editor.Length, 0);
        
        // Format line by line
        var text = editor.GetText(editor.Length);
        var reader = new StringReader(text);
        int pos = 0;
        while (true)
        {
            var line = reader.ReadLine();
            if (line is null)
                break;

            // TODO: Check if the line has been updated
            FormatLine(pos, line);
            pos += line.Length + 2;
        }

        @lock = false;
    }

    private void FormatLine(int lineStart, string line)
    {
        Guard.IsNotNull(_codeEditor);
        var editor = _codeEditor.Editor;

        // Clear the line to white
        editor.StartStyling(lineStart, 0);
        editor.SetStyling(line.Length, 0);

        // Tokenize the line
        var tokenized = Tokenizer.TokenizeLine(line, mode:TokenizerMode.IDE);
        foreach(var token in tokenized.Tokens)
        {
            var style = token.Type switch
            {
                TokenType.Instruction => InstructionStyleIndex,
                TokenType.Register => RegisterStyleIndex,
                TokenType.Immediate => ImmediateStyleIndex,

                TokenType.Reference or
                TokenType.LabelDeclaration => ReferenceStyleIndex,

                TokenType.Operator => OperatorStyleIndex,

                TokenType.Directive => DirectiveStyleIndex,
                TokenType.Comma => CommaStyleIndex,
                TokenType.String => StringStyleIndex,
                TokenType.Comment => CommentStyleIndex,

                _ => 0,
            };

            editor.StartStyling(lineStart + token.Location.Index, 0);
            editor.SetStyling(token.Source.Length, style);
        }
    }

    private const int InstructionStyleIndex = 1; 
    private const int RegisterStyleIndex = 2; 
    private const int ImmediateStyleIndex = 3;
    private const int ReferenceStyleIndex = 4;
    private const int OperatorStyleIndex = 5;
    private const int DirectiveStyleIndex = 6;
    private const int CommaStyleIndex = 7;
    private const int StringStyleIndex = 8;
    private const int CommentStyleIndex = 9;

    private void SetupHighlighting()
    {
        Guard.IsNotNull(_codeEditor);
        var editor = _codeEditor.Editor;

        editor.StyleSetFore(InstructionStyleIndex, ToInt("#A7FA95".ToColor()));
        editor.StyleSetFore(RegisterStyleIndex, ToInt("#FE8482".ToColor()));
        editor.StyleSetFore(ImmediateStyleIndex, ToInt("#F8FC8B".ToColor()));
        editor.StyleSetFore(ReferenceStyleIndex, ToInt("#73EEFD".ToColor()));
        editor.StyleSetFore(OperatorStyleIndex, ToInt("#77A7FD".ToColor()));
        editor.StyleSetFore(DirectiveStyleIndex, ToInt("#FA9EF6".ToColor()));
        editor.StyleSetFore(CommaStyleIndex, ToInt("#77A7FD".ToColor()));
        editor.StyleSetFore(StringStyleIndex, ToInt("#FFC47A".ToColor()));
        editor.StyleSetFore(CommentStyleIndex, ToInt("#9B88FC".ToColor()));

        UpdateSyntaxHighlighting();
    }

    private int ToInt(Color color) =>  color.R | color.G << 8 | color.B << 16;
}
