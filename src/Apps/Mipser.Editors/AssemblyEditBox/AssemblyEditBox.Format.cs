// Avishai Dernis 2025

using CommunityToolkit.Diagnostics;
using MIPS.Assembler.Logging;
using MIPS.Assembler.Logging.Enum;
using MIPS.Assembler.Models.Instructions;
using MIPS.Assembler.Tokenization;
using MIPS.Assembler.Tokenization.Models;
using MIPS.Assembler.Tokenization.Models.Enums;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Windows.UI;
using WinUIEditor;

namespace Mipser.Editors.AssemblyEditBox;

public partial class AssemblyEditBox
{
    private HashSet<string>? Instructions = null;

    private bool @lock = false;

    /// <summary>
    /// Applies formatting based on a log messages.
    /// </summary>
    public void ApplyLogHighlights(IReadOnlyList<AssemblerLog> logs)
    {
        var editor = _codeEditor?.Editor;
        if (editor is null)
            return;

        ClearLogHighlights();

        foreach (var log in logs)
        {
            // Get the token's start location in utf8
            if (!_locationMapper.TryGetValue(log.Location.Index, out var utf8Location))
                continue;

            // Get the token's string
            var highlightString = new StringBuilder();
            foreach (var token in log.Tokens)
            {
                highlightString.Append(token.Source);
            }

            // Find the start and length, using the string's length
            var tokenLength = GetEncodingSize($"{highlightString}");
            var start = utf8Location.Index;
            
            // Select the indictor
            editor.IndicatorCurrent = log.Severity switch
            {
                Severity.Error => ErrorIndicatorIndex,
                Severity.Warning => WarningIndicatorIndex,
                Severity.Message => MessageIndicatorIndex,
                _ => ErrorIndicatorIndex,
            };

            // Apply the indicator
            editor.IndicatorFillRange(start, tokenLength);
        }
    }
    
    /// <summary>
    /// Clears formatting based on a log messages.
    /// </summary>
    public void ClearLogHighlights()
    {
        var editor = _codeEditor?.Editor;
        if (editor is null)
            return;

        // Clear underlines
        for (int i = ErrorIndicatorIndex; i <= MessageIndicatorIndex; i++)
        {
            editor.IndicatorCurrent = i;
            editor.IndicatorClearRange(0, editor.Length);
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
        
        // Clear token mappings
        _locationMapper.Clear();

        // Format line by line
        var text = editor.GetText(editor.Length);
        var reader = new StringReader(text);
        var utf16Pos = new SourceLocation();
        var utf8Pos = new SourceLocation();

        while (true)
        {
            var line = reader.ReadLine();
            if (line is null)
                break;

            // TODO: Check if the line has been updated
            FormatLine(ref utf16Pos, ref utf8Pos, line);
            utf8Pos = utf8Pos.NextLine(2);
            utf16Pos = utf16Pos.NextLine(1);
        }

        @lock = false;
    }

    private void FormatLine(ref SourceLocation utf16Pos, ref SourceLocation utf8Pos, string line)
    {
        Guard.IsNotNull(_codeEditor);
        var editor = _codeEditor.Editor;

        // We need to convert everything to utf8 sizing
        var lineLength = GetEncodingSize(line);

        // Clear the line to white
        editor.StartStyling(utf8Pos.Index, 0);
        editor.SetStyling(lineLength, 0);
        
        // Tokenize the line
        var tokenized = Tokenizer.TokenizeLine(line, mode: TokenizerMode.IDE);

        // Apply Syntax Highlighting for each token
        foreach (var token in tokenized.Tokens)
        {
            // Log token position in location mapper.
            _locationMapper.Add(utf16Pos.Index, utf8Pos);

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

            // Set style and advance utf8/utf16 positions
            //var tokenLength = Encoding.UTF8.GetByteCount(token.Source);
            var tokenLength = GetEncodingSize(token.Source);
            editor.StartStyling(utf8Pos.Index, 0);
            editor.SetStyling(tokenLength, style);
            utf16Pos += tokenLength;
            utf8Pos += tokenLength;
        }

        // Set fold level
        FoldLevel foldLevel = tokenized.Label is not null ? FoldLevel.HeaderFlag : (FoldLevel)1;
        editor.SetFoldLevel(utf8Pos.Line - 1, foldLevel);
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
        if (_codeEditor is null)
            return;

        var editor = _codeEditor.Editor;

        //editor.IndicSetStyle(ErrorIndicatorIndex, IndicatorStyle.Squiggle);
        editor.IndicSetStyle(ErrorIndicatorIndex, IndicatorStyle.SquigglePixmap);
        editor.IndicSetFore(ErrorIndicatorIndex, ToInt(SyntaxHighlightingTheme.ErrorUnderlineColor));
        editor.IndicSetUnder(ErrorIndicatorIndex, true);
        
        //editor.IndicSetStyle(WarningIndicatorIndex, IndicatorStyle.Diagonal);
        editor.IndicSetStyle(WarningIndicatorIndex, IndicatorStyle.SquigglePixmap);
        editor.IndicSetFore(WarningIndicatorIndex, ToInt(SyntaxHighlightingTheme.WarningUnderlineColor));
        editor.IndicSetUnder(WarningIndicatorIndex, true);
        
        editor.IndicSetStyle(MessageIndicatorIndex, IndicatorStyle.Plain);
        editor.IndicSetFore(MessageIndicatorIndex, ToInt(SyntaxHighlightingTheme.MessageUnderlineColor));
        editor.IndicSetUnder(MessageIndicatorIndex, true);
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
    
    private int GetEncodingSize(string original)
        => Encoding.UTF8.GetByteCount(original);

    private int ToInt(Color color) => color.R | color.G << 8 | color.B << 16;
}
