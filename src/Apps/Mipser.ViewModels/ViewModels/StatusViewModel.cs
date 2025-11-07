// Avishai Dernis 2025

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;
using Mipser.Messages.Build;
using Mipser.Models.Enums;
using Mipser.Services.Build;

namespace Mipser.ViewModels;

/// <summary>
/// A view model for the status bar.
/// </summary>
public class StatusViewModel : ObservableRecipient
{
    private readonly IMessenger _messenger;

    private BuildStatus _buildStatus;

    /// <summary>
    /// Initializes a new instance of the <see cref="StatusViewModel"/> class.
    /// </summary>
    public StatusViewModel(IMessenger messenger, BuildService buildService)
    {
        _messenger = messenger;

        BuildStatus = buildService.Status;

        IsActive = true;
    }

    /// <inheritdoc/>
    protected override void OnActivated()
    {
        _messenger.Register<StatusViewModel, BuildStatusMessage>(this, (r, m) => r.BuildStatus = m.Status);
    }

    /// <summary>
    /// Gets the current build status.
    /// </summary>
    public BuildStatus BuildStatus
    {
        get => _buildStatus;
        private set => SetProperty(ref _buildStatus, value);
    }
}
