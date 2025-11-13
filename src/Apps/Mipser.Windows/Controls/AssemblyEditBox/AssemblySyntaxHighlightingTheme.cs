// Avishai Dernis 2025

using CommunityToolkit.WinUI.Helpers;
using Microsoft.UI;
using Microsoft.UI.Xaml;
using Windows.UI;

namespace Mipser.Windows.Controls.AssemblyEditBox;

/// <summary>
/// A collection of colors to use for syntax highlighting.
/// </summary>
public class AssemblySyntaxHighlightingTheme : DependencyObject
{
    /// <summary>
    /// A <see cref="DependencyProperty"/> for the <see cref="InstructionHighlightColor"/> property.
    /// </summary>
    public static readonly DependencyProperty InstructionHighlightColorProperty =
        DependencyProperty.Register(nameof(InstructionHighlightColor),
            typeof(Color),
            typeof(AssemblySyntaxHighlightingTheme),
            new PropertyMetadata(Colors.Transparent));

    /// <summary>
    /// A <see cref="DependencyProperty"/> for the <see cref="RegisterHighlightColor"/> property.
    /// </summary>
    public static readonly DependencyProperty RegisterHighlightColorProperty =
        DependencyProperty.Register(nameof(RegisterHighlightColor),
            typeof(Color),
            typeof(AssemblySyntaxHighlightingTheme),
            new PropertyMetadata(Colors.Transparent));

    /// <summary>
    /// A <see cref="DependencyProperty"/> for the <see cref="ImmediateHighlightColor"/> property.
    /// </summary>
    public static readonly DependencyProperty ImmediateHighlightColorProperty =
        DependencyProperty.Register(nameof(ImmediateHighlightColor),
            typeof(Color),
            typeof(AssemblySyntaxHighlightingTheme),
            new PropertyMetadata(Colors.Transparent));

    /// <summary>
    /// A <see cref="DependencyProperty"/> for the <see cref="ReferenceHighlightColor"/> property.
    /// </summary>
    public static readonly DependencyProperty ReferenceHighlightColorProperty =
        DependencyProperty.Register(nameof(ReferenceHighlightColor),
            typeof(Color),
            typeof(AssemblySyntaxHighlightingTheme),
            new PropertyMetadata(Colors.Transparent));

    /// <summary>
    /// A <see cref="DependencyProperty"/> for the <see cref="OperatorHighlightColor"/> property.
    /// </summary>
    public static readonly DependencyProperty OperatorHighlightColorProperty =
        DependencyProperty.Register(nameof(OperatorHighlightColor),
            typeof(Color),
            typeof(AssemblySyntaxHighlightingTheme),
            new PropertyMetadata(Colors.Transparent));

    /// <summary>
    /// A <see cref="DependencyProperty"/> for the <see cref="DirectiveHighlightColor"/> property.
    /// </summary>
    public static readonly DependencyProperty DirectiveHighlightColorProperty =
        DependencyProperty.Register(nameof(DirectiveHighlightColor),
            typeof(Color),
            typeof(AssemblySyntaxHighlightingTheme),
            new PropertyMetadata(Colors.Transparent));

    /// <summary>
    /// A <see cref="DependencyProperty"/> for the <see cref="StringHighlightColor"/> property.
    /// </summary>
    public static readonly DependencyProperty StringHighlightColorProperty =
        DependencyProperty.Register(nameof(StringHighlightColor),
            typeof(Color),
            typeof(AssemblySyntaxHighlightingTheme),
            new PropertyMetadata(Colors.Transparent));

    /// <summary>
    /// A <see cref="DependencyProperty"/> for the <see cref="CommentHighlightColor"/> property.
    /// </summary>
    public static readonly DependencyProperty CommentHighlightColorProperty =
        DependencyProperty.Register(nameof(CommentHighlightColor),
            typeof(Color),
            typeof(AssemblySyntaxHighlightingTheme),
            new PropertyMetadata(Colors.Transparent));

    /// <summary>
    /// A <see cref="DependencyProperty"/> for the <see cref="MacroHighlightColor"/> property.
    /// </summary>
    public static readonly DependencyProperty MacroHighlightColorProperty =
        DependencyProperty.Register(nameof(MacroHighlightColor),
            typeof(Color),
            typeof(AssemblySyntaxHighlightingTheme),
            new PropertyMetadata(Colors.Transparent));

    /// <summary>
    /// A <see cref="DependencyProperty"/> for the <see cref="InvalidInstructionHighlightColor"/> property.
    /// </summary>
    public static readonly DependencyProperty InvalidInstructionHighlightColorProperty =
        DependencyProperty.Register(nameof(InvalidInstructionHighlightColor),
            typeof(Color),
            typeof(AssemblySyntaxHighlightingTheme),
            new PropertyMetadata(Colors.Transparent));

    /// <summary>
    /// A <see cref="DependencyProperty"/> for the <see cref="ErrorUnderlineColor"/> property.
    /// </summary>
    public static readonly DependencyProperty ErrorUnderlineColorProperty =
        DependencyProperty.Register(nameof(ErrorUnderlineColor),
            typeof(Color),
            typeof(AssemblySyntaxHighlightingTheme),
            new PropertyMetadata(Colors.Transparent));

    /// <summary>
    /// A <see cref="DependencyProperty"/> for the <see cref="WarningUnderlineColor"/> property.
    /// </summary>
    public static readonly DependencyProperty WarningUnderlineColorProperty =
        DependencyProperty.Register(nameof(WarningUnderlineColor),
            typeof(Color),
            typeof(AssemblySyntaxHighlightingTheme),
            new PropertyMetadata(Colors.Transparent));

    /// <summary>
    /// A <see cref="DependencyProperty"/> for the <see cref="MessageUnderlineColor"/> property.
    /// </summary>
    public static readonly DependencyProperty MessageUnderlineColorProperty =
        DependencyProperty.Register(nameof(MessageUnderlineColor),
            typeof(Color),
            typeof(AssemblySyntaxHighlightingTheme),
            new PropertyMetadata(Colors.Transparent));

    /// <summary>
    /// Gets the default <see cref="AssemblySyntaxHighlightingTheme"/>.
    /// </summary>
    public static AssemblySyntaxHighlightingTheme Default => new()
    {
        // Syntax Highlight Colors
        InstructionHighlightColor = "#A7FA95".ToColor(),
        RegisterHighlightColor = "#FE8482".ToColor(),
        ImmediateHighlightColor = "#F8FC8B".ToColor(),
        ReferenceHighlightColor = "#73EEFD".ToColor(),
        OperatorHighlightColor = "#77A7FD".ToColor(),
        DirectiveHighlightColor = "#FA9EF6".ToColor(),
        StringHighlightColor = "#FFC47A".ToColor(),
        CommentHighlightColor = "#9B88FC".ToColor(),
        MacroHighlightColor = "#BF472F".ToColor(),
        InvalidInstructionHighlightColor = "#67995c".ToColor(),

        // Indicator Colors
        ErrorUnderlineColor = "#FF0000".ToColor(),
        WarningUnderlineColor = "#FFAA00".ToColor(),
        MessageUnderlineColor = "#00AAFF".ToColor(),
    };

    /// <summary>
    /// Gets or sets the color to use for highlighting instruction tokens.
    /// </summary>
    public Color InstructionHighlightColor
    {
        get => (Color)GetValue(InstructionHighlightColorProperty);
        set => SetValue(InstructionHighlightColorProperty, value);
    }

    /// <summary>
    /// Gets or sets the color to use for highlighting register tokens.
    /// </summary>
    public Color RegisterHighlightColor
    {
        get => (Color)GetValue(RegisterHighlightColorProperty);
        set => SetValue(RegisterHighlightColorProperty, value);
    }

    /// <summary>
    /// Gets or sets the color to use for highlighting immediate value tokens.
    /// </summary>
    public Color ImmediateHighlightColor
    {
        get => (Color)GetValue(ImmediateHighlightColorProperty);
        set => SetValue(ImmediateHighlightColorProperty, value);
    }

    /// <summary>
    /// Gets or sets the color to use for highlighting reference tokens.
    /// </summary>
    public Color ReferenceHighlightColor
    {
        get => (Color)GetValue(ReferenceHighlightColorProperty);
        set => SetValue(ReferenceHighlightColorProperty, value);
    }

    /// <summary>
    /// Gets or sets the color to use for highlighting operator tokens.
    /// </summary>
    public Color OperatorHighlightColor
    {
        get => (Color)GetValue(OperatorHighlightColorProperty);
        set => SetValue(OperatorHighlightColorProperty, value);
    }

    /// <summary>
    /// Gets or sets the color to use for highlighting directive tokens.
    /// </summary>
    public Color DirectiveHighlightColor
    {
        get => (Color)GetValue(DirectiveHighlightColorProperty);
        set => SetValue(DirectiveHighlightColorProperty, value);
    }

    /// <summary>
    /// Gets or sets the color to use for highlighting string tokens.
    /// </summary>
    public Color StringHighlightColor
    {
        get => (Color)GetValue(StringHighlightColorProperty);
        set => SetValue(StringHighlightColorProperty, value);
    }

    /// <summary>
    /// Gets or sets the color to use for highlighting comment tokens.
    /// </summary>
    public Color CommentHighlightColor
    {
        get => (Color)GetValue(CommentHighlightColorProperty);
        set => SetValue(CommentHighlightColorProperty, value);
    }

    /// <summary>
    /// Gets or sets the color to use for highlighting macro tokens.
    /// </summary>
    public Color MacroHighlightColor
    {
        get => (Color)GetValue(MacroHighlightColorProperty);
        set => SetValue(MacroHighlightColorProperty, value);
    }

    /// <summary>
    /// Gets or sets the color to use for highlighting instruction tokens for invalid instructions.
    /// </summary>
    public Color InvalidInstructionHighlightColor
    {
        get => (Color)GetValue(InvalidInstructionHighlightColorProperty);
        set => SetValue(InvalidInstructionHighlightColorProperty, value);
    }

    /// <summary>
    /// Gets or sets the color to use for underlining errors.
    /// </summary>
    public Color ErrorUnderlineColor
    {
        get => (Color)GetValue(ErrorUnderlineColorProperty);
        set => SetValue(ErrorUnderlineColorProperty, value);
    }

    /// <summary>
    /// Gets or sets the color to use for underlining warnings.
    /// </summary>
    public Color WarningUnderlineColor
    {
        get => (Color)GetValue(WarningUnderlineColorProperty);
        set => SetValue(WarningUnderlineColorProperty, value);
    }

    /// <summary>
    /// Gets or sets the color to use for underlining messages.
    /// </summary>
    public Color MessageUnderlineColor
    {
        get => (Color)GetValue(MessageUnderlineColorProperty);
        set => SetValue(MessageUnderlineColorProperty, value);
    }
}
