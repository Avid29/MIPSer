// Adam Dernis 2024

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.DependencyInjection;
using Mipser.Services.Files.Models;
using Mipser.Services.Localization;

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
    public string Name => Item?.Name ?? Ioc.Default.GetRequiredService<ILocalizationService>()["NewFile"];
}
