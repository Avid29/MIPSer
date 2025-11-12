// Avishai Dernis 2025

using CommunityToolkit.Mvvm.DependencyInjection;
using CommunityToolkit.Mvvm.Messaging;
using MIPS.Assembler.Tokenization.Models;
using Mipser.Bindables.Files;
using Mipser.Messages.Build;
using Mipser.Messages.Editor;
using Mipser.Messages.Editor.Enums;
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
        RegisterFileMessages();
        RegisterEditMessages();
        RegisterBuildMessages();
        RegisterNavigationMessages();
    }

    private void RegisterFileMessages()
    {
        _messenger.Register<MainViewModel, FileCreateNewRequestMessage>(this, (r, m) => r.CreateNewFile());
        _messenger.Register<MainViewModel, FileOpenRequestMessage>(this, (r, m) => r.OpenFile(m.File));
        _messenger.Register<MainViewModel, FilePickAndOpenRequestMessage>(this, (r, m) => _ = r.PickAndOpenFileAsync());
        _messenger.Register<MainViewModel, FileSaveRequestMessage>(this, (r, m) => r.FocusedPanel?.SaveCurrentFile());
        _messenger.Register<MainViewModel, PageCloseRequestMessage>(this, (r, m) => r.FocusedPanel?.ClosePage(m.Page));
        _messenger.Register<MainViewModel, FolderPickAndOpenRequestMessage>(this, (r, m) => _ = r.PickAndOpenFolderAsync());
    }

    private void RegisterEditMessages()
    {
        _messenger.Register<MainViewModel, EditorOperationRequestMessage>(this, (r, m) => r.ApplyEditorOperation(m.Operation));
    }

    private void RegisterBuildMessages()
    {
        _messenger.Register<MainViewModel, AssembleFilesRequestMessage>(this, async (r, m) =>
        {
            // Get current file if possible, return if not
            if (m.Files is null)
                return;

            // Build the file
            await _buildService.AssembleFilesAsync(m.Files);
        });
    }

    private void RegisterNavigationMessages()
    {
        _messenger.Register<MainViewModel, PanelFocusChangedMessage>(this, (r, m) => r.FocusedPanel = m.FocusedPanel);
        _messenger.Register<MainViewModel, NavigateToTokenRequestMessage>(this, (r, m) => _ = r.NavigateToToken(m.Target));
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
        var file = await _fileService.PickFileAsync();
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

        _projectService.OpenFolder(folder);
    }

    private FilePageViewModel OpenFile(BindableFile? file, bool reopen = false)
    {
        // Create anonymous file if needed
        file ??= _fileService.GetAnonymousFile();

        // Check for existing page
        FilePageViewModel? page = null;
        if (!reopen)
        {
            page = FocusedPanel?.OpenPages.
                OfType<FilePageViewModel>()
                .FirstOrDefault(x => x.File == file);
        }

        // Create page view model if needed
        if (page is null)
        {
            page = Ioc.Default.GetRequiredService<FilePageViewModel>();
            page.File = file;
        }

        // Open the page
        FocusedPanel?.OpenPage(page);
        return page;
    }

    private async Task NavigateToToken(Token token)
    {
        // Get the file path
        var path = token.FilePath;
        if (path is null)
            return;

        // Get the file
        var file = await _fileService.GetFileAsync(path);
        if (file is null)
            return;

        // Open the file
        var page = OpenFile(file);

        // Navigate to the token
        page.NavigateToToken(token);
    }

    private void ApplyEditorOperation(EditorOperation operation)
    {
        if (FocusedPanel?.CurrentPage is not FilePageViewModel filePage)
            return;

        filePage.ApplyOperation(operation);
    }
}
