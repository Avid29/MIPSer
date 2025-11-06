// Avishai Dernis 2025

using CommunityToolkit.Mvvm.Messaging;
using MIPS.Assembler;
using MIPS.Assembler.Models;
using Mipser.Bindables.Files;
using Mipser.Messages.Build;
using Mipser.Models.Enums;
using Mipser.Services.Files;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Mipser.Services.ProjectService;

/// <summary>
/// A service for managing the build status
/// </summary>
public class BuildService
{
    private readonly IMessenger _messenger;
    private readonly IFileService _fileService;

    private CancellationTokenSource? _resetToken;
    private BuildStatus _buildStatus;

    /// <summary>
    /// Initializes a new instance of the <see cref="BuildService"/> class.
    /// </summary>
    public BuildService(IMessenger messenger, IFileService fileService)
    {
        _messenger = messenger;
        _fileService = fileService;

        _buildStatus = BuildStatus.Ready;
    }

    /// <summary>
    /// Assembles a file.
    /// </summary>
    /// <param name="file">The file to assemble.</param>
    public async Task AssembleFileAsync(BindableFile file)
    {
        // Ensure the build service is ready to build
        if (!Ready)
            return;

        // Cancel any scheduled status resets
        _resetToken?.Cancel();

        Status = BuildStatus.Preparing;

        // Get the file contents as a stream
        var stream = await file.GetStreamAsync();
        if (stream is null)
            return;

        Status = BuildStatus.Assembling;

        // Assemble the file
        var assembly = await Assembler.AssembleAsync(stream, file.Name, new AssemblerConfig());

        // Send a message with the build results.
        Status = assembly.Failed ? BuildStatus.Failed : BuildStatus.Completed;
        _messenger.Send(new BuildFinishedMessage(assembly.Failed, assembly.Logs));

        // Wait 5 seconds, then clear the status (unless cancelled)
        _resetToken = new CancellationTokenSource();
        var token = _resetToken.Token;
        await Task.Delay(TimeSpan.FromSeconds(5));
        if (token.IsCancellationRequested)
            return;

        Status = BuildStatus.Ready;
    }

    /// <summary>
    /// Gets the build status
    /// </summary>
    public BuildStatus Status
    {
        get => _buildStatus;
        set
        {
            _buildStatus = value;
            _messenger.Send(new BuildStatusMessage(value));
        }
    }

    private bool Ready => _buildStatus is BuildStatus.Ready or BuildStatus.Completed or BuildStatus.Failed;
}
