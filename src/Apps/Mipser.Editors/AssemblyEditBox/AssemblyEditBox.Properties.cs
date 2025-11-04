// Avishai Dernis 2025

using Microsoft.UI.Xaml;
using System;

namespace Mipser.Editors.AssemblyEditBox;

public partial class AssemblyEditBox
{
    /// <summary>
    /// A <see cref="DependencyProperty"/> for the <see cref="SelectedRange"/> property.
    /// </summary>
    public static readonly DependencyProperty SelectionRangeProperty =
        DependencyProperty.Register(nameof(SelectedRange), typeof(Range), typeof(AssemblyEditBox), new PropertyMetadata(new Range(0, 0)));

    /// <summary>
    /// Gets or sets the selected range.
    /// </summary>
    public Range SelectedRange
    {
        get => (Range)GetValue(SelectionRangeProperty);
        set => SetValue(SelectionRangeProperty, value);
    }
}
