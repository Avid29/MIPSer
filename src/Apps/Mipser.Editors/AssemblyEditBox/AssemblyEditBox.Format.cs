// Avishai Dernis 2025

using CommunityToolkit.Diagnostics;
using CommunityToolkit.WinUI.Helpers;
using MIPS.Assembler.Logging;
using MIPS.Assembler.Logging.Enum;
using MIPS.Assembler.Models.Instructions;
using MIPS.Assembler.Tokenization;
using MIPS.Assembler.Tokenization.Models.Enums;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Windows.UI;

namespace Mipser.Editors.AssemblyEditBox;

public partial class AssemblyEditBox
{
    private HashSet<string>? Instructions = null;

    private bool @lock = false;

    /// <summary>
    /// Applies formatting based on a log messages.
    /// </summary>
    public void ApplyLogHighlights(IReadOnlyList<Log> logs)
    {
        if (_codeEditor is null)
            return;

        // Clear underlines
        for (int i = ErrorIndicatorIndex; i <= MessageIndicatorIndex; i++)
        {
            _codeEditor.Editor.IndicatorCurrent = i;
            _codeEditor.Editor.IndicatorClearRange(0, -1);
        }

        foreach (var log in logs)
        {
            // Underline range
            _codeEditor.Editor.IndicatorCurrent = log.Severity switch
            {
                Severity.Message => MessageIndicatorIndex,
                Severity.Warning => WarningIndicatorIndex,
                Severity.Error => ErrorIndicatorIndex,
                _ => ErrorIndicatorIndex,
            };

            _codeEditor.Editor.IndicatorFillRange(log.Start, log.End);
        }
    }

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
            FormatLine(pos, line, out var lineLength);
            pos += lineLength + 2;
        }

        @lock = false;
    }

    private void FormatLine(int lineStart, string line, out int lineLength)
    {
        Guard.IsNotNull(_codeEditor);
        var editor = _codeEditor.Editor;

        // We need to convert everything to utf8 sizing
        lineLength = Encoding.UTF8.GetByteCount(line);

        // Clear the line to white
        editor.StartStyling(lineStart, 0);
        editor.SetStyling(lineLength, 0);
        
        // Tokenize the line
        int pos = lineStart;
        var tokenized = Tokenizer.TokenizeLine(line, mode: TokenizerMode.IDE);
        foreach (var token in tokenized.Tokens)
        {
            var style = token.Type switch
            {
                TokenType.Instruction when Instructions is not null =>
                    Instructions.Contains(token.Source) ? InstructionStyleIndex : InvalidInstructionStyleIndex,

                TokenType.Instruction => InstructionStyleIndex,
                TokenType.Register => RegisterStyleIndex,
                TokenType.Immediate => ImmediateStyleIndex,

                TokenType.Reference or
                TokenType.LabelDeclaration => ReferenceStyleIndex,

                TokenType.OpenParenthesis or TokenType.CloseParenthesis or
                TokenType.OpenBracket or TokenType.CloseBracket or TokenType.Comma or 
                TokenType.Operator => OperatorStyleIndex,

                TokenType.Directive => DirectiveStyleIndex,
                TokenType.String => StringStyleIndex,
                TokenType.Comment => CommentStyleIndex,

                _ => 0,
            };

            // Set style and advance position
            var tokenLength = Encoding.UTF8.GetByteCount(token.Source);
            editor.StartStyling(pos, 0);
            editor.SetStyling(tokenLength, style);
            pos += tokenLength;
        }
    }

    private const int InstructionStyleIndex = 1;
    private const int RegisterStyleIndex = 2;
    private const int ImmediateStyleIndex = 3;
    private const int ReferenceStyleIndex = 4;
    private const int OperatorStyleIndex = 5;
    private const int DirectiveStyleIndex = 6;
    private const int StringStyleIndex = 7;
    private const int CommentStyleIndex = 8;
    private const int MacroStyleIndex = 9;
    private const int InvalidInstructionStyleIndex = 10;
    //private const int InvalidRegisterStyleIndex = 11;
    //private const int InvalidReferenceStyleIndex = 11;

    private void SetupHighlighting()
    {
        if (_codeEditor is null)
            return;

        var editor = _codeEditor.Editor;

        editor.StyleSetFore(InstructionStyleIndex, ToInt(SyntaxHighlightingTheme.InstructionHighlightColor));
        editor.StyleSetFore(RegisterStyleIndex, ToInt(SyntaxHighlightingTheme.RegisterHighlightColor));
        editor.StyleSetFore(ImmediateStyleIndex, ToInt(SyntaxHighlightingTheme.ImmediateHighlightColor));
        editor.StyleSetFore(ReferenceStyleIndex, ToInt(SyntaxHighlightingTheme.ReferenceHighlightColor));
        editor.StyleSetFore(OperatorStyleIndex, ToInt(SyntaxHighlightingTheme.OperatorHighlightColor));
        editor.StyleSetFore(DirectiveStyleIndex, ToInt(SyntaxHighlightingTheme.DirectiveHighlightColor));
        editor.StyleSetFore(StringStyleIndex, ToInt(SyntaxHighlightingTheme.StringHighlightColor));
        editor.StyleSetFore(CommentStyleIndex, ToInt(SyntaxHighlightingTheme.CommentHighlightColor));
        editor.StyleSetFore(InvalidInstructionStyleIndex, ToInt(SyntaxHighlightingTheme.InvalidInstructionHighlightColor));
    }

    private const int ErrorIndicatorIndex = 8;
    private const int WarningIndicatorIndex = 9;
    private const int MessageIndicatorIndex = 10;

    private void SetupIndicators()
    {
        Guard.IsNotNull(_codeEditor);

        //_codeEditor.Editor.
    }

    private void SetupKeywords(InstructionMetadata[] instructions)
    {
        Instructions = [];

        foreach (var instr in instructions)
        {
            // TODO: Handle formatting instructions
            Instructions.Add(instr.Name);
        }
    }

    private int ToInt(Color color) => color.R | color.G << 8 | color.B << 16;
}
