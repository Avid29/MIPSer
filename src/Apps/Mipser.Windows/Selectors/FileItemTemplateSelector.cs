// Adam Dernis 2024

using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Mipser.Bindables.Files;

namespace Mipser.Windows.Selectors;

/// <summary>
/// A <see cref="DataTemplateSelector"/> for 
/// </summary>
public partial class FileItemTemplateSelector : DataTemplateSelector
{
    /// <summary>
    /// Gets the <see cref="DataTemplate"/> for a <see cref="BindableFile"/>.
    /// </summary>
    public DataTemplate? BindableFileTemplate { get; set; }

    /// <summary>
    /// Gets the <see cref="DataTemplate"/> for a <see cref="BindableFolder"/>.
    /// </summary>
    public DataTemplate? BindableFolderTemplate { get; set; }

    /// <inheritdoc/>
    protected override DataTemplate? SelectTemplateCore(object item, DependencyObject container)
    {
        return item switch
        {
            BindableFile _ => BindableFileTemplate,
            BindableFolder _ => BindableFolderTemplate,
            _ => null,
        };
    }
}
