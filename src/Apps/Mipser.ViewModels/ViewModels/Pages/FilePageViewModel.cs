// Avishai Dernis 2025

using CommunityToolkit.Diagnostics;
using CommunityToolkit.Mvvm.Messaging;
using MIPS.Assembler.Logging;
using MIPS.Assembler.Tokenization.Models;
using Mipser.Bindables.Files;
using Mipser.Messages;
using Mipser.Messages.Build;
using Mipser.Messages.Editor.Enums;
using Mipser.Services.Settings;
using Mipser.Services.Settings.Enums;
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
    private readonly ISettingsService _settingsService;

    /// <summary>
    /// An event invoked requesting to navigate to a token.
    /// </summary>
    public event EventHandler<SourceLocation>? NavigateToTokenEvent;

    /// <summary>
    /// An event invoked when the file is assembled.
    /// </summary>
    public event EventHandler<IReadOnlyList<AssemblerLog>>? AssembledEvent;

    /// <summary>
    /// An event invoked when an editor operation is requested.
    /// </summary>
    public event EventHandler<EditorOperation>? EditorOperationRequested;

    private BindableFile? _file;

    /// <summary>
    /// Initializes a new instance of the <see cref="FilePageViewModel"/> class.
    /// </summary>
    public FilePageViewModel(IMessenger messenger, ISettingsService settingsService)
    {
        _messenger = messenger;
        _settingsService = settingsService;

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
    public override bool CanTextEdit => true;
    
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

    /// <summary>
    /// Gets whether or not the file should be assembled in real-time.
    /// </summary>
    public bool AssembleRealTime => _settingsService.Local.GetValue<bool>("RealTimeAssembly");
    

    /// <summary>
    /// Gets the threshold for showing logs as annotations.
    /// </summary>
    public AnnotationThreshold AnnotationThreshold => _settingsService.Local.GetValue<AnnotationThreshold>("AnnotationThreshold");

    /// <inheritdoc/>
    protected override void OnActivated()
    {
        _messenger.Register<FilePageViewModel, FileAssembledMessage>(this, (r, m) => r.OnBuildFinished(m.AssemblyFile, m.Logs));

        _messenger.Register<FilePageViewModel, SettingChangedMessage<bool>>(this, (r, m) =>
        {
            if (m.Key != "RealTimeAssembly")
                return;

            OnPropertyChanged(nameof(AssembleRealTime));
        });
        _messenger.Register<FilePageViewModel, SettingChangedMessage<AnnotationThreshold>>(this, (r, m) =>
        {
            if (m.Key != "AnnotationThreshold")
                return;

            OnPropertyChanged(nameof(AnnotationThreshold));
        });
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

    /// <summary>
    /// Requests an editor operation.
    /// </summary>
    /// <param name="operation">The editor operation requested.</param>
    public void ApplyOperation(EditorOperation operation) => EditorOperationRequested?.Invoke(this, operation);

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

    private void OnBuildFinished(string file, IReadOnlyList<AssemblerLog>? logs)
    {
        // Ensure the file matches
        if (file != _file?.Path)
            return;

        // Ensure the logs aren't null
        if (logs is null)
            return;

        AssembledEvent?.Invoke(this, logs);
    }
}
