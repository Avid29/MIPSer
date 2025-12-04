// Adam Dernis 2024

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.DependencyInjection;
using Mipser.Services;
using Mipser.Services.Files;
using Mipser.Services.Files.Models;
using System.Collections.ObjectModel;

namespace Mipser.Bindables.Files.Abstract;

/// <summary>
/// A <see cref="IFileItem"/> in the explorer.
/// </summary>
public abstract class BindableFileItem : ObservableObject
{
    /// <summary>
    /// Initializes a new instance of the <see cref="BindableFileItem"/> class.
    /// </summary>
    protected BindableFileItem(FileService fileService)
    {
        FileService = fileService;
    }

    /// <summary>
    /// Gets the FileService that owns the file.
    /// </summary>
    protected FileService FileService { get; }

    /// <summary>
    /// Gets the name of the file.
    /// </summary>
    public abstract string Name { get; }

    /// <summary>
    /// Gets the file's path.
    /// </summary>
    public abstract string? Path { get; }

    /// <summary>
    /// Gets the child items.
    /// </summary>
    public abstract ObservableCollection<BindableFileItem> Children { get; }
}
