// Avishai Dernis 2025

using Mipser.Services;
using Windows.ApplicationModel.DataTransfer;

namespace Mipser.Windows.Services;

/// <summary>
/// An implementation of the <see cref="IClipboardService"/>
/// </summary>
public class ClipboardService : IClipboardService
{
    /// <inheritdoc/>
    public void Copy(string text, bool flush = true)
    {
        var package = new DataPackage();
        package.SetText(text);
        Copy(package, flush);
    }

    private static void Copy(DataPackage data, bool flush = true)
    {
        // Set content
        data.RequestedOperation = DataPackageOperation.Copy;
        Clipboard.SetContent(data);
        
        // Flush?
        if (flush)
        {
            Clipboard.Flush();
        }
    }
}
