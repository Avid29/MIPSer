// Adam Dernis 2024

using Microsoft.UI.Xaml.Controls;
using Mipser.Bindables.Files;

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

    private BindableFile ViewModel => (BindableFile)this.DataContext;
}
