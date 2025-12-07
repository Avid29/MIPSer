// Avishai Dernis 2025

using CommunityToolkit.Mvvm.Messaging;
using Microsoft.Extensions.Logging;
using MIPS.Assembler.Logging;
using Mipser.Messages.Build;
using Mipser.Models;
using Mipser.Models.Enums;
using Mipser.Models.Files;
using Mipser.Services.Files;
using Mipser.Services.Settings;
using System;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Mipser.Services;

/// <summary>
/// A service for managing the build status.
/// </summary>
public class BuildService
{
    private readonly IMessenger _messenger;
    private readonly ILocalizationService _localizationService;
    private readonly ISettingsService _settingsService;
    private readonly IProjectService _projectService;

    private CancellationTokenSource? _resetToken;

    /// <summary>
    /// Initializes a new instance of the <see cref="BuildService"/> class.
    /// </summary>
    public BuildService(IMessenger messenger, ILocalizationService localizationService, ISettingsService settingsService, IProjectService projectService, IFileSystemService fileSystemService)
    {
        _messenger = messenger;
        _localizationService = localizationService;
        _settingsService = settingsService;
        _projectService = projectService;

        SetStatus(BuildStatus.Ready);
    }

    /// <summary>
    /// Gets the build status
    /// </summary>
    public BuildStatus Status { get; private set; }

    private bool Ready => Status is BuildStatus.Ready or BuildStatus.Completed or BuildStatus.Failed;

    /// <summary>
    /// Builds the project.
    /// </summary>
    public async Task<BuildResult?> BuildProjectAsync(bool rebuild = false)
    {
        // TODO: Report issue
        if (_projectService.Project is null)
            return null;

        var logger = new Logger();
        var task = _projectService.Project.BuildAsync(rebuild, logger);
        return await BuildAsync(task, logger);
    }

    /// <summary>
    /// Assembles a file.
    /// </summary>
    /// <param name="files">The file to assemble.</param>
    public async Task<BuildResult?> AssembleFilesAsync(SourceFile[] files)
    {
        // TODO: Report issue
        if (_projectService.Project is null)
            return null;

        var logger = new Logger();
        var task = _projectService.Project.AssembleFilesAsync(files, true, logger);
        return await BuildAsync(task, logger);
    }

    private async Task<BuildResult?> BuildAsync(Task<BuildResult?> buildTask, Logger logger)
    {
        // Run pre-build checks
        if (!PreBuildChecks())
            return null;

        SetStatus(BuildStatus.Assembling);

        // Culminate results
        _messenger.Send(new BuildStartedMessage(logger));

        // Override the current language
        var lang = _settingsService.Local.GetValue<string>(SettingsKeys.AssemblerLanguageOverride);
        var restore = CultureInfo.CurrentUICulture;
        if (lang is not null)
        {
            var newCulture = CultureInfo.GetCultureInfo(lang);
            CultureInfo.CurrentUICulture = newCulture ?? restore;
        }

        // Run the build task
        var result = await buildTask;

        // Restore the original language
        CultureInfo.CurrentUICulture = restore;

        // Send a message with the build results.
        var message = ConstructMessage(result);
        var status = logger.Failed ? BuildStatus.Failed : BuildStatus.Completed;
        SetStatus(status, message);

        _messenger.Send(new BuildFinishedMessage(result));

        // Clear status after some time
        _ = WaitAndClearStatusAsync();

        return result;
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

    private void SetStatus(BuildStatus value, string? message = null)
    {
        // Update value and cache old value
        var old = Status;
        Status = value;

        // Check if the value actually changed.
        if (old != value)
        {
            _messenger.Send(new BuildStatusMessage(value, message));
        }
    }

    private async Task WaitAndClearStatusAsync()
    {
        // Wait 5 seconds, then clear the status (unless cancelled)
        _resetToken = new CancellationTokenSource();
        var token = _resetToken.Token;
        await Task.Delay(TimeSpan.FromSeconds(5));
        if (token.IsCancellationRequested)
            return;

        SetStatus(BuildStatus.Ready);
    }

    private string? ConstructMessage(BuildResult? result)
    {
        if (result is null)
            return null;

        StringBuilder message = new StringBuilder();

        void Append(string oneKey, string multiKey, int value)
        {
            if (value is 0)
                return;

            if (message.Length is not 0)
                message.Append(" - ");
            
            var key = value is 1 ? oneKey : multiKey;
            message.Append(_localizationService[key, value]);
        }

        Append("BuildStatus/OneSucceeded", "BuildStatus/Succeeded", result.SucessfullyAssembledFiles.Count);
        Append("BuildStatus/OneFailed", "BuildStatus/Failed", result.FailedFiles.Count);
        Append("BuildStatus/OneSkipped", "BuildStatus/Skipped", result.SkippedFiles.Count);
        return $"{message}";
    }
}
