// Avishai Dernis 2025

using CommunityToolkit.Mvvm.DependencyInjection;
using CommunityToolkit.Mvvm.Messaging;
using Microsoft.UI.Xaml.Controls;
using MIPS.Assembler.Logging;
using Mipser.Messages.Navigation;
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

    private void ListView_ItemClick(object sender, ItemClickEventArgs e)
    {
        if (e.ClickedItem is not AssemblerLog log)
            return;

        var messenger = Ioc.Default.GetRequiredService<IMessenger>();
        messenger.Send(new NavigateToTokenRequestMessage(log.Tokens[0]));
    }
}
