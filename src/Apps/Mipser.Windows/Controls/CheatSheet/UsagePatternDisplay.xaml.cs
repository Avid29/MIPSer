// Avishai Dernis 2025

using CommunityToolkit.Diagnostics;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Documents;
using Microsoft.UI.Xaml.Media;
using MIPS.Assembler.Helpers.Tables;
using MIPS.Assembler.Models.Instructions;
using MIPS.Assembler.Tokenization;
using MIPS.Assembler.Tokenization.Enums;
using MIPS.Extensions;
using MIPS.Models.Instructions.Enums;
using MIPS.Models.Instructions.Enums.Registers;
using Mipser.Services.Localization;
using Mipser.Windows.Helpers;
using System.Threading.Tasks;
using Windows.UI.Text;

namespace Mipser.Windows.Controls.CheatSheet;

/// <summary>
/// A control for displaying usage patterns with colored .
/// </summary>
public sealed partial class UsagePatternDisplay : UserControl
{
    private InstructionMetadata? _metadata;

    /// <summary>
    /// Initializes a new instance of the <see cref="UsagePatternDisplay"/> class.
    /// </summary>
    public UsagePatternDisplay()
    {
        InitializeComponent();
    }

    public InstructionMetadata? Metadata
    {
        get => _metadata;
        set
        {
            _metadata = value;
            UpdateDisplay();
        }
    }

    /// <summary>
    /// Gets or sets the brush palette used for displaying arguments in the usage pattern display.
    /// </summary>
    public InstructionTypeBrushPalette? InstructionBrushPalette { get; set; }

    /// <summary>
    /// Gets or sets the brush palette used for displaying arguments in the usage pattern display.
    /// </summary>
    public ArgumentBrushPalette? ArgumentBrushPalette { get; set; }

    private void UpdateDisplay()
    {
        if (!_metadata.HasValue)
            return;

        var localizer = App.Current.Services.GetRequiredService<ILocalizationService>();
        var data = _metadata.Value;

        UpdateNameDisplay(data, localizer);
        UpdateUsageDisplay(data.ArgumentPattern, localizer);
        UpdateBehaviorDisplay(data.Behavior, localizer);
    }

    private void UpdateNameDisplay(InstructionMetadata data, ILocalizationService localizer)
    {
        // Construct a new Paragraph with the instruction name
        var block = new Paragraph();

        bool isFormatted = false;
        var name = data.Name;
        if (name.EndsWith(".fmt"))
        {
            name = name[..^4]; // Remove the ".fmt" suffix
            isFormatted = true;
        }

        block.Inlines.Add(new Run
        {
            Text = name,
            Foreground = data.Type switch
            {
                InstructionType.BasicR => InstructionBrushPalette?.RType,
                InstructionType.BasicI => InstructionBrushPalette?.IType,
                InstructionType.BasicJ => InstructionBrushPalette?.JType,

                InstructionType.RegisterImmediate or
                InstructionType.RegisterImmediateBranch => InstructionBrushPalette?.RegImmediate,

                InstructionType.Special2R or
                InstructionType.Special3R => InstructionBrushPalette?.R2Type,

                InstructionType.Coproc0 or InstructionType.Coproc0C0 or
                InstructionType.Coproc0MFMC0 => InstructionBrushPalette?.CoProcessor0,

                InstructionType.Coproc1 or
                InstructionType.Float => InstructionBrushPalette?.CoProcessor1,

                // TODO: Pseudo Instructions
                InstructionType.Pseudo => throw new System.NotImplementedException(),
                _ => ThrowHelper.ThrowArgumentOutOfRangeException<SolidColorBrush?>(),
            },
        });

        if (isFormatted)
        {
            block.Inlines.Add(new Run
            {
                Text = ".fmt",
                Foreground = ArgumentBrushPalette?.FormatBrush,
            });
        }

        // Clear the existing blocks and add the new one
        NameTextBlock.Blocks.Clear();
        NameTextBlock.Blocks.Add(block);
    }

    private void UpdateUsageDisplay(Argument[] args, ILocalizationService localizer)
    {
        var usage = new Paragraph();
        for (int i = 0; i < args.Length; i++)
        {
            var arg = args[i];
            usage.Inlines.Add(CreateArgumentRun(arg, localizer));
            if (i != args.Length - 1)
            {
                usage.Inlines.Add(new Run { Text = ", " }); // Add comma between arguments
            }
        }

        UsagePatternTextBlock.Blocks.Clear();
        UsagePatternTextBlock.Blocks.Add(usage);
    }

    private void UpdateBehaviorDisplay(string? behavior, ILocalizationService localizer)
    {
        if (string.IsNullOrWhiteSpace(behavior))
        {
            BehaviorTextBlock.Visibility = Visibility.Collapsed;
            return;
        }
        BehaviorTextBlock.Visibility = Visibility.Visible;

        // Create a new Paragraph for the behavior text
        // Tokenize the behavior string
        var paragraph = new Paragraph();
        var tokens = Tokenizer.TokenizeLine(behavior, null, TokenizerMode.BehaviorExpression);

        foreach (var token in tokens.Tokens)
        {
            paragraph.Inlines.Add(CreateTokenRun(token, localizer));
        }

        BehaviorTextBlock.Blocks.Clear();
        BehaviorTextBlock.Blocks.Add(paragraph);
    }

    private Inline CreateTokenRun(Token token, ILocalizationService localizer)
    {
        // Handle strict argument tokens
        if (ArgumentTable.TryGetArgument(token.Source, out var arg))
            return CreateArgumentRun(arg, localizer);
        
        var run = new Run
        {
            Text = token.Source,
        };

        if (token.Type is TokenType.Register)
        {
            run.Foreground = ArgumentBrushPalette?.MiscArgBrush;
        }

        return run;
    }

    private Inline CreateArgumentRun(Argument arg, ILocalizationService localizer)
    {
        return arg switch
        {
            Argument.RS or Argument.RT or Argument.RD => new Run
            {
                Text = ArgumentTable.GetArgPatternString(arg),
                Foreground = ArgumentBrushPalette?.GPRegisterBrush,
            },
            Argument.FS or Argument.FT or Argument.FD or Argument.RT_Numbered => new Run
            {
                Text = ArgumentTable.GetArgPatternString(arg),
                Foreground = ArgumentBrushPalette?.CPRegisterBrush,
            },
            Argument.Immediate or Argument.Offset or Argument.Address or
            Argument.Shift or Argument.FullImmediate => new Run
            {
                Text = localizer[$"Usage_{ArgumentTable.GetArgPatternString(arg)}"],
                Foreground = ArgumentBrushPalette?.ImmediateValueBrush,
            },
            Argument.AddressBase => new Span
            {
                Inlines =
                {
                    new Run
                    {
                        Text = localizer[$"Usage_{ArgumentTable.GetArgPatternString(Argument.Offset)}"],
                        Foreground = ArgumentBrushPalette?.ImmediateValueBrush,
                    },
                    new Run { Text = "(" },
                    new Run
                    {
                        Text = ArgumentTable.GetArgPatternString(Argument.RS),
                        Foreground = ArgumentBrushPalette?.GPRegisterBrush,
                    },
                    new Run { Text = ")" },
                }
            },

            _ => throw new System.NotImplementedException(),
        };
    }
}
