// Avishai Dernis 2025

using System.Collections.Generic;
using System.IO;

namespace Mipser.Models;

/// <summary>
/// A collection of source files in a project.
/// </summary>
public class SourceCollection
{
    private readonly Dictionary<string, SourceFile> _files;
    private readonly FileSystemWatcher _watcher;

    /// <summary>
    /// Initializes a new instance of the <see cref="SourceCollection"/> class.
    /// </summary>
    public SourceCollection(string rootFolder)
    {
        _files = [];
        _watcher = new(rootFolder);

        RootPath = rootFolder;

        // Subscribe to changes first to avoid a desync
        SubscribeToFileSystem();
        InitializeCollection();
    }

    /// <summary>
    /// Gets the root path of the source collection.
    /// </summary>
    public string RootPath { get; }

    private void InitializeCollection()
    {
        // Search for all ".asm" files
        var files = Directory.GetFiles(RootPath, "*.asm", SearchOption.AllDirectories);

        foreach (var file in files)
        {
            var sourceFile = new SourceFile(this, file);
            Track(sourceFile);
        }
    }

    private void SubscribeToFileSystem()
    {
        // Unsubscribe in case already subscribed
        UnSubscribeFromFileSystem();

        // Subscribe to events
        _watcher.Created += OnFileCreated;
        _watcher.Renamed += OnFileRenamed;
        _watcher.Deleted += OnFileDeleted;
        _watcher.Changed += OnFileChanged;

        // Enable event rising
        _watcher.EnableRaisingEvents = true;
    }

    private void UnSubscribeFromFileSystem()
    {
        // Unsubscribe from events
        _watcher.Created -= OnFileCreated;
        _watcher.Renamed -= OnFileRenamed;
        _watcher.Deleted -= OnFileDeleted;
        _watcher.Changed -= OnFileChanged;

        // Disabled event rising
        _watcher.EnableRaisingEvents = false;
    }

    private void OnFileCreated(object sender, FileSystemEventArgs e)
    {
        var extension = Path.GetExtension(e.FullPath);
        if (extension is not ".asm")
            return;

        var sourceFile = new SourceFile(this, e.FullPath);
        Track(sourceFile);
    }

    private void OnFileRenamed(object sender, RenamedEventArgs e)
    {
        // TODO: Handle rename
    }

    private void OnFileDeleted(object sender, FileSystemEventArgs e)
    {
        _files.Remove(e.FullPath);
    }

    private void OnFileChanged(object sender, FileSystemEventArgs e)
    {
        var extension = Path.GetExtension(e.FullPath);
        if (extension is not ".asm")
            return;

        // TODO: Handle dirty
    }

    private void Track(SourceFile file)
    {
        _files.TryAdd(file.FullPath, file);
    }
}
