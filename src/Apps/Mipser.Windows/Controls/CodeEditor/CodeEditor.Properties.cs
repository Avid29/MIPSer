// Avishai Dernis 2025

using Microsoft.UI.Xaml;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace Mipser.Windows.Controls.CodeEditor;

public partial class CodeEditor
{
    /// <summary>
    /// A <see cref="DependencyProperty"/> for the <see cref="Text"/> property.
    /// </summary>
    public static readonly DependencyProperty TextProperty =
        DependencyProperty.Register(nameof(Text),
            typeof(string),
            typeof(AssemblyEditor),
            new PropertyMetadata(string.Empty, OnTextChanged));

    /// <summary>
    /// Gets or sets the text contained in the editbox.
    /// </summary>
    public string Text
    {
        get => (string)GetValue(TextProperty);
        set => SetValue(TextProperty, value);
    }

    private static void OnTextChanged(DependencyObject d, DependencyPropertyChangedEventArgs arg)
    {
        if (d is not CodeEditor codeEditor)
            return;

        codeEditor.UpdateText();
    }

    private void UpdateText()
    {
        // Retrieve the editor
        var editor = ChildEditor?.Editor;
        if (editor is null)
            return;

        // Get current text, and check if it matches
        var text = editor.GetText(editor.Length);
        if (Text == text)
            return;

        // The text was not already update to date. Update it
        editor.SetText(Text);
        editor.ConvertEOLs(WinUIEditor.EndOfLine.CrLf);
        TextChanged?.Invoke(this, EventArgs.Empty);
    }
}
