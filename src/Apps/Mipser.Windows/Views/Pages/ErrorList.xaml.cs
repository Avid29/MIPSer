// Avishai Dernis 2025

using CommunityToolkit.Mvvm.Messaging;
using Microsoft.UI.Xaml.Controls;
using MIPS.Assembler.Logging;
using MIPS.Assembler.Tokenization.Models;
using Mipser.Messages.Navigation;
using Mipser.Services;
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
        DataContext = Service.Get<ErrorListViewModel>();
    }

    private ErrorListViewModel ViewModel => (ErrorListViewModel)this.DataContext;

    private static string DisplayLine(SourceLocation? location)
    {
        // The location is null. Display nothing
        if (location is null)
        {
            return string.Empty;
        }

        // Get the line
        return $"{location.Value.Line}";
    }

    private void ListView_ItemClick(object sender, ItemClickEventArgs e)
    {
        if (e.ClickedItem is not AssemblerLog log)
            return;

        var messenger = Service.Get<IMessenger>();
        messenger.Send(new NavigateToTokenRequestMessage(log.Tokens[0]));
    }
}
