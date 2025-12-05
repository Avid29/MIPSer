// Avishai Dernis 2025

using Microsoft.UI.Xaml.Controls;
using Mipser.Services;
using Mipser.ViewModels;

namespace Mipser.Windows.Views.Shell;

/// <summary>
/// The status bar.
/// </summary>
public sealed partial class StatusBar : UserControl
{
    public StatusBar()
    {
        this.InitializeComponent();

        this.DataContext = Service.Get<StatusViewModel>();
    }

    private StatusViewModel ViewModel => (StatusViewModel)DataContext;
}
