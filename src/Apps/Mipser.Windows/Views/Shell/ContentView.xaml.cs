// Adam Dernis 2023

using CommunityToolkit.Diagnostics;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml.Controls;
using Mipser.Bindables;
using Mipser.ViewModels;

namespace Mipser.Windows.Views.Shell;

/// <summary>
/// The main content tab view.
/// </summary>
public sealed partial class ContentView : UserControl
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ContentView"/> class.
    /// </summary>
    public ContentView()
    {
        this.InitializeComponent();
        DataContext = App.Current.Services.GetRequiredService<WindowViewModel>();
    }

    private WindowViewModel ViewModel => (WindowViewModel)DataContext;

    private void TabView_TabCloseRequested(TabView sender, TabViewTabCloseRequestedEventArgs args)
    {
        if (args.Item is not BindableFile)
            ThrowHelper.ThrowInvalidDataException();

        ViewModel.CloseFile((BindableFile)args.Item);
    }
}
