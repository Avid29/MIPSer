// Avishai Dernis 2025

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;
using Mipser.Messages.Build;
using Mipser.Models.Enums;
using Mipser.Services;

namespace Mipser.ViewModels;

/// <summary>
/// A view model for the status bar.
/// </summary>
public class StatusViewModel : ObservableRecipient
{
    private readonly IMessenger _messenger;

    private BuildStatus _buildStatus;
    private string? _statusMessage;

    /// <summary>
    /// Initializes a new instance of the <see cref="StatusViewModel"/> class.
    /// </summary>
    public StatusViewModel(IMessenger messenger, BuildService buildService)
    {
        _messenger = messenger;

        BuildStatus = buildService.Status;
        StatusMessage = string.Empty;

        IsActive = true;
    }

    /// <inheritdoc/>
    protected override void OnActivated()
    {
        _messenger.Register<StatusViewModel, BuildStatusMessage>(this, (r, m) =>
        {
            r.BuildStatus = m.Status;
            r.StatusMessage = m.Message;
        });
    }

    /// <summary>
    /// Gets the current build status.
    /// </summary>
    public BuildStatus BuildStatus
    {
        get => _buildStatus;
        private set => SetProperty(ref _buildStatus, value);
    }

    /// <summary>
    /// Gets the build status message.
    /// </summary>
    public string? StatusMessage
    {
        get => _statusMessage;
        private set => SetProperty(ref _statusMessage, value);
    }
}
