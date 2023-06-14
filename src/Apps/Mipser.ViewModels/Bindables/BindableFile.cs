// Adam Dernis 2023

using CommunityToolkit.Mvvm.ComponentModel;
using Mipser.Services.Files.Models;
using System.IO;
using System.Threading.Tasks;

namespace Mipser.Bindables;

/// <summary>
/// An file in the content view.
/// </summary>
public class BindableFile : ObservableObject
{
    private readonly IFile? _file;
    private string? _contents;

    /// <summary>
    /// Initializes a new instance of the <see cref="BindableFile"/> class.
    /// </summary>
    public BindableFile()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="BindableFile"/> class.
    /// </summary>
    public BindableFile(IFile file)
    {
        _file = file;
        _ = LoadContent();
    }

    /// <summary>
    /// Gets the name of the file.
    /// </summary>
    public string Name => _file?.Name ?? "NewFile"; // TODO: Localize

    /// <summary>
    /// Gets if the file exists in storage, or just in memory.
    /// </summary>
    public bool IsAnonymous => _file is null;

    /// <summary>
    /// Gets file contents.
    /// </summary>
    public string? Contents
    {
        get => _contents;
        set => SetProperty(ref _contents, value);
    }

    private async Task LoadContent()
    {
        if (_file is null)
            return;

        await using var stream = await _file.OpenStreamForReadAsync();
        using var reader = new StreamReader(stream);
        Contents = await reader.ReadToEndAsync();
    }
}
