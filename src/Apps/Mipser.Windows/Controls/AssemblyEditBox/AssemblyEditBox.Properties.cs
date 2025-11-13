// Avishai Dernis 2025

using Microsoft.UI.Xaml;
using Mipser.Services.Settings.Enums;
using System;

namespace Mipser.Windows.Controls.AssemblyEditBox;

public partial class AssemblyEditBox
{
    /// <summary>
    /// A <see cref="DependencyProperty"/> for the <see cref="Text"/> property.
    /// </summary>
    public static readonly DependencyProperty TextProperty =
        DependencyProperty.Register(nameof(Text),
            typeof(string), 
            typeof(AssemblyEditBox),
            new PropertyMetadata(string.Empty, OnTextChanged));

    /// <summary>
    /// A <see cref="DependencyProperty"/> for the <see cref="RealTimeAssembly"/> property.
    /// </summary>
    public static readonly DependencyProperty RealTimeAssemblyChecksProperty =
        DependencyProperty.Register(nameof(RealTimeAssembly),
            typeof(bool),
            typeof(AssemblyEditBox),
            new PropertyMetadata(true, OnRTAssemblyChanged));

    /// <summary>
    /// A <see cref="DependencyProperty"/> for the <see cref="RealTimeAssembly"/> property.
    /// </summary>
    public static readonly DependencyProperty AnnotationThresholdProperty =
        DependencyProperty.Register(nameof(AnnotationThreshold),
            typeof(AnnotationThreshold),
            typeof(AssemblyEditBox),
            new PropertyMetadata(AnnotationThreshold.Errors, OnLogAnnotationsChanged));

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
    /// Gets or sets a value indicating whether or not to check assembly errors in real-time.
    /// </summary>
    public bool RealTimeAssembly
    {
        get => (bool)GetValue(RealTimeAssemblyChecksProperty);
        set => SetValue(RealTimeAssemblyChecksProperty, value);
    }

    /// <summary>
    /// Gets or sets a value indicating whether or not to show log annotations below indicators.
    /// </summary>
    public AnnotationThreshold AnnotationThreshold
    {
        get => (AnnotationThreshold)GetValue(AnnotationThresholdProperty);
        set => SetValue(AnnotationThresholdProperty, value);
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
        asmBox.SetupIndicators();
        asmBox.UpdateSyntaxHighlighting();
    }

    private static void OnTextChanged(DependencyObject d, DependencyPropertyChangedEventArgs arg)
    {
        if (d is not AssemblyEditBox asmBox)
            return;

        asmBox.UpdateText();
    }

    private static void OnRTAssemblyChanged(DependencyObject d, DependencyPropertyChangedEventArgs arg)
    {
        if (d is not AssemblyEditBox asmBox)
            return;

        asmBox.ClearLogHighlights();
    }
    
    private static void OnLogAnnotationsChanged(DependencyObject d, DependencyPropertyChangedEventArgs args)
    {
        if (d is not AssemblyEditBox asmBox)
            return;

        asmBox._codeEditor?.Editor.AnnotationClearAll();
    }

    private void UpdateText()
    {
        // Retrieve the editor
        var editor = _codeEditor?.Editor;
        if (editor is null)
            return;

        // Get current text, and check if it matches
        var text = editor.GetText(editor.Length);
        if (Text == text)
            return;

        // The text was not already update to date. Update it
        editor.SetText(Text);
        TextChanged?.Invoke(this, EventArgs.Empty);
    }
}
