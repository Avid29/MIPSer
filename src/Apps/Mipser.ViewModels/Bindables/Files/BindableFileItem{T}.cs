// Adam Dernis 2024

using Mipser.Services.Files;
using Mipser.Services.Files.Models;

namespace Mipser.Bindables.Files;

/// <summary>
/// A <see cref="IFileItem"/> in the explorer.
/// </summary>
public abstract class BindableFileItem<T> : BindableFileItem
    where T : IFileItem
{
    /// <summary>
    /// Initializes a new instance of the <see cref="BindableFileItem"/> class.
    /// </summary>
    protected BindableFileItem(FileService fileService) : base(fileService)
    {
    }

    /// <summary>
    /// The wrapped <see cref="IFileItem"/>.
    /// </summary>
    protected internal abstract T FileItem { get; set; }

    /// <inheritdoc/>
    public override string Name => FileItem.Name;

    /// <inheritdoc/>
    public override string Path => FileItem.Path;
}
