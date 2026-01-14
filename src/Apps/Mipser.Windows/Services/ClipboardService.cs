// Avishai Dernis 2025

using Mipser.Services;
using System;
using System.Threading.Tasks;
using Windows.ApplicationModel.DataTransfer;
using Windows.Storage;

namespace Mipser.Windows.Services;

/// <summary>
/// An implementation of the <see cref="IClipboardService"/>
/// </summary>
public class ClipboardService : IClipboardService
{
    /// <inheritdoc/>
    public void CopyText(string text, bool flush = true)
    {
        var package = new DataPackage();
        package.SetText(text);
        SetClipboard(package, DataPackageOperation.Copy, flush);
    }

    /// <inheritdoc/>
    public async Task CutFileAsync(string filePath, bool flush = true)
        => await ClipFileAsync(filePath, DataPackageOperation.Move, flush);

    /// <inheritdoc/>
    public async Task CopyFileAsync(string filePath, bool flush = true)
        => await ClipFileAsync(filePath, DataPackageOperation.Copy, flush);

    private static void SetClipboard(DataPackage data, DataPackageOperation operation, bool flush = true)
    {
        data.RequestedOperation = operation;
        Clipboard.SetContent(data);

        // Flush?
        if (flush)
        {
            Clipboard.Flush();
        }
    }
}
