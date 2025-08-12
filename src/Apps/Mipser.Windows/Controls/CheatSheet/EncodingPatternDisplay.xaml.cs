// Avishai Dernis 2025

using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Mipser.Models.CheatSheet;

namespace Mipser.Windows.Controls.CheatSheet;

public sealed partial class EncodingPatternDisplay : UserControl
{
    private static readonly DependencyProperty EncodingPatternProperty =
        DependencyProperty.Register(nameof(EncodingPattern), typeof(EncodingPattern), typeof(EncodingPatternDisplay), new PropertyMetadata(null));

    public EncodingPatternDisplay()
    {
        InitializeComponent();
    }

    public EncodingPattern EncodingPattern
    {
        get => (EncodingPattern)GetValue(EncodingPatternProperty);
        set => SetValue(EncodingPatternProperty, value);
    }

    private double ConvertWidth(int bits)
    {
        return Container.ActualWidth * ((double)bits/32);
    }
}
