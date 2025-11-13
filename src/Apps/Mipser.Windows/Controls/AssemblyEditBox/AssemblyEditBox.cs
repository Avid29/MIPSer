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

        // Select or construct action
        Action? action = operation switch
        {
            EditorOperation.Undo => editor.Undo,
            EditorOperation.Redo => editor.Redo,
            EditorOperation.Cut => editor.SelectionEmpty ? editor.LineCut : editor.Cut,
            EditorOperation.Copy => editor.CopyAllowLine,
            EditorOperation.Paste => editor.Paste,
            EditorOperation.Duplicate => editor.SelectionEmpty ? editor.LineDuplicate : editor.SelectionDuplicate,
            EditorOperation.SelectAll => editor.SelectAll,
            EditorOperation.TransposeUp => () =>
            {
                editor.LineTranspose();
                editor.LineUp();
            },
            EditorOperation.TransposeDown => () =>
            {
                editor.LineDown();
                editor.LineTranspose();
            },
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

        // Clear all built-in commands and restore only desired ones
        // This must be done to prevent undesired key captures.
        editor.ClearAllCmdKeys();

        // Tab
        editor.AssignCmdKey(KeyDef(Keys.Tab), (int)ScintillaMessage.Tab);
        editor.AssignCmdKey(KeyDef(Keys.Tab, KeyMod.Shift), (int)ScintillaMessage.BackTab);

        // New line
        editor.AssignCmdKey(KeyDef(Keys.Return), (int)ScintillaMessage.NewLine);
        editor.AssignCmdKey(KeyDef(Keys.Return, KeyMod.Shift), (int)ScintillaMessage.NewLine);

        // Backspace/Delete
        editor.AssignCmdKey(KeyDef(Keys.Back), (int)ScintillaMessage.DeleteBack);
        editor.AssignCmdKey(KeyDef(Keys.Back, KeyMod.Ctrl), (int)ScintillaMessage.DelWordLeft);
        editor.AssignCmdKey(KeyDef(Keys.Delete), (int)ScintillaMessage.Clear);
        editor.AssignCmdKey(KeyDef(Keys.Delete, KeyMod.Ctrl), (int)ScintillaMessage.DelWordRight);

        // Zoom
        editor.AssignCmdKey(KeyDef(Keys.Add, KeyMod.Ctrl), (int)ScintillaMessage.ZoomIn);
        editor.AssignCmdKey(KeyDef(Keys.Subtract, KeyMod.Ctrl), (int)ScintillaMessage.ZoomOut);

        // Line down keys
        editor.AssignCmdKey(KeyDef(Keys.Down), (int)ScintillaMessage.LineDown);
        editor.AssignCmdKey(KeyDef(Keys.Down, KeyMod.Shift), (int)ScintillaMessage.LineDownExtend);
        editor.AssignCmdKey(KeyDef(Keys.Down, KeyMod.Ctrl), (int)ScintillaMessage.LineScrollDown);
        editor.AssignCmdKey(KeyDef(Keys.Down, KeyMod.Shift | KeyMod.Alt), (int)ScintillaMessage.LineDownRectExtend); // TODO: Make command?

        // Line up keys
        editor.AssignCmdKey(KeyDef(Keys.Up), (int)ScintillaMessage.LineUp);
        editor.AssignCmdKey(KeyDef(Keys.Up, KeyMod.Shift), (int)ScintillaMessage.LineUpExtend);
        editor.AssignCmdKey(KeyDef(Keys.Up, KeyMod.Ctrl), (int)ScintillaMessage.LineScrollUp);
        editor.AssignCmdKey(KeyDef(Keys.Up, KeyMod.Ctrl | KeyMod.Shift), (int)ScintillaMessage.LineUpRectExtend); // TODO: Make command?

        // Line left keys
        editor.AssignCmdKey(KeyDef(Keys.Left), (int)ScintillaMessage.CharLeft);
        editor.AssignCmdKey(KeyDef(Keys.Left, KeyMod.Shift), (int)ScintillaMessage.CharLeftExtend);
        editor.AssignCmdKey(KeyDef(Keys.Left, KeyMod.Ctrl), (int)ScintillaMessage.WordLeft);
        editor.AssignCmdKey(KeyDef(Keys.Left, KeyMod.Ctrl | KeyMod.Shift), (int)ScintillaMessage.WordLeftExtend);

        // Line right keys
        editor.AssignCmdKey(KeyDef(Keys.Right), (int)ScintillaMessage.CharRight);
        editor.AssignCmdKey(KeyDef(Keys.Right, KeyMod.Shift), (int)ScintillaMessage.CharRightExtend);
        editor.AssignCmdKey(KeyDef(Keys.Right, KeyMod.Ctrl), (int)ScintillaMessage.WordRight);
        editor.AssignCmdKey(KeyDef(Keys.Right, KeyMod.Shift | KeyMod.Alt), (int)ScintillaMessage.WordRightExtend);
    }

    private int KeyDef(char key, KeyMod mod = KeyMod.Norm)
    {
        return key + ((byte)mod << 16);
    }

    private int KeyDef(Keys key, KeyMod mod = KeyMod.Norm) => KeyDef((char)key, mod);
}
