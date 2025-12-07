// Avishai Dernis 2025

using CommunityToolkit.Mvvm.Messaging;
using MIPS.Assembler;
using MIPS.Assembler.Logging;
using MIPS.Assembler.Models;
using Mipser.Bindables.Files;
using Mipser.Messages.Build;
using Mipser.Models.Enums;
using Mipser.Models.Files;
using Mipser.Services.Files;
using Mipser.Services.Files.Models;
using Mipser.Services.Settings;
using RASM.Modules;
using RASM.Modules.Config;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Mipser.Services;

/// <summary>
/// A service for managing the build status.
/// </summary>
public class BuildService
{
    private readonly IMessenger _messenger;
    private readonly ISettingsService _settingsService;
    private readonly IProjectService _projectService;

    private CancellationTokenSource? _resetToken;
    private BuildStatus _buildStatus;

    /// <summary>
    /// Initializes a new instance of the <see cref="BuildService"/> class.
    /// </summary>
    public BuildService(IMessenger messenger, ISettingsService settingsService, IProjectService projectService, IFileSystemService fileSystemService)
    {
        _messenger = messenger;
        _settingsService = settingsService;
        _projectService = projectService;

        _buildStatus = BuildStatus.Ready;
    }

    /// <summary>
    /// Assembles a file.
    /// </summary>
    /// <param name="files">The file to assemble.</param>
    public async Task AssembleFilesAsync(SourceFile[] files)
    {
        // Run pre-build checks
        if (!PreBuildChecks())
            return;

        // TODO: Report issue
        if (_projectService.Project is null)
            return;

        Status = BuildStatus.Assembling;

        // Culminate results
        bool failed = false;
        var logs = new List<ILog>();

        // Assemble each file
        foreach (var file in files)
        {
            if (file?.FullPath is null)
                continue;

            // Override the current language
            var lang = _settingsService.Local.GetValue<string>("AssemblerLanguageOverride");
            var restore = CultureInfo.CurrentUICulture;
            if (lang is not null)
            {
                var newCulture = CultureInfo.GetCultureInfo(lang);
                CultureInfo.CurrentUICulture = newCulture ?? restore;
            }

            // Assemble the file
            var result = await _projectService.Project.AssembleFileAsync(file);
            
            // Restore the original language
            CultureInfo.CurrentUICulture = restore;

            // Check the assembler result
            if (result is null)
                continue; // TODO: Handle file loading errors

            // Update the status and log
            var assemblerFailed = result?.Failed ?? false;
            failed = failed || (result?.Failed ?? false);
            Status = failed ? BuildStatus.Failing : BuildStatus.Assembling;
            if (result?.Logs is not null)
                logs.AddRange(result.Logs);

            _messenger.Send(new FileAssembledMessage(file.RelativePath, assemblerFailed, result?.Logs));
        }

        // Send a message with the build results.
        Status = failed ? BuildStatus.Failed : BuildStatus.Completed;
        _messenger.Send(new BuildFinishedMessage(failed, logs));

        // Clear status after some time
        await WaitAndClearStatus();
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
            // Update value and cache old value
            var old = _buildStatus;
            _buildStatus = value;

            // Check if the value actually changed.
            if (old != value)
            {
                _messenger.Send(new BuildStatusMessage(value));
            }
        }
    }

    private bool Ready => _buildStatus is BuildStatus.Ready or BuildStatus.Completed or BuildStatus.Failed;
}
