// Avishai Dernis 2025

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;
using Zarem.Messages.Build;
using Zarem.Models.Enums;
using Zarem.Services;

namespace Zarem.ViewModels;

/// <summary>
/// A view model for the status bar.
/// </summary>
public class StatusViewModel : ObservableRecipient
{
    private readonly IMessenger _messenger;

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
        get => field;
        private set => SetProperty(ref field, value);
    }

    /// <summary>
    /// Gets the build status message.
    /// </summary>
    public string? StatusMessage
    {
        get;
        private set => SetProperty(ref field, value);
    }
}
