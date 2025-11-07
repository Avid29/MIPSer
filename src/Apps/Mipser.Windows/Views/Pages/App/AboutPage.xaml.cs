// Avishai Dernis 2025

using Microsoft.UI.Xaml.Controls;
using Mipser.ViewModels.Pages.App;

namespace Mipser.Windows.Views.Pages.App;

/// <summary>
/// A viewer for files.
/// </summary>
public sealed partial class AboutPage : UserControl
{
    /// <summary>
    /// Initializes a new instance of the <see cref="FileViewer"/> class.
    /// </summary>
    public AboutPage()
    {
        this.InitializeComponent();
    }

    public AboutPageViewModel? ViewModel { get; set; }
}
