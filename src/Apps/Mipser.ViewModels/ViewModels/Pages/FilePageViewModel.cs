// Avishai Dernis 2025

using CommunityToolkit.Diagnostics;
using CommunityToolkit.Mvvm.Messaging;
using MIPS.Assembler.Logging;
using MIPS.Assembler.Tokenization.Models;
using Mipser.Bindables.Files;
using Mipser.Messages.Build;
using Mipser.ViewModels.Pages.Abstract;
using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace Mipser.ViewModels.Pages;

/// <summary>
/// A view model for a file page.
/// </summary>
public class FilePageViewModel : PageViewModel
{
    private readonly IMessenger _messenger;

    /// <summary>
    /// An event invoked requesting to navigate to a token.
    /// </summary>
    public event EventHandler<SourceLocation>? NavigateToTokenEvent;

    /// <summary>
    /// An event invoked when the file is assembled.
    /// </summary>
    public event EventHandler<IReadOnlyList<ILog>>? AssembledEvent;

    private BindableFile? _file;

    /// <summary>
    /// Initializes a new instance of the <see cref="FilePageViewModel"/> class.
    /// </summary>
    public FilePageViewModel(IMessenger messenger)
    {
        _messenger = messenger;

        IsActive = true;
    }

    /// <inheritdoc/>
    public override string Title
    {
        get
        {
            Guard.IsNotNull(File);

            // Get name and append astrics if dirty
            var name = File.Name;
            if (File.IsDirty)
                name += " *";

            return name;
        }
    }
    
    /// <inheritdoc/>
    public override bool CanSave => true;
    
    /// <inheritdoc/>
    public override bool CanAssemble => true; // TODO: Check file type

    /// <summary>
    /// Gets the bindable file for this page.
    /// </summary>
    public BindableFile? File
    {
        get => _file;
        set => SetFile(value);
    }

    /// <inheritdoc/>
    protected override void OnActivated()
    {
        _messenger.Register<FilePageViewModel, BuildFinishedMessage>(this, (r, m) => r.OnBuildFinished(m.Logs));
    }

    /// <summary>
    /// Saves changes to the file.
    /// </summary>
    public async void Save()
    {
        // TODO: Save as dialog for anonymous files.
        if (File is null)
            return;

        await File.SaveAsync();
        OnPropertyChanged(Title);
    }

    /// <summary>
    /// Requests to navigate to a token.
    /// </summary>
    /// <param name="token">The token to navigate to.</param>
    public void NavigateToToken(Token token) => NavigateToTokenEvent?.Invoke(this, token.Location);

    private async void SetFile(BindableFile? file)
    {
        if (_file is not null)
        {
            _file.PropertyChanged -= OnFileUpdate;
        }

        _file = file;

        if (_file is not null)
        {
            if (_file.Contents is null)
                await _file.LoadContent();

            _file.PropertyChanged += OnFileUpdate;
        }
    }

    private void OnFileUpdate(object? sender, PropertyChangedEventArgs args)
    {
        if (args.PropertyName == nameof(BindableFile.Name) || args.PropertyName == nameof(BindableFile.IsDirty))
        {
            OnPropertyChanged(nameof(Title));
        }
    }

    private void OnBuildFinished(IReadOnlyList<ILog>? logs)
    {
        if (logs is null)
            return;

        AssembledEvent?.Invoke(this, logs);
    }
}
