// Avishai Dernis 2025

using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using MIPS.Assembler.Models.Instructions;
using MIPS.Assembler.Tokenization.Models;
using Mipser.Messages.Editor.Enums;
using System;
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

    /// <summary>
    /// Applies an editor operation.
    /// </summary>
    /// <param name="operation"></param>
    public void ApplyOperation(EditorOperation operation)
    {
        // Get the editor
        var editor = _codeEditor?.Editor;
        if (editor is null)
            return;

        Action? action = operation switch
        {
            EditorOperation.Cut => editor.SelectionEmpty ? editor.LineCut : editor.Cut,
            EditorOperation.Copy => editor.CopyAllowLine,
            EditorOperation.Paste => editor.Paste,
            EditorOperation.Duplicate => editor.SelectionEmpty ? editor.LineDuplicate : editor.SelectionDuplicate,
            _ => null,
        };

        if (action is null)
            return;

        // Perform the action
        action();
    }

    private void SetupKeybinds()
    {
        var editor = _codeEditor?.Editor;
        if (editor is null)
            return;

        // Clear built-in commands that will be handled through commands
        editor.ClearCmdKey(KeyDef('X', KeyMod.Ctrl));
        editor.ClearCmdKey(KeyDef('C', KeyMod.Ctrl));
        editor.ClearCmdKey(KeyDef('P', KeyMod.Ctrl));
        editor.ClearCmdKey(KeyDef('D', KeyMod.Ctrl));
        editor.ClearCmdKey(KeyDef(Keys.Up, KeyMod.Alt));
        editor.ClearCmdKey(KeyDef(Keys.Down, KeyMod.Alt));
    }

    private int KeyDef(char key, KeyMod mod)
    {
        return key + ((byte)key << 16);
    }

    private int KeyDef(Keys key, KeyMod mod) => KeyDef((char)key, mod);
}
