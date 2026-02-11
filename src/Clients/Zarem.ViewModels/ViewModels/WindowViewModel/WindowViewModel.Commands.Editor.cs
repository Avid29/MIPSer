// Avishai Dernis 2025

using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Zarem.Messages.Editor;
using Zarem.Messages.Editor.Enums;

namespace Zarem.ViewModels;

public partial class WindowViewModel
{
    /// <summary>
    /// Gets a command that un-does an action in the editor.
    /// </summary>
    public RelayCommand UndoCommand { get; }

    /// <summary>
    /// Gets a command that re-does an action in the editor.
    /// </summary>
    public RelayCommand RedoCommand { get; }

    /// <summary>
    /// Gets a command that cuts in the editor.
    /// </summary>
    public RelayCommand CutCommand { get; }

    /// <summary>
    /// Gets a command that copies in the editor.
    /// </summary>
    public RelayCommand CopyCommand { get; }

    /// <summary>
    /// Gets a command that pastes in the editor.
    /// </summary>
    public RelayCommand PasteCommand { get; }

    /// <summary>
    /// Gets a command that duplicates the current line in the editor.
    /// </summary>
    public RelayCommand DuplicateCommand { get; }

    /// <summary>
    /// Gets a command that selects all content in the editor.
    /// </summary>
    public RelayCommand SelectAllCommand { get; }

    /// <summary>
    /// Gets a command that moves the current line in the editor up.
    /// </summary>
    public RelayCommand TransposeUpCommand { get; }

    /// <summary>
    /// Gets a command that moves the current line in the editor down.
    /// </summary>
    public RelayCommand TransposeDownCommand { get; }

    /// <summary>
    /// Gets a command that toggles outlining in the editor.
    /// </summary>
    public RelayCommand ToggleOutliningCommand { get; }

    /// <summary>
    /// Gets a command that expands all children folds from the current line in the editor.
    /// </summary>
    public RelayCommand ExpandCurrentCommand { get; }

    /// <summary>
    /// Gets a command that collapses all children folds from the current line in the editor.
    /// </summary>
    public RelayCommand CollapseCurrentCommand { get; }

    /// <summary>
    /// Gets a command that expands all folds from the current line in the editor.
    /// </summary>
    public RelayCommand ExpandAllCommand { get; }

    /// <summary>
    /// Gets a command that collapses all folds from the current line in the editor.
    /// </summary>
    public RelayCommand CollapseAllCommand { get; }

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
