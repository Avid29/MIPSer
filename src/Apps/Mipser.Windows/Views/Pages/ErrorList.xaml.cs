// Avishai Dernis 2025

using CommunityToolkit.Mvvm.DependencyInjection;
using Microsoft.UI.Xaml.Controls;
using Mipser.ViewModels.Pages;

namespace Mipser.Windows.Views.Pages;

/// <summary>
/// A viewer for files.
/// </summary>
public sealed partial class ErrorList : UserControl
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ErrorList"/> class.
    /// </summary>
    public ErrorList()
    {
        this.InitializeComponent();
        DataContext = Ioc.Default.GetRequiredService<ErrorListViewModel>();
    }

    private ErrorListViewModel ViewModel => (ErrorListViewModel)this.DataContext;
}
