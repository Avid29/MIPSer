// Adam Dernis 2024

using Zarem.Models.Files;
using Zarem.Services.Files;
using Zarem.Services.Files.Models;
using System.Collections.ObjectModel;

namespace Zarem.Bindables.Files;

/// <summary>
/// A file in the content view or explorer.
/// </summary>
public partial class BindableFile : BindableFileItem<IFile>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="BindableFile"/> class.
    /// </summary>
    internal BindableFile(FileService fileService, IFile file) : base(fileService)
    {
        FileItem = file;
        Children = [];

        OpenCommand = new(Open);
    }

    /// <inheritdoc/>
    /// <remarks>
    /// This usually means 
    /// </remarks>
    public override ObservableCollection<BindableFileItem> Children { get; }

    /// <inheritdoc/>
    protected internal override IFile FileItem
    {
        get;
        set
        {
            if (SetProperty(ref field, value))
            {
                OnPropertyChanged(nameof(Name));
                OnPropertyChanged(nameof(Path));
            }
        }
    }

    /// <summary>
    /// Gets the associate <see cref="SourceFile"/>.
    /// </summary>
    public SourceFile? SourceFile { get; init; }

    internal void TrackAsChild(BindableFileItem child)
    {
        Children.Add(child);
    }

    internal void UntrackChild(BindableFileItem child)
    {
        Children.Remove(child);
    }

    /// <inheritdoc/>
    public override void Dispose()
    {
    }
}
