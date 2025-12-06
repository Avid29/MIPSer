// Adam Dernis 2024

using Mipser.Services.Files;
using Mipser.Services.Files.Models;
using System.Collections.ObjectModel;
using System.IO;
using System.Threading.Tasks;

namespace Mipser.Bindables.Files;

/// <summary>
/// A file in the content view or explorer.
/// </summary>
public partial class BindableFile : BindableFileItem<IFile>
{
    private IFile _file;

    /// <summary>
    /// Initializes a new instance of the <see cref="BindableFile"/> class.
    /// </summary>
    internal BindableFile(FileService fileService, IFile file) : base(fileService)
    {
        _file = file;
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
        get => _file;
        set
        {
            if (SetProperty(ref _file, value))
            {
                OnPropertyChanged(nameof(Name));
                OnPropertyChanged(nameof(Path));
            }
        }
    }

    internal void TrackAsChild(BindableFileItem child)
    {
        Children.Add(child);
    }

    /// <inheritdoc/>
    public override void Dispose()
    {
    }
}
