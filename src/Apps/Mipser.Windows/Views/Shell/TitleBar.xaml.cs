// Adam Dernis 2023

using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml.Controls;
using Mipser.ViewModels;

namespace Mipser.Windows.Views.Shell;

/// <summary>
/// The title bar contents.
/// </summary>
public sealed partial class TitleBar : UserControl
{
    /// <summary>
    /// Initializes a new instance of the <see cref="TitleBar"/> class.
    /// </summary>
    public TitleBar()
    {
        this.InitializeComponent();
        DataContext = App.Current.Services.GetRequiredService<WindowViewModel>();
    }

    private WindowViewModel ViewModel => (WindowViewModel)DataContext;
}
