// Adam Dernis 2024

using Mipser.Bindables.Files.Abstract;
using Mipser.Services.Files;
using Mipser.Services.Files.Models;
using System.IO;
using System.Threading.Tasks;

namespace Mipser.Bindables.Files;

/// <summary>
/// A file in the content view or explorer.
/// </summary>
public class BindableFile : BindableFilesItemBase
{
    private readonly IFile? _file;
    private string? _contents;
    private bool _isDirty;

    /// <summary>
    /// Initializes a new instance of the <see cref="BindableFile"/> class.
    /// </summary>
    internal BindableFile(FileService fileService) : base(fileService)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="BindableFile"/> class.
    /// </summary>
    internal BindableFile(FileService fileService, IFile file) : base(fileService)
    {
        _file = file;
    }

    /// <summary>
    /// Gets if the file exists in storage, or just in memory.
    /// </summary>
    public bool IsAnonymous => Item is null;

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
    /// Gets a <see cref="Stream"/> for reading the file contents.
    /// </summary>
    public async Task<Stream?> GetStreamAsync()
    {
        if (_file is null)
            return null;

        return await _file.OpenStreamForReadAsync();
    }

    /// <summary>
    /// Gets a value indicating whether the file has unsaved changes.
    /// </summary>
    public bool IsDirty
    {
        get => _isDirty;
        private set => SetProperty(ref _isDirty, value);
    }

    /// <inheritdoc/>
    protected override IFilesItem? Item => _file;

    /// <summary>
    /// Saves the file contents.
    /// </summary>
    public async Task SaveAsync() => await SaveContent();

    /// <summary>
    /// Loads the files content.
    /// </summary>
    public async Task LoadContent()
    {
        if (_file is null)
            return;

        await using var stream = await _file.OpenStreamForReadAsync();
        using var reader = new StreamReader(stream);
        Contents = await reader.ReadToEndAsync();
        IsDirty = false;
    }

    private async Task SaveContent()
    {
        if (_file is null)
            return;

        try
        {
            await using var stream = await _file.OpenStreamForWriteAsync();
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
}
