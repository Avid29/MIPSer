// Adam Dernis 2023

using CommunityToolkit.Diagnostics;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml.Controls;
using Mipser.Bindables;
using Mipser.ViewModels.Views;

namespace Mipser.Windows.Views.Shell;

/// <summary>
/// The main content tab view.
/// </summary>
public sealed partial class OpenFilesView : UserControl
{
    /// <summary>
    /// Initializes a new instance of the <see cref="OpenFilesView"/> class.
    /// </summary>
    public OpenFilesView()
    {
        this.InitializeComponent();
        DataContext = App.Current.Services.GetRequiredService<OpenFilesViewModel>();
    }

    private OpenFilesViewModel ViewModel => (OpenFilesViewModel)DataContext;

    private void TabView_TabCloseRequested(TabView sender, TabViewTabCloseRequestedEventArgs args)
    {
        if (args.Item is not BindableFile)
            ThrowHelper.ThrowInvalidDataException();


    }
}
