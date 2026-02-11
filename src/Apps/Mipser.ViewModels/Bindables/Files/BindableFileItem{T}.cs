// Adam Dernis 2024

using Mipser.Services;
using Mipser.Services.Files;
using Mipser.Services.Files.Models;
using System.Threading.Tasks;

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
    public override string Name
    {
        get => FileItem.Name;
        set => FileItem.RenameAsync(value);
    }

    /// <inheritdoc/>
    public override string Path => FileItem.Path;

    /// <inheritdoc/>
    public override async Task DeleteAsync() => await Service.Get<IFileSystemService>().DeleteFileItemAsync(FileItem);
}
