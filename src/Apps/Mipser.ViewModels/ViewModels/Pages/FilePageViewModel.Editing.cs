// Avishai Dernis 2025

using System.IO;
using System.Threading.Tasks;

namespace Mipser.ViewModels.Pages;

public partial class FilePageViewModel
{
    private string? _originalContent;
    private string? _contents;

    /// <summary>
    /// Gets or sets the content of file.
    /// </summary>
    public string? Content
    {
        get => _contents;
        set
        {
            if (SetProperty(ref _contents, value))
            {
                OnPropertyChanged(nameof(IsDirty));
            }
        }
    }

    private string? OriginalContent
    {
        get => _originalContent;
        set
        {
            Content = value;
            if (SetProperty(ref _originalContent, value))
            {
                OnPropertyChanged(nameof(IsDirty));
            }
        }
    }

    private async Task LoadContentAsync()
    {
        if (File is null)
            return;

        await using var stream = await File.FileItem.OpenStreamForReadAsync();
        using var reader = new StreamReader(stream);
        OriginalContent = await reader.ReadToEndAsync();
    }

    /// <summary>
    /// Saves changes to the file.
    /// </summary>
    public override async Task SaveAsync()
    {
        // TODO: Save as dialog for anonymous files.
        if (File is null)
            return;

        try
        {
            await using var stream = await File.FileItem.OpenStreamForWriteAsync();
            using var writer = new StreamWriter(stream);
            await writer.WriteAsync(Content ?? string.Empty);
            stream.SetLength(stream.Position);
            OriginalContent = Content;
        }
        catch
        {
            // Ignore errors for now
            return;
        }
    }
}
