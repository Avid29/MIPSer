// Adam Dernis 2023

using CommunityToolkit.Diagnostics;
using CommunityToolkit.Mvvm.DependencyInjection;
using CommunityToolkit.Mvvm.Messaging;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml.Controls;
using Mipser.Bindables.Files;
using Mipser.Messages.Files;
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

        // TODO: This is bad design. Fix.

        Ioc.Default.GetRequiredService<IMessenger>().Send(new FileCloseRequestMessage((BindableFile)args.Item));
    }
}
