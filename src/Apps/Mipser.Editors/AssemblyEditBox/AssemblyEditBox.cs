// Avishai Dernis 2025

using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using MIPS.Assembler.Models.Instructions;
using MIPS.Assembler.Tokenization.Models;
using System.Collections.Generic;
using WinUIEditor;

namespace Mipser.Editors.AssemblyEditBox;

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
        var table = new InstructionTable(MIPS.Models.Instructions.Enums.MipsVersion.MipsIII);
        SetupKeywords(table.GetInstructions());

        // Setup styling
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
        editor.GotoPos(mappedLocation.Index);
        _codeEditor?.Focus(FocusState.Keyboard);
    }
}
