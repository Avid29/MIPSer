// Adam Dernis 2024

using Zarem.Services;
using Zarem.Services.Files;
using Zarem.Services.Files.Models;
using System.Threading.Tasks;

namespace Zarem.Bindables.Files;

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
