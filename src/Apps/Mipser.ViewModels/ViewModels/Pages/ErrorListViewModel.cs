// Avishai Dernis 2025

using CommunityToolkit.Mvvm.Messaging;
using MIPS.Assembler.Logging;
using Mipser.Messages.Build;
using Mipser.ViewModels.Pages.Abstract;
using System.Collections.ObjectModel;

namespace Mipser.ViewModels.Pages;

/// <summary>
/// A view model for the error list.
/// </summary>
public class ErrorListViewModel : PageViewModel
{
    private readonly IMessenger _messenger;

    /// <summary>
    /// Initializes a new instance of the <see cref="ErrorListViewModel"/> class.
    /// </summary>
    public ErrorListViewModel(IMessenger messenger)
    {
        _messenger = messenger;

        Messages = [];

        IsActive = true;
    }
    
    /// <inheritdoc/>
    protected override void OnActivated()
    {
        _messenger.Register<ErrorListViewModel, BuildFinishedMessage>(this, (r, m) => r.UpdateLog(m));
    }

    /// <inheritdoc/>
    public override string Title => "Error List"; // TODO: Localization

    /// <summary>
    /// Gets the list of errors, warnings, and messages.
    /// </summary>
    public ObservableCollection<Log> Messages { get; }

    private void UpdateLog(BuildFinishedMessage message)
    {
        Messages.Clear();
        foreach(var log in message.Logs)
            Messages.Add(log);
    }
}
