// Avishai Dernis 2025

using CommunityToolkit.Mvvm.DependencyInjection;
using Microsoft.UI.Xaml.Controls;
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

        this.DataContext = Ioc.Default.GetRequiredService<StatusViewModel>();
    }

    private StatusViewModel ViewModel => (StatusViewModel)DataContext;
}
