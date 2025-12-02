// Avishai Dernis 2025

using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Media;
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
    public static readonly DependencyProperty RealTimeAssemblyProperty =
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
            new PropertyMetadata(new AssemblySyntaxHighlightingTheme(), OnSyntaxHighlightingThemeChanged));

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
        get => (bool)GetValue(RealTimeAssemblyProperty);
        set => SetValue(RealTimeAssemblyProperty, value);
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

    private static void OnSyntaxHighlightingThemeChanged(DependencyObject d, DependencyPropertyChangedEventArgs arg)
    {
        if (d is not AssemblyEditBox asmBox)
            return;
        
        asmBox.OnColorSchemeUpdated();
        asmBox.SyntaxHighlightingTheme.Updated += (_, _) => asmBox.OnColorSchemeUpdated();

    }

    private void OnColorSchemeUpdated()
    {
        // This is not great
        Background = new SolidColorBrush(SyntaxHighlightingTheme.BackgroundColor);

        SetupHighlighting();
        SetupIndicators();
        UpdateSyntaxHighlighting();
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
        _ = asmBox.RunAssemblerAsync();
    }
    
    private static void OnLogAnnotationsChanged(DependencyObject d, DependencyPropertyChangedEventArgs args)
    {
        if (d is not AssemblyEditBox asmBox)
            return;

        asmBox.ClearLogHighlights();
        _ = asmBox.RunAssemblerAsync();
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
        editor.ConvertEOLs(WinUIEditor.EndOfLine.CrLf);
        TextChanged?.Invoke(this, EventArgs.Empty);
    }
}
