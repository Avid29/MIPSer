// Adam Dernis 2024

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.DependencyInjection;
using Mipser.Services.Files;
using Mipser.Services.Files.Models;
using Mipser.Services.Localization;
using System.Collections.ObjectModel;

namespace Mipser.Bindables.Files.Abstract;

/// <summary>
/// A <see cref="IFilesItem"/> in the explorer.
/// </summary>
public abstract class BindableFilesItemBase : ObservableObject
{
    /// <summary>
    /// Initializes a new instance of the <see cref="BindableFilesItemBase"/> class.
    /// </summary>
    protected BindableFilesItemBase(FileService fileService)
    {
        FileService = fileService;
    }

    /// <summary>
    /// Gets the FileService that owns the file.
    /// </summary>
    protected FileService FileService { get; }

    /// <summary>
    /// The wrapped <see cref="IFilesItem"/>.
    /// </summary>
    protected abstract IFilesItem? Item { get; }

    /// <summary>
    /// Gets the name of the file.
    /// </summary>
    public string Name => Item?.Name ?? Ioc.Default.GetRequiredService<ILocalizationService>()["NewFile"];

    /// <summary>
    /// Gets the file's path.
    /// </summary>
    public string? Path => Item?.Path;

    /// <summary>
    /// Gets the child items.
    /// </summary>
    public abstract ObservableCollection<BindableFilesItemBase> Children { get; }
}
