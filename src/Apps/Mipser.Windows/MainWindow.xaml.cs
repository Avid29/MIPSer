// Adam Dernis 2024

using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml;
using Mipser.ViewModels;

namespace Mipser.Windows;

/// <summary>
/// An empty window that can be used on its own or navigated to within a Frame.
/// </summary>
public sealed partial class MainWindow : Window
{
    /// <summary>
    /// Initializes a new instance of the <see cref="MainWindow"/> class.
    /// </summary>
    public MainWindow()
    {
        this.InitializeComponent();

        ViewModel = App.Current.Services.GetRequiredService<WindowViewModel>();
    }

    private WindowViewModel ViewModel { get; }
}
