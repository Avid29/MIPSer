// Avishai Dernis 2025

using Microsoft.UI.Xaml;
using WinUIEditor;

namespace Mipser.Windows.Controls.CodeEditor;

public partial class AssemblyEditor
{
    private void AssemblyEditBox_Loaded(object sender, RoutedEventArgs e)
    {
        // While loaded, detach the loaded event and attach unloaded event
        this.Loaded -= AssemblyEditBox_Loaded;
        this.Unloaded += AssemblyEditBox_Unloaded;

        if (!TryGetEditor(out var editor))
            return;

        editor.Modified += Editor_Modified;
        editor.StyleNeeded += Editor_StyleNeeded;
        ChildEditor.SyntaxHighlightingApplied += CodeEditor_SyntaxHighlightingApplied;
        ChildEditor.HighlightingLanguage = "asm";
    }

    private void Editor_Modified(Editor sender, ModifiedEventArgs args)
    {
        var text = sender.GetText(sender.Length);
        UpdateTextProperty(text);
    }

    private async void Editor_StyleNeeded(Editor sender, StyleNeededEventArgs args)
    {
        UpdateSyntaxHighlighting();
        await RunAssemblerAsync();
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
