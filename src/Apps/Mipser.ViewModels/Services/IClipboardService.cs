// Avishai Dernis 2025

namespace Mipser.Services;

/// <summary>
/// An interface for a service to manage the clipboard.
/// </summary>
public interface IClipboardService
{
    /// <summary>
    /// Copies a string to the clipboard.
    /// </summary>
    /// <param name="text">The text to copy.</param>
    /// <param name="flush">Whether or not the data should persist outside the app.</param>
    void Copy(string text, bool flush = true);
}
