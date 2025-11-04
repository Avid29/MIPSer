// Avishai Dernis 2025

using CommunityToolkit.Mvvm.DependencyInjection;
using CommunityToolkit.Mvvm.Messaging;
using Mipser.Messages.Build;
using Mipser.Messages.Files;
using Mipser.Messages.Navigation;
using Mipser.Messages.Pages;
using Mipser.ViewModels.Pages;
using System.Linq;

namespace Mipser.ViewModels;

public partial class MainViewModel
{
    /// <inheritdoc/>
    protected override void OnActivated()
    {
        RegisterFileMessages();
        RegisterNavigationMessages();
    }

    private void RegisterFileMessages()
    {
        _messenger.Register<MainViewModel, FileCreateNewRequestMessage>(this, (r, m) => r.FocusedPanel?.CreateNewFile());
        _messenger.Register<MainViewModel, FilePickAndOpenRequestMessage>(this, (r, m) => _ = r.FocusedPanel?.PickAndOpenFileAsync());
        _messenger.Register<MainViewModel, PageCloseRequestMessage>(this, (r, m) => r.FocusedPanel?.ClosePage(m.Page));
        _messenger.Register<MainViewModel, AssembleFileRequestMessage>(this, (r, m) => r.CurrentFilePage?.Assemble());
        _messenger.Register<MainViewModel, FileSaveRequestMessage>(this, (r, m) => r.CurrentFilePage?.Save());
    }

    private void RegisterNavigationMessages()
    {
        _messenger.Register<MainViewModel, PanelFocusChangedMessage>(this, (r, m) => r.FocusedPanel = m.FocusedPanel);
        _messenger.Register<MainViewModel, OpenCheatSheetRequestMessage>(this, (r, m) => r.NavigateToCheatSheet());
    }

    private void NavigateToCheatSheet(bool open = true)
    {
            // Check if the cheat sheet is already open, and open it if not.
            var page = FocusedPanel?.OpenPages.FirstOrDefault(p => p is CheatSheetViewModel);
            if (page is null && open)
            {
                page = Ioc.Default.GetRequiredService<CheatSheetViewModel>();
                FocusedPanel?.OpenPages.Add(page);
            }

            // Navigate to the cheat sheet.
            if (FocusedPanel is not null)
            {
                FocusedPanel.CurrentPage = page;
            }
    }
}
