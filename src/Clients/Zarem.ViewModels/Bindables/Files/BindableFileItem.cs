// Adam Dernis 2024

using CommunityToolkit.Mvvm.ComponentModel;
using Zarem.Services.Files;
using Zarem.Services.Files.Models;
using System;
using System.Collections.ObjectModel;

namespace Zarem.Bindables.Files;

/// <summary>
/// A <see cref="IFileItem"/> in the explorer.
/// </summary>
public abstract partial class BindableFileItem : ObservableObject, IDisposable
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
    /// Gets or sets the name of the file.
    /// </summary>
    public abstract string Name { get; set;  }

    /// <summary>
    /// Gets the file's path.
    /// </summary>
    public abstract string Path { get; }

    /// <summary>
    /// Gets the child items.
    /// </summary>
    public abstract ObservableCollection<BindableFileItem> Children { get; }

    /// <inheritdoc/>
    public abstract void Dispose();
}
