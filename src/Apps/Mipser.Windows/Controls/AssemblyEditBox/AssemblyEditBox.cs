// Avishai Dernis 2025

using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using MIPS.Assembler.Models.Instructions;
using MIPS.Assembler.Tokenization.Models;
using Mipser.Messages.Editor.Enums;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using Windows.UI;
using WinUIEditor;

namespace Mipser.Windows.Controls.AssemblyEditBox;

/// <summary>
/// A modified <see cref="RichEditBox"/> to add assembly syntax-highlighting and other features.
/// </summary>
[TemplatePart(Name = CodeEditorPartName, Type = typeof(CodeEditorControl))]
public partial class AssemblyEditBox : Control
{
    private const string CodeEditorPartName = "CodeEditor";

    /// <remarks>
    /// The text is in UTF8, while the tokenizer output <see cref="SourceLocation"/> is in UTF16.
    /// We make this conversion during syntax highlighting. Track the results for log highlights.
    /// </remarks>
    private readonly Dictionary<int, SourceLocation> _locationMapper;

    private CodeEditorControl? _codeEditor;

    /// <summary>
    /// Initializes a new instance of the <see cref="AssemblyEditBox"/> class.
    /// </summary>
    public AssemblyEditBox()
    {
        _locationMapper = [];

        DefaultStyleKey = typeof(AssemblyEditBox);
    }

    /// <inheritdoc/>
    protected override void OnApplyTemplate()
    {
        base.OnApplyTemplate();

        // Setup template parts
        _codeEditor = (CodeEditorControl)GetTemplateChild(CodeEditorPartName);


        // Setup events
        this.Loaded += AssemblyEditBox_Loaded;

        // Setup keywords
        var table = new InstructionTable(new());
        SetupKeywords(table.GetInstructions());

        // Setup key-binds and styling
        SetupKeybinds();
        SetupHighlighting();
        SetupIndicators();
        
        // Apply the current text
        UpdateText();
    }

    /// <summary>
    /// Navigates to a <see cref="SourceLocation"/>.
    /// </summary>
    /// <param name="location">The position to navigate to.</param>
    public void NavigateToToken(SourceLocation location)
    {
        // Get the editor
        var editor = _codeEditor?.Editor;
        if (editor is null)
            return;

        // Attempt to get mapped location
        if(!_locationMapper.TryGetValue(location.Index, out var mappedLocation))
            return;

        // Go to position, and focus the keyboard
        editor.EnsureVisible(location.Line - 1);
        editor.GotoPos(mappedLocation.Index);
        _codeEditor?.Focus(FocusState.Keyboard);
    }

    [MemberNotNullWhen(true, nameof(_codeEditor))]
    private bool TryGetEditor([NotNullWhen(true)] out Editor? editor)
    {
        editor = _codeEditor?.Editor;
        return editor is not null;
    }

    private static int GetEncodingSize(string original)
        => Encoding.UTF8.GetByteCount(original);

    private int ToInt(Color color) => color.R | color.G << 8 | color.B << 16;
}
