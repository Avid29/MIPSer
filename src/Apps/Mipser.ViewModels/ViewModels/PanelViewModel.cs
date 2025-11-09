// Adam Dernis 2024

using CommunityToolkit.Mvvm.ComponentModel;
using Mipser.Bindables.Files;
using Mipser.Services.Files;
using Mipser.ViewModels.Pages;
using Mipser.ViewModels.Pages.Abstract;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace Mipser.ViewModels;

/// <summary>
/// A view model for tracking the open files.
/// </summary>
public class PanelViewModel : ObservableObject
{
    private PageViewModel? _currentPage;
     
    /// <summary>
    /// Initializes a new instance of the <see cref="PanelViewModel"/> class.
    /// </summary>
    public PanelViewModel()
    {
        OpenPages = [];
    }

    /// <summary>
    /// Gets or sets the currently selected file.
    /// </summary>
    public PageViewModel? CurrentPage
    {
        get => _currentPage;
        set
        {
            if (SetProperty(ref _currentPage, value))
            {
                OnPropertyChanged(nameof(IsPageOpen));
            }
        }
    }

    /// <summary>
    /// Gets a value indicating whether or not a page is open.
    /// </summary>
    public bool IsPageOpen => CurrentPage is not null;

    /// <summary>
    /// Gets an <see cref="ObservableCollection{T}"/> of open files.
    /// </summary>
    public ObservableCollection<PageViewModel> OpenPages { get; }

    /// <summary>
    /// Attempts to save the current file.
    /// </summary>
    /// <remarks>
    /// Does nothing if the current page is not a file.
    /// </remarks>
    public void SaveCurrentFile()
    {
        if (CurrentPage is not FilePageViewModel filePage)
            return;

        filePage.Save();
    }

    /// <summary>
    /// Opens a page.
    /// </summary>
    public void OpenPage(PageViewModel page)
    {
        // Open the page if needed
        if (!OpenPages.Contains(page))
            OpenPages.Add(page);

        CurrentPage = page;
    }

    /// <summary>
    /// Closes a page.
    /// </summary>
    /// <remarks>
    /// Does not save the file.
    /// </remarks>
    public void ClosePage(PageViewModel? page)
    {
        page ??= CurrentPage;
        if (page is null)
            return;

        OpenPages.Remove(page);
    }
}
