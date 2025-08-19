// Avishai Dernis 2025

using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Mipser.Models.CheatSheet;

namespace Mipser.Windows.Selector;

public partial class EncodingTemplateConverter : DataTemplateSelector
{
    protected override DataTemplate? SelectTemplateCore(object item, DependencyObject container)
    {
        if (NarrowTemplate is null || DefaultTemplate is null)
            return null;

        if (item is not EncodingSection es)
            return null;

        return es.BitCount switch
        {
            1 => NarrowTemplate,
            _ => DefaultTemplate,
        };
    }

    public DataTemplate? DefaultTemplate { get; set; }

    public DataTemplate? NarrowTemplate { get; set; }
}
