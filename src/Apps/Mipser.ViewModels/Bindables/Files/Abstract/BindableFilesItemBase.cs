// Adam Dernis 2023

using CommunityToolkit.Mvvm.ComponentModel;
using Mipser.Services.Files.Models;

namespace Mipser.Bindables.Files.Abstract;

/// <summary>
/// A <see cref="IFilesItem"/> in the explorer.
/// </summary>
public abstract class BindableFilesItemBase : ObservableObject
{
    /// <summary>
    /// The wrapped <see cref="IFilesItem"/>.
    /// </summary>
    protected abstract IFilesItem? Item { get; }

    /// <summary>
    /// Gets the name of the file.
    /// </summary>
    public string Name => Item?.Name ?? "NewFile"; // TODO: Localize
}
