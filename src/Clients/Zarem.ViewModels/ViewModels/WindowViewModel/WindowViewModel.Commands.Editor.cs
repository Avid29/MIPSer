// Avishai Dernis 2025

using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Zarem.Messages.Editor;
using Zarem.Messages.Editor.Enums;

namespace Zarem.ViewModels;

public partial class WindowViewModel
{
    [RelayCommand]
    private void Undo() => SendEdit(EditorOperation.Undo);

    [RelayCommand]
    private void Redo() => SendEdit(EditorOperation.Redo);

    [RelayCommand]
    private void Cut() => SendEdit(EditorOperation.Cut);

    [RelayCommand]
    private void Copy() => SendEdit(EditorOperation.Copy);

    [RelayCommand]
    private void Paste() => SendEdit(EditorOperation.Paste);

    [RelayCommand]
    private void Duplicate() => SendEdit(EditorOperation.Duplicate);

    [RelayCommand]
    private void SelectAll() => SendEdit(EditorOperation.SelectAll);

    [RelayCommand]
    private void TransposeUp() => SendEdit(EditorOperation.TransposeUp);

    [RelayCommand]
    private void TransposeDown() => SendEdit(EditorOperation.TransposeDown);

    [RelayCommand]
    private void ToggleOutlining() => SendEdit(EditorOperation.ToggleOutlining);

    [RelayCommand]
    private void ExpandChildren() => SendEdit(EditorOperation.ExpandChildren);

    [RelayCommand]
    private void CollapseChildren() => SendEdit(EditorOperation.CollapseChildren);

    [RelayCommand]
    private void ExpandAll() => SendEdit(EditorOperation.ExpandAll);

    [RelayCommand]
    private void CollapseAll() => SendEdit(EditorOperation.CollapseAll);

    private void SendEdit(EditorOperation operation) => _messenger.Send(new EditorOperationRequestMessage(operation));
}
