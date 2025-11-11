// Avishai Dernis 2025

using CommunityToolkit.Diagnostics;
using Microsoft.UI.Xaml;
using MIPS.Assembler;
using MIPS.Assembler.Models;
using MIPS.Models.Instructions.Enums;
using System;
using WinUIEditor;

namespace Mipser.Editors.AssemblyEditBox;

public partial class AssemblyEditBox
{
    /// <summary>
    /// An event invoked when the <see cref="Text"/> property changes
    /// </summary>
    public event EventHandler? TextChanged;

    private void AssemblyEditBox_Loaded(object sender, RoutedEventArgs e)
    {
        // While loaded, detach the loaded event and attach unloaded event
        this.Loaded -= AssemblyEditBox_Loaded;
        this.Unloaded += AssemblyEditBox_Unloaded;

        Guard.IsNotNull(_codeEditor);

        _codeEditor.Editor.Modified += Editor_Modified;_codeEditor.SyntaxHighlightingApplied += CodeEditor_SyntaxHighlightingApplied;
        
        _codeEditor.Editor.StyleNeeded += Editor_StyleNeeded;

        _codeEditor.HighlightingLanguage = "asm";
    }

    private void Editor_Modified(Editor sender, ModifiedEventArgs args)
    {
        var text = sender.GetText(sender.Length);
        UpdateTextProperty(text);
    }

    private async void Editor_StyleNeeded(Editor sender, StyleNeededEventArgs args)
    {
        UpdateSyntaxHighlighting();

        // Run assembler and show errors
        var assembler = await Assembler.AssembleAsync(Text, null, new AssemblerConfig(MipsVersion.MipsIII));
        ApplyLogHighlights(assembler.Logs);
    }

    private void CodeEditor_SyntaxHighlightingApplied(object? sender, ElementTheme e)
    {
        SetupHighlighting();
    }

    private void AssemblyEditBox_Unloaded(object sender, RoutedEventArgs e)
    {
        // Restore the loaded event and detach unloaded event
        this.Loaded += AssemblyEditBox_Loaded;
        this.Unloaded -= AssemblyEditBox_Unloaded;
    }
}
