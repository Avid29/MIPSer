// Avishai Dernis 2025

using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Mipser.Messages.Editor;
using Mipser.Messages.Editor.Enums;

namespace Mipser.ViewModels;

public partial class WindowViewModel
{
    /// <summary>
    /// Gets a command that cuts in the editor.
    /// </summary>
    public RelayCommand CutCommand { get; set; }

    /// <summary>
    /// Gets a command that copies in the editor.
    /// </summary>
    public RelayCommand CopyCommand { get; set; }

    /// <summary>
    /// Gets a command that pastes in the editor.
    /// </summary>
    public RelayCommand PasteCommand { get; set; }

    /// <summary>
    /// Gets a command that duplicates the current line in the editor.
    /// </summary>
    public RelayCommand DuplicateCommand { get; set; }

    /// <summary>
    /// Gets a command that moves the current line in the editor up.
    /// </summary>
    public RelayCommand TransposeUpCommand { get; set; }

    /// <summary>
    /// Gets a command that moves the current line in the editor down.
    /// </summary>
    public RelayCommand TransposeDownCommand { get; set; }

    private void Cut() => _messenger.Send(new EditorOperationRequestMessage(EditorOperation.Cut));

    private void Copy() => _messenger.Send(new EditorOperationRequestMessage(EditorOperation.Copy));

    private void Paste() => _messenger.Send(new EditorOperationRequestMessage(EditorOperation.Paste));

    private void Duplicate() => _messenger.Send(new EditorOperationRequestMessage(EditorOperation.Duplicate));

    private void TransposeUp() => _messenger.Send(new EditorOperationRequestMessage(EditorOperation.TransposeUp));

    private void TransposeDown() => _messenger.Send(new EditorOperationRequestMessage(EditorOperation.TransposeDown));
}
