// Avishai Dernis 2025

using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Mipser.ViewModels.Pages;
using Mipser.ViewModels.Pages.App;

namespace Mipser.Windows.Selectors;

public partial class PageTemplateSelector : DataTemplateSelector
{
    /// <summary>
    /// Gets the <see cref="DataTemplate"/> for a <see cref="CheatSheetViewModel"/>."/>
    /// </summary>
    public DataTemplate? AboutPageTemplate { get; set; }

    /// <summary>
    /// Gets the <see cref="DataTemplate"/> for a <see cref="CheatSheetViewModel"/>."/>
    /// </summary>
    public DataTemplate? CheatSheetPageTemplate { get; set; }

    /// <summary>
    /// Gets the <see cref="DataTemplate"/> for a <see cref="FilePageViewModel"/>.
    /// </summary>
    public DataTemplate? FilePageTemplate { get; set; }

    /// <summary>
    /// Gets the <see cref="DataTemplate"/> for a <see cref="SettingsPageViewModel"/>."/>
    /// </summary>
    public DataTemplate? SettingsPageTemplate { get; set; }

    protected override DataTemplate? SelectTemplateCore(object item)
    {
        return item switch
        {
            AboutPageViewModel => AboutPageTemplate,
            CheatSheetViewModel => CheatSheetPageTemplate,
            FilePageViewModel => FilePageTemplate,
            SettingsPageViewModel => SettingsPageTemplate,
            _ => null,
        };
    }

    protected override DataTemplate? SelectTemplateCore(object item, DependencyObject container) => this.SelectTemplateCore(item);
}
