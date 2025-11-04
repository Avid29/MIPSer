// Adam Dernis 2024

using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Mipser.ViewModels.Pages;

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

    private void AssemblyEditBox_TextChanged(object sender, RoutedEventArgs e)
    {
        
    }
}
