// Avishai Dernis 2025

using CommunityToolkit.Mvvm.Messaging;
using MIPS.Assembler;
using MIPS.Assembler.Models;
using Mipser.Bindables.Files;
using Mipser.Messages.Build;
using Mipser.Models.Enums;
using Mipser.Services.Project;
using RASM.Modules;
using RASM.Modules.Config;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Mipser.Services.Build;

/// <summary>
/// A service for managing the build status.
/// </summary>
public class BuildService
{
    private readonly IMessenger _messenger;
    private readonly IProjectService _projectService;

    private CancellationTokenSource? _resetToken;
    private BuildStatus _buildStatus;

    /// <summary>
    /// Initializes a new instance of the <see cref="BuildService"/> class.
    /// </summary>
    public BuildService(IMessenger messenger, IProjectService projectService)
    {
        _messenger = messenger;
        _projectService = projectService;

        _buildStatus = BuildStatus.Ready;
    }

    /// <summary>
    /// Assembles a file.
    /// </summary>
    /// <param name="file">The file to assemble.</param>
    public async Task AssembleFileAsync(BindableFile file)
    {
        // Run pre-build checks
        if (!PreBuildChecks())
            return;

        Status = BuildStatus.Assembling;

        // Create save file
        var folder = _projectService.ProjectRootFolder;
        BindableFile? saveFile = null;
        if (folder is not null)
        {
            var saveFilename = Path.GetFileNameWithoutExtension(file.Name) + ".obj";
            saveFile = await folder.CreateFileAsync(saveFilename);
        }

        // Assemble the file
        var assembler = await AssembleFileAsync(file, null, saveFile);

        // Send a message with the build results.
        bool failed = assembler?.Failed ?? false;
        Status = failed ? BuildStatus.Failed : BuildStatus.Completed;
        _messenger.Send(new BuildFinishedMessage(failed, assembler?.Logs));

        // Clear status after some time
        await WaitAndClearStatus();
    }

    private static async Task<Assembler?> AssembleFileAsync(BindableFile file, AssemblerConfig? config, BindableFile? saveLocation = null)
    {
        config ??= new RasmConfig();

        // Get the file contents as a stream
        var stream = await file.GetReadStreamAsync();
        if (stream is null)
            return null;

        // Assemble the file
        var assembler = await Assembler.AssembleAsync(stream, file.Name, config);

        // Return if no save file is provided
        if (saveLocation is null)
            return assembler;

        // Open save file for 
        stream = await saveLocation.GetWriteStreamAsync();
        if (stream is null)
            return null;

        // TODO: Support other module formats
        assembler.CompleteModule<RasmModule>(stream);
        return assembler;
    }

    private bool PreBuildChecks()
    {
        // Ensure the build service is ready to build
        if (!Ready)
        {
            return false;
        }

        // Cancel any scheduled status resets
        _resetToken?.Cancel();
        return true;
    }

    private async Task WaitAndClearStatus()
    {
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
