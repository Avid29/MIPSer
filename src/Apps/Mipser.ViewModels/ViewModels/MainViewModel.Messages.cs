// Avishai Dernis 2025

using CommunityToolkit.Mvvm.DependencyInjection;
using CommunityToolkit.Mvvm.Messaging;
using Mipser.Bindables.Files;
using Mipser.Messages.Build;
using Mipser.Messages.Files;
using Mipser.Messages.Navigation;
using Mipser.Messages.Pages;
using Mipser.ViewModels.Pages;
using System.Linq;
using System.Threading.Tasks;

namespace Mipser.ViewModels;

public partial class MainViewModel
{
    /// <inheritdoc/>
    protected override void OnActivated()
    {
        RegisterBuildMessages();
        RegisterFileMessages();
        RegisterNavigationMessages();
    }

    private void RegisterBuildMessages()
    {
        _messenger.Register<MainViewModel, AssembleFileRequestMessage>(this, async (r, _) =>
        {
            // Get current file if possible, return if not
            var file = r.CurrentFilePage?.File;
            if (file is null)
                return;

            // Build the current file
            await _buildService.AssembleFileAsync(file);
        });
    }

    private void RegisterFileMessages()
    {
        _messenger.Register<MainViewModel, FileCreateNewRequestMessage>(this, (r, m) => r.CreateNewFile());
        _messenger.Register<MainViewModel, FileOpenRequestMessage>(this, (r, m) => r.OpenFile(m.File));
        _messenger.Register<MainViewModel, FilePickAndOpenRequestMessage>(this, (r, m) => _ = r.PickAndOpenFileAsync());
        _messenger.Register<MainViewModel, FileSaveRequestMessage>(this, (r, m) => r.CurrentFilePage?.Save());
        _messenger.Register<MainViewModel, PageCloseRequestMessage>(this, (r, m) => r.FocusedPanel?.ClosePage(m.Page));
        _messenger.Register<MainViewModel, FolderPickAndOpenRequestMessage>(this, (r, m) => _ = r.PickAndOpenFolderAsync());

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

    /// <summary>
    /// Creates and opens a new anonymous file.
    /// </summary>
    public void CreateNewFile() => OpenFile(null);

    /// <summary>
    /// Picks and opens a file.
    /// </summary>
    public async Task PickAndOpenFileAsync()
    {
        // Select the file to open
        var file = await _fileService.PickFileAsyc();
        if (file is null)
            return;
        
        OpenFile(file);
    }
    
    /// <summary>
    /// Picks and opens a folder.
    /// </summary>
    public async Task PickAndOpenFolderAsync()
    {
        // Select the folder to open
        var folder = await _fileService.PickFolderAsync();
        if (folder is null)
            return;

        OpenFolder(folder);
    }

    private void OpenFile(BindableFile? file)
    {
        // Create anonymous file if needed
        file ??= _fileService.GetAnonymousFile();

        // Create page view model
        var page = Ioc.Default.GetRequiredService<FilePageViewModel>();
        page.File = file;

        // Open the page
        FocusedPanel?.OpenPage(page);
    }

    private void OpenFolder(BindableFolder folder)
    {
        // TODO: Move to ProjectService

        _ = _cacheService.CacheAsync("OpenFolder", folder.Path);
        _messenger.Send(new FolderOpenedMessage(folder));
    }
}
