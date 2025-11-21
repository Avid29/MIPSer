// Avishai Dernis 2025

using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Mipser.Messages.Editor;
using Mipser.Messages.Editor.Enums;

namespace Mipser.ViewModels;

public partial class WindowViewModel
{
    /// <summary>
    /// Gets a command that un-does an action in the editor.
    /// </summary>
    public RelayCommand UndoCommand { get; set; }

    /// <summary>
    /// Gets a command that re-does an action in the editor.
    /// </summary>
    public RelayCommand RedoCommand { get; set; }

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
    /// Gets a command that selects all content in the editor.
    /// </summary>
    public RelayCommand SelectAllCommand { get; set; }

    /// <summary>
    /// Gets a command that moves the current line in the editor up.
    /// </summary>
    public RelayCommand TransposeUpCommand { get; set; }

    /// <summary>
    /// Gets a command that moves the current line in the editor down.
    /// </summary>
    public RelayCommand TransposeDownCommand { get; set; }

    /// <summary>
    /// Gets a command that toggles outlining in the editor.
    /// </summary>
    public RelayCommand ToggleOutliningCommand { get; set; }

    /// <summary>
    /// Gets a command that expands all children folds from the current line in the editor.
    /// </summary>
    public RelayCommand ExpandCurrentCommand { get; set; }

    /// <summary>
    /// Gets a command that collapses all children folds from the current line in the editor.
    /// </summary>
    public RelayCommand CollapseCurrentCommand { get; set; }

    /// <summary>
    /// Gets a command that expands all folds from the current line in the editor.
    /// </summary>
    public RelayCommand ExpandAllCommand { get; set; }

    /// <summary>
    /// Gets a command that collapses all folds from the current line in the editor.
    /// </summary>
    public RelayCommand CollapseAllCommand { get; set; }

    private void Undo() => _messenger.Send(new EditorOperationRequestMessage(EditorOperation.Undo));

    private void Redo() => _messenger.Send(new EditorOperationRequestMessage(EditorOperation.Redo));

    private void Cut() => _messenger.Send(new EditorOperationRequestMessage(EditorOperation.Cut));

    private void Copy() => _messenger.Send(new EditorOperationRequestMessage(EditorOperation.Copy));

    private void Paste() => _messenger.Send(new EditorOperationRequestMessage(EditorOperation.Paste));

    private void Duplicate() => _messenger.Send(new EditorOperationRequestMessage(EditorOperation.Duplicate));

    private void SelectAll() => _messenger.Send(new EditorOperationRequestMessage(EditorOperation.SelectAll));

    private void TransposeUp() => _messenger.Send(new EditorOperationRequestMessage(EditorOperation.TransposeUp));

    private void TransposeDown() => _messenger.Send(new EditorOperationRequestMessage(EditorOperation.TransposeDown));

    private void ToggleOutlining() => _messenger.Send(new EditorOperationRequestMessage(EditorOperation.ToggleOutlining));

    private void ExpandChildren() => _messenger.Send(new EditorOperationRequestMessage(EditorOperation.ExpandChildren));
    
    private void CollapseChildren() => _messenger.Send(new EditorOperationRequestMessage(EditorOperation.CollapseChildren));

    private void ExpandAll() => _messenger.Send(new EditorOperationRequestMessage(EditorOperation.ExpandAll));
    
    private void CollapseAll() => _messenger.Send(new EditorOperationRequestMessage(EditorOperation.CollapseAll));
}
