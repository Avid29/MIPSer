// Avishai Dernis 2025

using System.Threading.Tasks;

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
    void CopyText(string text, bool flush = true);

    /// <summary>
    /// Copies a file to the clipboard as a cut operation.
    /// </summary>
    /// <param name="filePath">The path of the file to cut.</param>
    /// <param name="flush">Whether or not the data should persist outside the app.</param>
    Task CutFileAsync(string filePath, bool flush = true);

    /// <summary>
    /// Copies a file to the clipboard.
    /// </summary>
    /// <param name="filePath">The path of the folder to copy.</param>
    /// <param name="flush">Whether or not the data should persist outside the app.</param>
    Task CopyFileAsync(string filePath, bool flush = true);
}
