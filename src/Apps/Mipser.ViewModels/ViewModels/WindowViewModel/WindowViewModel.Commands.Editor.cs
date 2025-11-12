// Avishai Dernis 2025

using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Mipser.Messages.Editor;
using Mipser.Messages.Editor.Enums;

namespace Mipser.ViewModels;

public partial class WindowViewModel
{
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

    private void Duplicate() => _messenger.Send(new EditorOperationRequestMessage(EditorOperation.Duplicate));

    private void TransposeUp() => _messenger.Send(new EditorOperationRequestMessage(EditorOperation.TransposeUp));

    private void TransposeDown() => _messenger.Send(new EditorOperationRequestMessage(EditorOperation.TransposeDown));
}
