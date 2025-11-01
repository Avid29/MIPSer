// Adam Dernis 2024

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.DependencyInjection;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Mipser.Messages.Files;
using Mipser.Messages.Navigation;
using Mipser.Messages.Pages;
using Mipser.ViewModels.Views;
using System.Linq;

namespace Mipser.ViewModels;

/// <summary>
/// The view model for the root window.
/// </summary>
public partial class WindowViewModel : ObservableRecipient
{
    private readonly IMessenger _messenger;

    /// <summary>
    /// Initializes a new instance of the <see cref="WindowViewModel"/> class.
    /// </summary>
    /// <param name="messenger"></param>
    public WindowViewModel(IMessenger messenger)
    {
        _messenger = messenger;

        MainPanel = Ioc.Default.GetRequiredService<PanelViewModel>();

        CreateNewFileCommand = new RelayCommand(CreateNewFile);
        PickAndOpenFileCommand = new RelayCommand(PickAndOpenFile);
        PickAndOpenFolderCommand = new RelayCommand(PickAndOpenFolder);
        CloseFileCommand = new RelayCommand(CloseFile);
        OpenCheatSheetCommand = new RelayCommand(OpenCheatSheet);

        IsActive = true;
    }
    
    /// <inheritdoc/>
    protected override void OnActivated()
    {
        _messenger.Register<WindowViewModel, OpenCheatSheetRequestMessage>(this, (r, m) =>
        {
            // Check if the cheat sheet is already open, and open it if not.
            var page = r.MainPanel.OpenPages.FirstOrDefault(p => p is CheatSheetViewModel);
            if (page is null)
            {
                page = Ioc.Default.GetRequiredService<CheatSheetViewModel>();
                r.MainPanel.OpenPages.Add(page);
            }

            // Navigate to the cheat sheet.
            r.MainPanel.CurrentPage = page;
        });
        _messenger.Register<WindowViewModel, FileCreateNewRequestMessage>(this, (r, m) => r.MainPanel.CreateNewFile());
        _messenger.Register<WindowViewModel, FilePickAndOpenRequestMessage>(this, (r, m) => _ = r.MainPanel.PickAndOpenFileAsync());
        _messenger.Register<WindowViewModel, PageCloseRequestMessage>(this, (r, m) => r.MainPanel.ClosePage(m.Page));
    }
}
