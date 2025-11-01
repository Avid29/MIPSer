// Adam Dernis 2024

using Microsoft.UI.Xaml.Controls;
using Mipser.ViewModels.Pages;
using WinUIEditor;

namespace Mipser.Windows.Views;

/// <summary>
/// A viewer for files.
/// </summary>
public sealed partial class FileViewer : UserControl
{
    /// <summary>
    /// Initializes a new instance of the <see cref="FileViewer"/> class.
    /// </summary>
    public FileViewer()
    {
        this.InitializeComponent();
    }

    private FilePageViewModel ViewModel => (FilePageViewModel)this.DataContext;

    private void CodeEditorControl_Loaded(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
    {
        if (sender is CodeEditorControl editor)
        {
            editor.Editor.SetText(ViewModel.File.Contents);
        }
    }
}
