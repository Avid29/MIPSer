// Adam Dernis 2024

using Mipser.Bindables.Files.Abstract;
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
    private string? _contents;
    private bool _isDirty;
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

    /// <summary>
    /// Gets file contents.
    /// </summary>
    public string? Contents
    {
        get => _contents;
        set
        {
            SetProperty(ref _contents, value);
            IsDirty = true;
        }
    }

    /// <summary>
    /// Gets a value indicating whether the file has unsaved changes.
    /// </summary>
    public bool IsDirty
    {
        get => _isDirty;
        private set => SetProperty(ref _isDirty, value);
    }

    /// <summary>
    /// Gets if the file exists in storage, or just in memory.
    /// </summary>
    public bool IsAnonymous => FileItem is null;

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

    /// <summary>
    /// Saves the file contents.
    /// </summary>
    public async Task SaveAsync() => await SaveContent();

    internal void TrackAsChild(BindableFileItem child)
    {
        Children.Add(child);
    }

    /// <summary>
    /// Loads the files content.
    /// </summary>
    public async Task LoadContent()
    {
        if (FileItem is null)
            return;

        await using var stream = await FileItem.OpenStreamForReadAsync();
        using var reader = new StreamReader(stream);
        Contents = await reader.ReadToEndAsync();
        IsDirty = false;
    }

    private async Task SaveContent()
    {
        if (FileItem is null)
            return;

        try
        {
            await using var stream = await FileItem.OpenStreamForWriteAsync();
            using var writer = new StreamWriter(stream);
            await writer.WriteAsync(Contents ?? string.Empty);
            stream.SetLength(stream.Position);
        }
        catch
        {
            // Ignore errors for now
            return;
        }
        IsDirty = false;
    }

    /// <inheritdoc/>
    public override void Dispose()
    {
    }
}
