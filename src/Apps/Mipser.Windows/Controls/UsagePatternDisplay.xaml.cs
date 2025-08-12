// Avishai Dernis 2025

using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Documents;
using Microsoft.UI.Xaml.Media;
using MIPS.Extensions;
using MIPS.Models.Instructions.Enums;
using Mipser.Services.Localization;

namespace Mipser.Windows.Controls;

/// <summary>
/// A control for displaying usage patterns with colored .
/// </summary>
public sealed partial class UsagePatternDisplay : UserControl
{
    private Argument[]? _usagePattern;

    /// <summary>
    /// Initializes a new instance of the <see cref="UsagePatternDisplay"/> class.
    /// </summary>
    public UsagePatternDisplay()
    {
        InitializeComponent();
    }

    public Argument[]? UsagePattern
    {
        get => _usagePattern;
        set
        {
            _usagePattern = value;
            UpdateUsagePatternDisplay();
        }
    }

    /// <summary>
    /// Gets or sets the brush used for general-purpose registers in the usage pattern display.
    /// </summary>
    public SolidColorBrush? GPRegisterBrush { get; set; }

    /// <summary>
    /// Gets or sets the brush used for coprocessor registers in the usage pattern display.
    /// </summary>
    public SolidColorBrush? CPRegisterBrush { get; set; }

    /// <summary>
    /// Gets or sets the brush used for immediate values in the usage pattern display.
    /// </summary>
    public SolidColorBrush? ImmediateValueBrush { get; set; }

    /// <summary>
    /// Gets or sets the brush used for miscellaneous argument types in the usage pattern display.
    /// </summary>
    public SolidColorBrush? MiscArgBrush { get; set; }

    private void UpdateUsagePatternDisplay()
    {
        if (_usagePattern == null || _usagePattern.Length == 0)
            return;

        var localizer = App.Current.Services.GetRequiredService<ILocalizationService>();

        var block = new Paragraph();
        for (int i = 0; i < _usagePattern.Length; i++)
        {
            var item = _usagePattern[i];
            block.Inlines.Add(CreateArgumentRun(item, localizer));
            if (i != _usagePattern.Length - 1)
            {
                block.Inlines.Add(new Run { Text = ", " }); // Add comma between arguments
            }
        }
        UsagePatternTextBlock.Blocks.Add(block);
    }

    private Inline CreateArgumentRun(Argument arg, ILocalizationService localizer)
    {
        return arg switch
        {
            Argument.RS or Argument.RT or Argument.RD => new Run
            {
                Text = arg.GetArgPatternString(),
                Foreground = GPRegisterBrush,
            },
            Argument.FS or Argument.FT or Argument.FD or Argument.RT_Numbered => new Run
            {
                Text = arg.GetArgPatternString(),
                Foreground = CPRegisterBrush,
            },
            Argument.Immediate or Argument.Offset or Argument.Address or
            Argument.Shift or Argument.FullImmediate => new Run
            {
                Text = localizer[$"Usage_{arg.GetArgPatternString()}"],
                Foreground = ImmediateValueBrush,
            },
            Argument.AddressBase => new Span
            {
                Inlines =
                {
                    new Run
                    {
                        Text = localizer[$"Usage_{Argument.Offset.GetArgPatternString()}"],
                        Foreground = ImmediateValueBrush,
                    },
                    new Run { Text = "(" },
                    new Run
                    {
                        Text = Argument.RS.GetArgPatternString(),
                        Foreground = GPRegisterBrush,
                    },
                    new Run { Text = ")" },
                }
            },

            _ => throw new System.NotImplementedException(),
        };
    }
}
