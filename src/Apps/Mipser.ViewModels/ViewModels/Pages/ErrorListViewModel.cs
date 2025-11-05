// Avishai Dernis 2025

using CommunityToolkit.Mvvm.DependencyInjection;
using CommunityToolkit.Mvvm.Messaging;
using MIPS.Assembler;
using MIPS.Assembler.Logging;
using MIPS.Assembler.Models;
using Mipser.Messages.Build;
using Mipser.ViewModels.Pages.Abstract;
using System.Collections.ObjectModel;

namespace Mipser.ViewModels.Pages;

/// <summary>
/// A view model for the error list.
/// </summary>
public class ErrorListViewModel : PageViewModel
{
    private IMessenger _messenger;

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
        _messenger.Register<ErrorListViewModel, AssembleFileRequestMessage>(this, (r, _) => r.HandleAssembly());
    }

    /// <inheritdoc/>
    public override string Title => "Error List"; // TODO: Localization

    /// <summary>
    /// Gets the list of errors, warnings, and messages.
    /// </summary>
    public ObservableCollection<Log> Messages { get; }

    private async void HandleAssembly()
    {
        var currentFile = Ioc.Default.GetRequiredService<MainViewModel>().CurrentFilePage?.File;
        if (currentFile is null)
            return;

        var stream = await currentFile.GetStreamAsync();
        if (stream is null)
            return;

        var assembly = await Assembler.AssembleAsync(stream, currentFile.Name, new AssemblerConfig());

        Messages.Clear();
        foreach(var log in assembly.Logs)
            Messages.Add(log);
    }
}
