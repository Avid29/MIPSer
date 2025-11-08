// Avishai Dernis 2025

using Microsoft.UI.Xaml;
using System;

namespace Mipser.Editors.AssemblyEditBox;

public partial class AssemblyEditBox
{
    /// <summary>
    /// A <see cref="DependencyProperty"/> for the <see cref="Text"/> property.
    /// </summary>
    public static readonly DependencyProperty TextProperty =
        DependencyProperty.Register(nameof(Text), typeof(string), typeof(AssemblyEditBox), new PropertyMetadata(string.Empty, OnTextChanged));

    /// <summary>
    /// A <see cref="DependencyProperty"/> for the <see cref="Text"/> property.
    /// </summary>
    public static readonly DependencyProperty SyntaxHighlightingThemeProperty =
        DependencyProperty.Register(
            nameof(SyntaxHighlightingTheme),
            typeof(AssemblySyntaxHighlightingTheme),
            typeof(AssemblyEditBox),
            new PropertyMetadata(AssemblySyntaxHighlightingTheme.Default, OnThemeChanged));

    /// <summary>
    /// Gets or sets the text contained in the editbox.
    /// </summary>
    public string Text
    {
        get => (string)GetValue(TextProperty);
        set => SetValue(TextProperty, value);
    }

    /// <summary>
    /// Gets or sets the text contained in the editbox.
    /// </summary>
    public AssemblySyntaxHighlightingTheme SyntaxHighlightingTheme
    {
        get => (AssemblySyntaxHighlightingTheme)GetValue(SyntaxHighlightingThemeProperty);
        set => SetValue(SyntaxHighlightingThemeProperty, value);
    }

    private void UpdateTextProperty(string value) => SetValue(TextProperty, value);

    private static void OnThemeChanged(DependencyObject d, DependencyPropertyChangedEventArgs arg)
    {
        if (d is not AssemblyEditBox asmBox)
            return;

        asmBox.SetupHighlighting();
        asmBox.UpdateSyntaxHighlighting();
    }

    private static void OnTextChanged(DependencyObject d, DependencyPropertyChangedEventArgs arg)
    {
        if (d is not AssemblyEditBox asmBox)
            return;

        asmBox._codeEditor?.Editor.SetText(asmBox.Text);
        asmBox.TextChanged?.Invoke(d, EventArgs.Empty);
    }
}
