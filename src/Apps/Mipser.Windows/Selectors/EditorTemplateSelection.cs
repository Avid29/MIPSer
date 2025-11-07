// Avishai Dernis 2025

using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Mipser.ViewModels.Pages;
using System.IO;

namespace Mipser.Windows.Selectors;

public partial class EditorTemplateSelector : DataTemplateSelector
{
    /// <summary>
    /// Gets the <see cref="DataTemplate"/> for a text editor.
    /// </summary>
    public DataTemplate? TextEditorTemplate { get; set; }

    /// <summary>
    /// Gets the <see cref="DataTemplate"/> for a hex editor.
    /// </summary>
    public DataTemplate? HexEditorTemplate { get; set; }

    protected override DataTemplate? SelectTemplateCore(object item, DependencyObject container)
    {
        if (item is not FilePageViewModel filePage)
            return null;

        // Attempt to retrieve a path
        var path = filePage.File?.Path;
        if (path is null)
            return null;

        // Get file type
        var type = Path.GetExtension(path);

        // TODO: Make configurable
        return type switch
        {
            ".obj" => HexEditorTemplate,
            _ => TextEditorTemplate,
        };
    }
}
