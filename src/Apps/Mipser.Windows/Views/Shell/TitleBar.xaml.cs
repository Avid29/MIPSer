// Adam Dernis 2024

using CommunityToolkit.Diagnostics;
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

        Loaded += TitleBar_Loaded;
    }

    private WindowViewModel ViewModel => (WindowViewModel)DataContext;

    private void TitleBar_Loaded(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
    {
        ExtendIntoTitleBar();
    }

    private void ExtendIntoTitleBar()
    {
        var window = App.Current.Window;
        Guard.IsNotNull(window);

        window.ExtendsContentIntoTitleBar = true;
        window.SetTitleBar(AppTitleBar);
    }
}
