// Avishai Dernis 2025

using CommunityToolkit.Mvvm.Messaging;
using MIPS.Assembler;
using MIPS.Assembler.Models;
using Mipser.Bindables.Files;
using Mipser.Messages.Build;
using Mipser.Services.Files;
using System.Threading.Tasks;

namespace Mipser.Services.ProjectService;

/// <summary>
/// A service for managing the build status
/// </summary>
public class BuildService
{
    private IMessenger _messenger;
    private FileService _fileService;

    /// <summary>
    /// Initializes a new instance of the <see cref="BuildService"/> class.
    /// </summary>
    public BuildService(IMessenger messenger, FileService fileService)
    {
        _messenger = messenger;
        _fileService = fileService;
    }

    /// <summary>
    /// Assembles a file.
    /// </summary>
    /// <param name="file">The file to assemble.</param>
    public async Task AssembleFileAsync(BindableFile file)
    {
        // Get the file contents as a stream
        var stream = await file.GetStreamAsync();
        if (stream is null)
            return;

        // Assemble the file
        var assembly = await Assembler.AssembleAsync(stream, file.Name, new AssemblerConfig());

        // Send a message with the build results.
        _messenger.Send(new BuildFinishedMessage(assembly.Failed, assembly.Logs));
    }
}
